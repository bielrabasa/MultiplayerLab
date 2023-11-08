using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.tvOS;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Client : MonoBehaviour
{
    Thread messageReciever;
    Thread waitForStart;

    string serverIP = "127.0.0.1";

    Socket socket;
    EndPoint remote;
    IPEndPoint ipep;

    bool startGame;
    bool startConnection;


    private void Start()
    {
        startGame = false;
        startConnection = false;
        messageReciever = new Thread(ConnectToServer);
        waitForStart = new Thread(WaitForStart);

        FillInputField();
    }

    private void Update()
    {
        if (startConnection)
        {
            FullyConnected();
            startConnection = false;
        }

        if(startGame)
        {
            ChangeScene();
            startGame = false;
        }
    }

    void FillInputField()
    {
        InputField ipInput = FindObjectOfType<InputField>();
        ipInput.text = GetMyIp();
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
        ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9889);

        //Open Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //Set port 0 to send messages
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)sender;
    }

    void SendConfirmation()
    {
        byte[] data =  Encoding.ASCII.GetBytes("ClientConnected");
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
            Debug.Log("Client stopped listening!");
            return;
        }

        string message = Encoding.ASCII.GetString(data, 0, recv);

        if(message == "ServerConnected")
        {
            startConnection = true;
        }
        else
        {
            Debug.Log("Incorrect confirmation message: " + message);
        }
    }

    void FullyConnected()
    {
        TransferInformation();
        waitForStart.Start();
    }

    void TransferInformation()
    {
        MultiplayerState ms = FindObjectOfType<MultiplayerState>();
        ms.socket = socket;
        ms.remote = remote;
        ms.isServer = false;
    }

    void WaitForStart() //TODO: function to abort
    {
        Debug.Log("Waiting for the Server to Start...");

        byte[] data = new byte[1024];
        int recv;

        try
        {
            recv = socket.ReceiveFrom(data, ref remote);
        }
        catch
        {
            Debug.Log("Client did not want to wait for Start!");
            return;
        }

        string message = Encoding.ASCII.GetString(data, 0, recv);

        //ChangeScene
        if (message == "StartGame")
        {
            startGame = true;
        }
        else
        {
            Debug.Log("Message recieved to start game is INCORRECT: " + message);
        }
    }
    void ChangeScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    string GetMyIp()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "";
    }
}
