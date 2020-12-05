using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    public static Server Instance { private set; get; }

    private const string VERSION = "0.3";
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
        // lower poll rate to save CPU
        Application.targetFrameRate = 10;

        NetworkTransport.Init();

        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannel = connectionConfig.AddChannel(QosType.Reliable);
        HostTopology topo = new HostTopology(connectionConfig, MAX_USER);

        hostId = NetworkTransport.AddHost(topo, PORT, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT);


        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));
        isStarted = true;

        db = new Datastore();
        db.Init();
    }
    
    private void OnApplicationQuit()
    {
        Shutdown();
    }
    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }
    public void UpdateMessagePump()
    {
        if (!isStarted)
            return;

        bool loop = true;
        while (loop)
        {
            byte[] recBuffer = new byte[BYTE_SIZE];
            NetworkEventType type = NetworkTransport.Receive(out int recHostId, out int connectionId, out int channelId, recBuffer, BYTE_SIZE, out int dataSize, out error);
            switch (type)
            {
                case NetworkEventType.DataEvent:
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream(recBuffer);
                    NetMsg msg = (NetMsg)formatter.Deserialize(ms);
                    OnData(connectionId, channelId, recHostId, msg);
                    break;
                case NetworkEventType.ConnectEvent:
                    break;
                case NetworkEventType.DisconnectEvent:
                    OnDisconnect(recHostId, connectionId, channelId);
                    break;
                case NetworkEventType.Nothing:
                    loop = false;
                    break;
                default:
                case NetworkEventType.BroadcastEvent:
                    Debug.Log("Unexpected network event type");
                    break;
            }
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
            case NetOP.EndGame:
                EndGame(cnnId, channelId, recHostId, (Net_EndGame)msg);
                break;
            default:
                Debug.LogError("Unexpected NETOP code: " + msg.OP);
                break;
        }
    }
    private void CreateAccount(int cnnId, int channelId, int recHostId, Net_CreateAccount ca)
    {
        Net_CreateAccountResponse resp = new Net_CreateAccountResponse();
        resp.success = db.CreateAccount(ca.Username, ca.Password, ca.Email);
        SendClient(recHostId, cnnId, channelId, resp);
    }
    private void EnterMMPool(int cnnId, int channelId, int recHostId, Net_EnterMMPool msg)
    {
        if (msg.inPool)
        {
            MatchMakerObject mmo = new MatchMakerObject();
            mmo.channelId = channelId;
            mmo.connectionId = cnnId;
            mmo.hostId = recHostId;
            MatchMaker.GetInstance().QueueMatchMakerObject(mmo);
        }
        else
        {
            MatchMakerObject mmo = new MatchMakerObject();
            mmo.channelId = channelId;
            mmo.connectionId = cnnId;
            mmo.hostId = recHostId;
            MatchMaker.GetInstance().RemoveMatchMakerObject(mmo);
        }
    }
    private void LoginRequest(int cnnId, int channelId, int recHostId, Net_LoginRequest msg)
    {
        string randomToken = AccountUtils.GenerateRandomString(256);
        Model_Account account = db.LoginAccount(msg.UsernameOrEmail, msg.Password, cnnId, randomToken);

        Net_LoginRequestResponse response = new Net_LoginRequestResponse();
        if (account != null)
        {
            // login was successful
            response.success = 0;
            response.information = "Login was successful!";
            response.connectionId = cnnId;
            response.token = randomToken;
            response.username = account.Username;
            response.discriminator = account.Discriminator;
        }
        else
        {
            // login was not successful
            response.success = 1;
            response.information = "Login attempt failed";
        }

        SendClient(recHostId, cnnId, channelId, response); // let the client know if their login attempt was successful or not
    }
    private void RelayNetMessage(int cnnId, int channelId, int recHostId, Net_InGameRelay msg)
    {
        int targetCnId = pairedConnectionIds[cnnId];
        int targetHostId = connectionIdToHostId[targetCnId];

        // get the user we need to message from the map and then relay the message to them
        SendClient(targetHostId, targetCnId, channelId, msg.msg);
    }
    private void EndGame(int cnnId, int channelId, int recHostId, Net_EndGame msg)
    {
        // if the server still sees the game as in progress
        if (pairedConnectionIds.TryGetValue(cnnId, out int oppCnnId))
        {
            // unpair users
            int oppHostId = connectionIdToHostId[oppCnnId];
            pairedConnectionIds.Remove(oppCnnId);
            pairedConnectionIds.Remove(cnnId);
            // send message to opponent
            SendClient(oppHostId, oppCnnId, channelId, msg);
        }
    }
    private void CheckClientVersion(int cnnId, int channelId, int recHostId, Net_CheckVersion msg)
    {
        // send a message to the client with the current version
        Net_CheckVersion response = new Net_CheckVersion();
        response.version = VERSION;
        SendClient(recHostId, cnnId, channelId, response);
    }
    #endregion

    #region Send
    public void SendClient(int recHost, int cnnId, int channelId, NetMsg msg)
    {
        byte[] buffer = new byte[BYTE_SIZE];
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg); // if something isn't serializable this line will error

        NetworkTransport.Send(recHost, cnnId, channelId, buffer, BYTE_SIZE, out error);

        if ((NetworkError)error != NetworkError.Ok)
            Debug.LogError("Message send error: " + (NetworkError)error);
    }
    #endregion

    #region OtherServerOperations
    public void PairUsers(MatchMakerObject mmo1, MatchMakerObject mmo2)
    {
        // add users to map and let them know they've been paired up
        pairedConnectionIds.Add(mmo1.connectionId, mmo2.connectionId);
        pairedConnectionIds.Add(mmo2.connectionId, mmo1.connectionId);
        if (!connectionIdToHostId.ContainsKey(mmo1.connectionId))
            connectionIdToHostId.Add(mmo1.connectionId, mmo1.hostId);
        if (!connectionIdToHostId.ContainsKey(mmo2.connectionId))
            connectionIdToHostId.Add(mmo2.connectionId, mmo2.hostId);

        Net_NotifyClientOfMMPairing mmNotification = new Net_NotifyClientOfMMPairing();
        // designate a player as player 1
        mmNotification.isPlayer1 = true;
        SendClient(mmo1.hostId, mmo1.connectionId, mmo1.channelId, mmNotification);
        mmNotification.isPlayer1 = false;
        SendClient(mmo2.hostId, mmo2.connectionId, mmo2.channelId, mmNotification);
    }
    private void OnDisconnect(int recHostId, int cnnId, int channelId)
    {
        MatchMakerObject mmo = new MatchMakerObject();
        mmo.connectionId = cnnId;
        MatchMaker.GetInstance().RemoveMatchMakerObject(mmo);
        if (pairedConnectionIds.TryGetValue(cnnId, out int oppCnnId)) // will be true if they were in game during disconnect
        {
            // notify opponent's client that opponent disconnected
            int oppHostId = connectionIdToHostId[oppCnnId];
            Net_EndGame msg = new Net_EndGame();
            msg.endGameCode = EndGameCode.Disconnect;
            SendClient(oppHostId, oppCnnId, channelId, msg);

            // remove paired connection
            pairedConnectionIds.Remove(oppCnnId);
        }

        if (pairedConnectionIds.ContainsKey(cnnId))
            pairedConnectionIds.Remove(cnnId);

        connectionIdToHostId.Remove(cnnId);
    }

    // for debugging
    public void PrintConnectionMap()
    {
        foreach (int key in pairedConnectionIds.Keys)
        {
            Debug.Log("Key " + key + ", Value: " + pairedConnectionIds[key]);
        }
    }
    #endregion
}
