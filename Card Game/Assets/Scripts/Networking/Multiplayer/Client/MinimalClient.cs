using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class MinimalClient : MonoBehaviour
{
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int BYTE_SIZE = 1024;
    private const string SERVER_IP = "127.0.0.1"; // loopback

    private int reliableChannel;
    private int hostId;
    private int connectionId;

    void Start()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        HostTopology topo = new HostTopology(cc, MAX_USER);
        //hostId = NetworkTransport.AddHost(topo, 0);
        hostId = NetworkTransport.AddHost(topo);
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out byte error);
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.LogError("Message send error: " + (NetworkError)error);
        }
    }

    void Update()
    {
        byte[] recBuffer = new byte[BYTE_SIZE];

        NetworkEventType type = NetworkTransport.Receive(out int recHostId, out int connectionId, out int channelId, recBuffer, BYTE_SIZE, out int dataSize, out byte error);
        if ((NetworkError)error != NetworkError.Ok)
            Debug.LogError("Message send error: " + (NetworkError)error);
        switch (type)
        {
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                string msg = (string)formatter.Deserialize(ms);
                Debug.Log(msg);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("We have connected to the server");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("We have been disconnected");
                break;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q key pressed");
            SendServer("Message from client to server");
        }
    }

    public void SendServer(string msg)
    {
        byte[] buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg); // if something isn't serializable this line will error

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out byte error);

        if ((NetworkError)error != NetworkError.Ok)
            Debug.LogError("Message send error: " + (NetworkError)error);
    }
}
