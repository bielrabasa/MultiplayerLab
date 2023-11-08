using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    Thread waitingClientThread;
    bool connected;

    Socket socket;
    EndPoint remote;


    void Start()
    {
        waitingClientThread = new Thread(WaitClient);
        connected = false;

        ServerSetup();
        waitingClientThread.Start();
    }

    void ServerSetup()
    {
        //Create IP info struct
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9889);

        //Create Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //Bind Socket to ONLY recieve info from the 9889 port
        socket.Bind(ipep);

        //Set port 0 to send the messages
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)(sender);
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
            return;
        }

        //Recieved message
        string message = Encoding.ASCII.GetString(recieveData, 0, recv);

        //Incorrect message
        if(message != "ClientConnected")
        {
            Debug.Log("Incorrect confirmation message: " + message);
            return;
        }

        //Send Confirmation Message
        byte[] sendData = Encoding.ASCII.GetBytes("ServerConnected");
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, remote);

        connected = true;
    }

    public void StopSearching()
    {
        waitingClientThread.Abort();
    }

    //GAME
    public void StartPlaying()
    {
        if (!connected) return;

        TransferInformation();

        //SendStart message
        byte[] sendData = Encoding.ASCII.GetBytes("StartGame");
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, remote);

        //ChangeScene
        ChangeScene();
    }

    void TransferInformation()
    {
        MultiplayerState ms = FindObjectOfType<MultiplayerState>();
        ms.socket = socket;
        ms.remote = remote;
        ms.isServer = true;
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GetIP(Text text)
    {
        text.text = GetMyIp();
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
