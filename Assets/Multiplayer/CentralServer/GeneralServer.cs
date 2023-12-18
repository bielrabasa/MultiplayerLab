using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.tvOS;
using UnityEngine.UI;

public class GeneralServer : MonoBehaviour
{
    const int MAX_PLAYERS = 2;
    int connectedPlayers = 0;

    Thread waitingClientThread;
    bool finishedConnection = false;
    
    Socket socket;
    EndPoint[] remote;
    int port;

    void Start()
    {
        remote = new EndPoint[MAX_PLAYERS];

        waitingClientThread = new Thread(WaitClient);

        //Create Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //Setup port
        ServerSetup();
        Debug.Log("IP: " + GetMyIp() + "\tPORT: " + port.ToString());
        RemoteSetup();
        waitingClientThread.Start();

        //Set port in screen
        //GameObject.Find("Port").GetComponent<Text>().text = "Port: " + port.ToString();
    }

    private void Update()
    {
        //Search for a new client
        if (finishedConnection)
        {
            Debug.Log(connectedPlayers);
            finishedConnection = false;

            if(connectedPlayers < MAX_PLAYERS)
            {
                Debug.Log("Should start waiting for connection again!");
                RemoteSetup();
                waitingClientThread = new Thread(WaitClient);
                waitingClientThread.Start();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            StartPlaying();
        }
    }

    void ServerSetup()
    {
        //Try different ports until one is free
        port = 9000;
        bool correctPort = false;

        while (!correctPort)
        {
            try
            {
                //Create IP info struct
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);

                //Bind Socket to ONLY recieve info from the said port
                socket.Bind(ipep);

                correctPort = true;
            }
            catch
            {
                port++;
            }
        }
    }

    void RemoteSetup()
    {
        //Set port to send the messages
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, connectedPlayers);
        remote[connectedPlayers] = (EndPoint)(sender);
    }

    public void StopConnection()
    {
        socket.Close();
        Debug.Log("SERVER DISCONNECTED");
    }
    
    //THREAD
    private void WaitClient()
    {
        byte[] recieveData = new byte[1024];
        int recv;

        //Recieve message
        try
        {
            recv = socket.ReceiveFrom(recieveData, ref remote[connectedPlayers]);
        }
        catch
        {
            Debug.Log("Server stopped listening! ");
            StopConnection();
            return;
        }

        //Recieved message
        string message = Encoding.ASCII.GetString(recieveData, 0, recv);

        //Incorrect message
        if (message != "ClientConnected")
        {
            Debug.Log("Incorrect confirmation message: " + message);
            StopConnection();
            return;
        }

        //Send Confirmation Message
        byte[] sendData = Encoding.ASCII.GetBytes("ServerConnected");
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, remote[connectedPlayers]);

        //Setup
        finishedConnection = true;
        connectedPlayers++;
    }

    public void StopSearching()
    {
        waitingClientThread.Abort();
    }

    //GAME
    public void StartPlaying()
    {
        if (connectedPlayers == 0) return;

        //Stop searching clients
        StopSearching();

        //TransferInformation();

        //SendStart message
        foreach (var rem in remote)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("StartGame");
            socket.SendTo(sendData, sendData.Length, SocketFlags.None, rem);
        }

        //ChangeScene
        ChangeScene();
    }

    void ChangeScene()
    {
        //SceneManager.LoadScene("MainScene");
        //MessageManager.StartComunication();
        ServerReceiver sr = gameObject.AddComponent<ServerReceiver>();
        sr.socket = socket;

        //Create new remote to receive game messages
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, connectedPlayers);
        sr.remote = (EndPoint)(sender);
    }

    //Screen
    
    public void GetIP(Text text)
    {
        text.text = GetMyIp();
    }

    /*public void GetPort(Text text)
    {
        text.text = port.ToString();
    }*/

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