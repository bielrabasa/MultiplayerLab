using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    Thread messageReciever;

    string serverIP = "127.0.0.1";

    Socket socket;
    EndPoint remote;
    IPEndPoint ipep;

    bool connected;


    private void Start()
    {
        connected = false;
        messageReciever = new Thread(ConnectToServer);
    }

    private void Update()
    {
        if (connected)
        {
            TransferInformation();
            //change scene
            Debug.Log("Change Scene");
        }
    }

    public void SetIP()
    {
        InputField ipInput = FindObjectOfType<InputField>(); 
        string ip = ipInput.text.ToString();
        serverIP = ip;

        StartConnection();
    }

    void StartConnection()
    {
        ClientSetup();
        messageReciever.Start();
    }

    void ClientSetup()
    {
        //Create IP info struct
        ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);

        //Open Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //Set port 0 to send messages
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)sender;
    }

    void SendConfirmation()
    {
        string stringData = "ClientConnected";
        byte[] data = new byte[1024];
        data = Encoding.ASCII.GetBytes(stringData);
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);
    }

    //THREAD
    void ConnectToServer()
    {
        SendConfirmation();

        byte[] data = new byte[1024];
        int recv;

        try
        {
            recv = socket.ReceiveFrom(data, ref remote);
        }
        catch
        {
            Debug.Log("Client stopped listening! ");
            return;
        }

        string message = Encoding.ASCII.GetString(data, 0, recv);

        if(message == "ServerConnected")
        {
            connected = true;
        }
        else
        {
            Debug.Log("Incorrect confirmation message!");
        }
    }

    void TransferInformation()
    {
        MultiplayerState ms = FindObjectOfType<MultiplayerState>();
        ms.socket = socket;
        ms.remote = remote;
        ms.isServer = false;
    }
}
