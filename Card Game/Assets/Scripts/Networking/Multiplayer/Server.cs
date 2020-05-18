using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    public static Server Instance { private set; get; }

    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE = 1024;

    private byte reliableChannel;
    private int hostId;
    private int webHostId;

    private bool isStarted = false;
    private byte error;

    private Datastore db;
    private Dictionary<int, int> pairedConnectionIds = new Dictionary<int, int>();
    private Dictionary<int, int> connectionIdToHostId = new Dictionary<int, int>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
        Instance = this;
    }
    private void Update()
    {
        UpdateMessagePump();
    }


    public void Init()
    {
        // lower frame rate so server doesn't get overloaded doing nothing
        Application.targetFrameRate = 20;

        NetworkTransport.Init();

        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannel = connectionConfig.AddChannel(QosType.Reliable);
        HostTopology topo = new HostTopology(connectionConfig, MAX_USER);

        // Server only code
        hostId = NetworkTransport.AddHost(topo, PORT, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT);

        db = new Datastore();
        db.init();

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));
        isStarted = true;
    }
    /*
    private void OnApplicationQuit()
    {
        Shutdown();
    }*/
    public void Shutdown()
    {
        isStarted = false;
        //NetworkTransport.Shutdown();
    }
    public void UpdateMessagePump()
    {
        if (!isStarted)
            return;

        byte[] recBuffer = new byte[BYTE_SIZE];
        NetworkEventType type = NetworkTransport.Receive(out int recHostId, out int connectionId, out int channelId, recBuffer, BYTE_SIZE, out int dataSize, out error);
        switch (type)
        {
            case NetworkEventType.DataEvent:
                Debug.Log("Recieve data event");
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);
                OnData(connectionId, channelId, recHostId, msg);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("User " + connectionId + " has connected!");
                break;
            case NetworkEventType.DisconnectEvent:
                onDisconnect(recHostId, connectionId, channelId);
                break;
            case NetworkEventType.Nothing:
                break;
            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        }
    }

    #region OnData
    private void OnData(int cnnId, int channelId, int recHostId, NetMsg msg)
    {
        switch (msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.CreateAccount:
                Debug.Log("Recieved create account");
                CreateAccount(cnnId, channelId, recHostId, (Net_CreateAccount)msg);
                break;
            case NetOP.LoginRequest:
                LoginRequest(cnnId, channelId, recHostId, (Net_LoginRequest)msg);
                break;
            case NetOP.EnterMMPool:
                EnterMMPool(cnnId, channelId, recHostId, (Net_EnterMMPool)msg);
                break;
            case NetOP.InGameRelay:
                RelayNetMessage(cnnId, channelId, recHostId, (Net_InGameRelay)msg);
                break;
            default:
                Debug.LogError("Unexpected NETOP code: " + msg.OP);
                break;
        }
    }
    private void CreateAccount(int cnnId, int channelId, int recHostId, Net_CreateAccount ca)
    {
        Net_CreateAccountResponse resp = new Net_CreateAccountResponse();
        Debug.Log("Before create account");
        resp.success = db.createAccount(ca.Username, ca.Password, ca.Email);
        Debug.Log("After create account");
        SendClient(recHostId, cnnId, channelId, resp);
        Debug.Log("After send client");
    }
    private void EnterMMPool(int cnnId, int channelId, int recHostId, Net_EnterMMPool msg)
    {
        Debug.Log("Adding " + cnnId + " to matchmaking pool");
        MatchMakerObject mmo = new MatchMakerObject();
        mmo.channelId = channelId;
        mmo.connectionId = cnnId;
        mmo.hostId = recHostId;
        MatchMaker.getInstance().queueMatchMakerObject(mmo);
    }
    private void LoginRequest(int cnnId, int channelId, int recHostId, Net_LoginRequest msg)
    {
        string randomToken = AccountUtils.GenerateRandom(256);
        Model_Account account = db.loginAccount(msg.UsernameOrEmail, msg.Password, cnnId, randomToken);

        Net_LoginRequestResponse response = new Net_LoginRequestResponse();
        if (account != null)
        {
            // login was successful
            response.success = 0;
            response.information = "Login was successful!";
            response.connectionId = cnnId;
            response.token = randomToken;
            response.username = account.username;
            response.discriminator = account.discriminator;
            Debug.Log("login attempt successful");
        }
        else
        {
            // login was not successful
            response.success = 1;
            response.information = "Login attempt failed";
            Debug.Log("login attempt failed");
        }

        SendClient(recHostId, cnnId, channelId, response); // let the client know if their login attempt was successful or not
    }
    private void RelayNetMessage(int cnnId, int channelId, int recHostId, Net_InGameRelay msg)
    {
        int targetCnId = pairedConnectionIds[cnnId];
        int targetHostId = connectionIdToHostId[targetCnId];

        Debug.LogError("Relaying message " + msg.msg.OP);
        // get the user we need to message from the map and then relay the message to them
        SendClient(targetHostId, targetCnId, channelId, msg.msg); // Might need to change HostId. If so then it needs to be stored along with connection id in map
    }
    #endregion

    #region Send
    public void SendClient(int recHost, int cnnId, int channelId, NetMsg msg)
    {
        byte[] buffer = new byte[BYTE_SIZE];
        Debug.Log("host: " + recHost + " cnnId" + cnnId + "channel Id" + channelId);
        // This is where you crush your data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg); // if something isn't serializable this line will error

        NetworkTransport.Send(recHost, cnnId, channelId, buffer, BYTE_SIZE, out error);

        if ((NetworkError)error != NetworkError.Ok)
            Debug.LogError("Message send error: " + (NetworkError)error);
    }
    #endregion

    #region OtherServerOperations
    public void pairUsers(MatchMakerObject mmo1, MatchMakerObject mmo2)
    {
        // add users to map and let them know they've been paired up
        pairedConnectionIds.Add(mmo1.connectionId, mmo2.connectionId);
        pairedConnectionIds.Add(mmo2.connectionId, mmo1.connectionId);
        connectionIdToHostId.Add(mmo1.connectionId, mmo1.hostId);
        connectionIdToHostId.Add(mmo2.connectionId, mmo2.hostId);
        Net_NotifyClientOfMMPairing mmNotification = new Net_NotifyClientOfMMPairing();
        // designate a palyer as player 1
        mmNotification.isPlayer1 = true;
        SendClient(mmo1.hostId, mmo1.connectionId, mmo1.channelId, mmNotification);
        mmNotification.isPlayer1 = false;
        SendClient(mmo2.hostId, mmo2.connectionId, mmo2.channelId, mmNotification);
        //printConnectionMap();
    }
    private void onDisconnect(int recHostId, int cnnId, int channelId)
    {
        // TODO issue game loss
        if (pairedConnectionIds.TryGetValue(cnnId, out int pairedCnnId))
        {
            connectionIdToHostId.Remove(pairedCnnId);
            pairedConnectionIds.Remove(pairedCnnId);
        }

        if (pairedConnectionIds.ContainsKey(cnnId))
            pairedConnectionIds.Remove(cnnId);

        connectionIdToHostId.Remove(cnnId);

        //printConnectionMap();
    }
    public void printConnectionMap()
    {
        foreach (int key in pairedConnectionIds.Keys)
        {
            Debug.LogError("Key " + key + ", Value: " + pairedConnectionIds[key]);
        }
    }
    #endregion
}
