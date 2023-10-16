using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;
using System.Threading;
using UnityEngine.UI;

public class P2_Client : MonoBehaviour
{
    [SerializeField] string ip = "127.0.0.1";
    Socket socket;
    EndPoint Remote;

    byte[] data = new byte[1024];
    string stringData;
    int recv;

    IPEndPoint ipep;

    P2_Server server;

    [SerializeField] GameObject indicator;

    void Start()
    {
        //Create IP info struct
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050); //TODO: Preguntar port

        //Open Socket
        CreateSocket();

        //Recive info in UDP or TCP mode
        RecieveData();

        server = GameObject.FindAnyObjectByType<P2_Server>();
        server.Connecting();

        Thread messageReciever = new Thread(MessageReciever);
        messageReciever.Start();

        if(socket.Connected)
        {
            ChangeColor();
        }
    }

    void CreateSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        Debug.Log("___CLIENT___\nSocket CREATED\n");
    }

    void MessageReciever()
    {
        data = new byte[1024];
        recv = socket.ReceiveFrom(data, ref Remote);

        Debug.Log("___CLIENT___\nMessage received from player " +": " + Remote.ToString()
            + Encoding.ASCII.GetString(data, 0, recv));

        KillSocket();
    }

    void RecieveData()
    {
        stringData = "Hello, IM A CLIENT UDP!";
        data = Encoding.ASCII.GetBytes(stringData);
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)sender;
    }

    void KillSocket()
    {
        socket.Close();

        Debug.Log("___CLIENT___\nSocket KILLED\n");
    }

    void ChangeColor()
    {
        indicator.GetComponent<Image>().color = new Color(0, 255, 0);
    }
}