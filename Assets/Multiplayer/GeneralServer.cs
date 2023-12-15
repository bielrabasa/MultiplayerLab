using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.tvOS;
using UnityEngine.UI;

public class GeneralServer : MonoBehaviour
{
    uint connectedPlayers = 0;

    Thread waitingClientThread;
    bool finishedConnection = false;
    
    Socket socket;
    EndPoint remote;
    int port;

    void Start()
    {
        waitingClientThread = new Thread(WaitClient);

        ServerSetup();
        waitingClientThread.Start();

        //Set port in screen
        //GameObject.Find("Port").GetComponent<Text>().text = "Port: " + port.ToString();
        Debug.Log("IP: " + GetMyIp() + "\tPORT: " + port.ToString());
    }

    private void Update()
    {
        //Search for a new client
        if (finishedConnection)
        {
            Debug.Log(connectedPlayers);
            finishedConnection = false;
            waitingClientThread = new Thread(WaitClient);
            waitingClientThread.Start();
        }

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

        //Set port 0 to send the messages
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)(sender);
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
            recv = socket.ReceiveFrom(recieveData, ref remote);
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
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, remote);

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
        waitingClientThread.Abort();

        TransferInformation();

        //SendStart message
        byte[] sendData = Encoding.ASCII.GetBytes("StartGame");
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, remote);

        //ChangeScene
        ChangeScene();
    }

    void TransferInformation()
    {
        MessageManager.socket = socket;
        MessageManager.remote = remote;
        MessageManager.isServer = true;
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("MainScene");
        MessageManager.StartComunication();
    }

    //Screen
    
    public void GetIP(Text text)
    {
        text.text = GetMyIp();
    }

    public void GetPort(Text text)
    {
        text.text = port.ToString();
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