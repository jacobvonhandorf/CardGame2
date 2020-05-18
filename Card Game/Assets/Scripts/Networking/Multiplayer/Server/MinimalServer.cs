using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class MinimalServer : MonoBehaviour
{
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int BYTE_SIZE = 1024;

    private int reliableChannel;
    private int hostId;

    void Start()
    {
        NetworkTransport.Init();
        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannel = connectionConfig.AddChannel(QosType.Reliable);
        HostTopology topo = new HostTopology(connectionConfig, MAX_USER);
        hostId = NetworkTransport.AddHost(topo, PORT);
    }

    void Update()
    {
        byte[] recBuffer = new byte[BYTE_SIZE];

        NetworkEventType type = NetworkTransport.Receive(out int recHostId, out int connectionId, out int channelId, recBuffer, BYTE_SIZE, out int dataSize, out byte error);
        if ((NetworkError)error != NetworkError.Ok)
            Debug.LogError("Message receive error: " + (NetworkError)error);
        switch (type)
        {
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                string msg = (string)formatter.Deserialize(ms);
                Debug.Log(msg);
                SendClient("Message from server to client", recHostId, connectionId, channelId);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("User " + connectionId + " has connected!");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("User " + connectionId + " has disconnected");
                break;
        }
    }

    private void SendClient(string msg, int clientHostId, int cnnId, int chId)
    {
        // buffer that holds the data being sent
        byte[] buffer = new byte[BYTE_SIZE];

        // This is where you crush your data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg); // if something isn't serializable this line will error

        NetworkTransport.Send(clientHostId, cnnId, chId, buffer, BYTE_SIZE, out byte error);

        if ((NetworkError)error != NetworkError.Ok)
            Debug.LogError("Message send error: " + (NetworkError)error);
    }

    private void OnApplicationQuit()
    {
        NetworkTransport.Shutdown();
    }
}
