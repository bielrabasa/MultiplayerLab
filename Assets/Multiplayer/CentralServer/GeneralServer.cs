using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GeneralServer : MonoBehaviour
{
    const int MAX_PLAYERS = 2;
    int connectedPlayers = 0;

    Thread waitingClientThread;
    
    Socket socket;
    EndPoint[] remote;
    
    int port;

    void Start()
    {
        remote = new EndPoint[MAX_PLAYERS];
        waitingClientThread = new Thread(WaitClient);

        //Setup port
        ServerSetup();
        Debug.Log("IP: " + GetMyIp() + "\tPORT: " + port.ToString());
        waitingClientThread.Start();

        //Set port in screen
        //GameObject.Find("Port").GetComponent<Text>().text = "Port: " + port.ToString();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            StartPlaying();
        }
    }

    void ServerSetup()
    {
        //Create Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

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
        while (connectedPlayers < MAX_PLAYERS)
        {
            //Setup new remote
            RemoteSetup();

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
                return;
            }

            //Recieved message
            string message = Encoding.ASCII.GetString(recieveData, 0, recv);

            //Incorrect message
            if (message != "ClientConnected")
            {
                Debug.Log("Incorrect confirmation message: " + message);
                return;
            }

            //Send Confirmation Message
            byte[] sendData = Encoding.ASCII.GetBytes("ServerConnected");
            socket.SendTo(sendData, sendData.Length, SocketFlags.None, remote[connectedPlayers]);

            //End
            connectedPlayers++;
            Debug.Log("Connected Player " +  connectedPlayers);
        }
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

        //SendStart message
        for (int i = 0; i < connectedPlayers; i++)
        {
            byte[] sendData = Encoding.ASCII.GetBytes(i + "StartGame");
            socket.SendTo(sendData, sendData.Length, SocketFlags.None, remote[i]);
        }

        //ChangeScene
        StartComunication();
    }

    void StartComunication()
    {
        //Create instance
        gameObject.AddComponent<ServerReceiver>();

        //Setup variables
        ServerReceiver.socket = socket;

        //Create new remote to receive game messages
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, connectedPlayers);
        ServerReceiver.remote = (EndPoint)(sender);

        ServerReceiver.messagers = new ServerMessager[connectedPlayers];
        for (int i = 0; i < connectedPlayers; i++)
        {
            //Setup Messager
            ServerMessager sm = gameObject.AddComponent<ServerMessager>();
            sm.playerID = i;
            sm.socket = socket;
            sm.remote = remote[i];

            //Set ServerMessager array in the ServerReceiver
            ServerReceiver.messagers[i] = sm;
        }
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