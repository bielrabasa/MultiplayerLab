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

        TransferInformation();

        //SendStart message
        string stringData = "StartGame";
        byte[] sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(stringData);
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
        // Getting Ip address of local machine...
        string strHostName = string.Empty;

        // First get the host name of local machine.
        strHostName = Dns.GetHostName();

        // Then using host name, get the IP address list..
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;

        for (int i = 0; i < addr.Length; i++)
        {
            text.text = addr[i].ToString();
        }

        MultiplayerState ms = FindObjectOfType<MultiplayerState>();
        ms.ipList.Add(text.text);

    }
}
