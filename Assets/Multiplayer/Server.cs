using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

        //Create Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //Bind Socket to ONLY recieve info from the 9050 port
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
            Debug.Log("Incorrect confirmation message!");
            return;
        }

        string stringData = "ServerConnected";
        byte[] sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(stringData);
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

        Debug.Log("PLAYING!!");
        //TODO: Transfer info
        //TODO: Send message

        //ChangeScene
        ChangeScene();
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
