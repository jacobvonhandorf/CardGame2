using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MinimalServer2 : MonoBehaviour
{

    public Text Txt1;

    public int myReliableChannelId;
    public int socketId;
    public int socketPort = 8888;
    public int connectionId;

    void Start()
    {

        NetworkTransport.Init();
        Connectar();

    }

    public void Connectar()
    {
        ConnectionConfig config = new ConnectionConfig();
        int maxConnections = 5;
        config.AddChannel(QosType.UnreliableSequenced);
        HostTopology topology = new HostTopology(config, maxConnections);
        socketId = NetworkTransport.AddHost(topology, socketPort);
        //byte error;
        //connectionId = NetworkTransport.Connect(socketId, “192.168.1.141”, socketPort, 0, out error);
        //Txt1.text = Txt1.text + “SockId: ” + socketId + “ConnId: ” + connectionId + “-” + error.ToString();
    }

    void Update()
    {
        //Recibo del cliente
        int recChannelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType networkEvent = NetworkTransport.Receive(out socketId, out connectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
        switch (networkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Txt1.text = Txt1.text + "incoming";
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                Txt1.text = Txt1.text + message;
                break;
            case NetworkEventType.DisconnectEvent:
                Txt1.text = "Disconnected";
                break;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Sending message");
            SendSocketMessage();
        }

    }

    public void SendSocketMessage()
    {
        byte error;
        byte[] buffer = new byte[1024];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, "Hello del Server ");

        int bufferSize = 1024;

        NetworkTransport.Send(socketId, connectionId, myReliableChannelId, buffer, bufferSize, out error);
        // Txt1.text = socketId.ToString() + connectionId.ToString() + myReliableChannelId.ToString() + buffer.ToString() + bufferSize.ToString() + error.ToString();

    }
}//Clase