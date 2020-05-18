using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    public static Client Instance { private set; get; }
    public static string selectedDeckName;

    // Must stay the same between client and server
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const string SERVER_IP = "127.0.0.1"; // loopback
    //private const string SERVER_IP = "35.224.116.208"; // production server


    private const int BYTE_SIZE = 1024;

    private byte reliableChannel;
    private int connectionId;
    private int hostId;
    private byte error;

    private bool isStarted = false;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Update()
    {
        UpdateMessagePump();
    }

    public void Init()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        hostId = NetworkTransport.AddHost(topo, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
        // Web Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
#else
        // Standalon Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out error);
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.LogError("Message send error: " + (NetworkError)error);
        }

#endif

        Debug.Log(string.Format("Attempting to connect on port {0}", SERVER_IP));
        isStarted = true;
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

        byte[] recBuffer = new byte[BYTE_SIZE];

        NetworkEventType type = NetworkTransport.Receive(out int recHostId, out connectionId, out int channelId, recBuffer, BYTE_SIZE, out int dataSize, out error);
        switch (type)
        {
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);
                OnData(connectionId, channelId, recHostId, msg);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("We have connected to the server");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("We have been disconnected");
                break;
            case NetworkEventType.Nothing:
                break;
            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        }
    }

    // when ereceiving info from server
    #region OnData
    private void OnData(int cnnId, int channelId, int recHostId, NetMsg msg)
    {
        Debug.Log("Receive event. OP: " + msg.OP);
        switch (msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.LoginRequestResponse:
                LoginRequestResponse((Net_LoginRequestResponse)msg);
                break;
            case NetOP.CreateAccountResponse:
                LoginScene.Instance.receiveCreateAccountResponse((Net_CreateAccountResponse)msg);
                break;
            case NetOP.NotifyClientOfMMPairing:
                NetInterface.Get().setIsPlayer1(((Net_NotifyClientOfMMPairing)msg).isPlayer1);
                MMDeckSelectScene.Instance.startGame();
                break;
            case NetOP.InstantiateCard:
                NetInterface.Get().importNewCardFromOpponent((Net_InstantiateCard)msg);
                break;
            case NetOP.MoveCardToPile:
                Net_MoveCardToPile mctp = (Net_MoveCardToPile)msg;
                NetInterface.Get().recieveMoveCardToPile(mctp.cardId, mctp.cpId);
                break;
            case NetOP.SyncDeckOrder:
                Net_SyncDeckOrder sdo = (Net_SyncDeckOrder)msg;
                NetInterface.Get().recieveDeckOrder(sdo.cardIds, sdo.deckCpId);
                break;
            case NetOP.SyncCreatureCoordinates:
                Net_SyncCreatureCoordinates scc = (Net_SyncCreatureCoordinates)msg;
                NetInterface.Get().recieveCreatureCoordinates(scc.creatureCardId, scc.x, scc.y, scc.wasForceMove);
                break;
            case NetOP.SyncAttack:
                Net_SyncAttack sa = (Net_SyncAttack)msg;
                NetInterface.Get().receiveAttack(sa.attackerId, sa.defenderId, sa.damageRoll);
                break;
            case NetOP.SyncPlayerStats:
                Net_SyncPlayerResources spr = (Net_SyncPlayerResources)msg;
                NetInterface.Get().recievePlayerStats(spr.isPlayerOne, spr.gold, spr.goldPTurn, spr.mana, spr.manaPTurn, spr.actions, spr.actionsPTurn);
                break;
            case NetOP.EndTurn:
                NetInterface.Get().recieveEndTurn();
                break;
            case NetOP.DoneSendingCards:
                NetInterface.Get().opponentFinishedSendingCards = true;
                break;
            case NetOP.DoneWithSetup:
                NetInterface.Get().finishedWithSetup = true;
                break;
            case NetOP.SyncPermanentPlaced:
                NetInterface.Get().recievePermanentPlaced((Net_SyncPermanentPlaced)msg);
                break;
            case NetOP.SyncCreature:
                NetInterface.Get().recieveCreatureStats((Net_SyncCreature)msg);
                break;
        }
    }
    private void LoginRequestResponse(Net_LoginRequestResponse lrr)
    {
        LoginScene.Instance.receiveLoginResponse(lrr);
    }
    #endregion
    // all server commands go here
    #region Send
    public void SendServer(NetMsg msg)
    {
        // buffer that holds the data being sent
        byte[] buffer = new byte[BYTE_SIZE];

        // This is where you would crush your data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg); // if something isn't serializable this line will error with MemoryStreamNotExpandable

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);

        if ((NetworkError)error != NetworkError.Ok)
            Debug.LogError("Message send error: " + (NetworkError)error);
    }
    public void enterMMPool()
    {
        Net_EnterMMPool msg = new Net_EnterMMPool();
        SendServer(msg);
    }
    #endregion

    public bool getIsStarted()
    {
        return isStarted;
    }


}
