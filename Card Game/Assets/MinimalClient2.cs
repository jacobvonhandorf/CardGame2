using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MinimalClient2 : MonoBehaviour
{
    public int port = 8888;
    public Text Txt1;

    private HostTopology topology;
    private ConnectionConfig config;
    private int recConnectionId;
    private int hostId;
    private int recChannelId;

    void Start()
    {
        NetworkTransport.Init();
        config = new ConnectionConfig();

        config.AddChannel(QosType.UnreliableSequenced);
        topology = new HostTopology(config, 5);
        hostId = NetworkTransport.AddHost(topology, port);
        byte error;
        recConnectionId = NetworkTransport.Connect(hostId, "127.0.0.1", port, 0, out error); //Llamo al server

    }

    void Update()
    {

        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType networkEvent = NetworkTransport.Receive(out hostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
        //Txt1.text = “IdS” + hostId + ” IdC=” + recConnectionId;
        NetworkError networkError = (NetworkError)error;
        //Txt1.text = hostId +”-” + recConnectionId + “-” + recChannelId.ToString() + “-” + recBuffer.ToString() + “-” + bufferSize.ToString() + “-” + dataSize.ToString() + “-” + “Error:” + “-” + networkError;
        if (networkError != NetworkError.Ok)
        {
            //Txt1.text = “>” + hostId + recConnectionId.ToString() + “-” + recChannelId.ToString() + “-” + recBuffer.ToString() + “-” + bufferSize.ToString() + “-” + dataSize.ToString() + “-” + “Error:” + “-” + networkError;
        }

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
        formatter.Serialize(stream, "Hello Cliente");

        int bufferSize = 1024;

        NetworkTransport.Send(hostId, recConnectionId, recChannelId, buffer, bufferSize, out error);

    }

}//Clase