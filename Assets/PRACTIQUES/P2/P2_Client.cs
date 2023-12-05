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

    bool actuConsole = false;
    string axuConsole;
    string myName;
    fool consoleUI;

    bool colorConnected;

    [SerializeField] GameObject indicator;

    public void ConnectMe()
    {
        myName = this.name;
        consoleUI = GameObject.FindAnyObjectByType<fool>();

        //Create IP info struct
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050);

        //Open Socket
        CreateSocket();

        //Recive info in UDP
        RecieveData();

        server = GameObject.FindAnyObjectByType<P2_Server>();
        server.Connecting();

        Thread messageReciever = new Thread(MessageReciever);
        messageReciever.Start();

        if(socket.Connected)
        {
            colorConnected = true;
            ChangeColor();
        }
    }

    private void Update()
    {
        if(actuConsole) { consoleUI.ConsoleLogs(axuConsole); actuConsole = false; }
    }

    void CreateSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        Debug.Log("___CLIENT___\nSocket CREATED\n");
    }

    void RecieveData()
    {
        stringData = "Hello, IM A CLIENT UDP!";
        data = Encoding.ASCII.GetBytes(stringData);
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)sender;
    }

    void MessageReciever()
    {
        string info;
        do
        {
            data = new byte[1024];
            recv = socket.ReceiveFrom(data, ref Remote);

            info = Encoding.ASCII.GetString(data, 0, recv);

            axuConsole = ("___CLIENT___\nMessage received from " + myName + ": " + Remote.ToString()
                + Encoding.ASCII.GetString(data, 0, recv));

            actuConsole = true;
        }
        while (info != "stop");

        KillSocket();
    }

    void KillSocket()
    {
        socket.Close();

        Debug.Log("___CLIENT___\nSocket KILLED\n");
    }

    void ChangeColor()
    {
        if(colorConnected)
        {
            indicator.GetComponent<Image>().color = new Color(0, 255, 0);
        }
        else 
        { 
            indicator.GetComponent<Image>().color = new Color(255, 0, 0);
        }
    }

    public void DisconnetMe()
    {
        stringData = "stop";
        data = Encoding.ASCII.GetBytes(stringData);
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);
        colorConnected = false;
        ChangeColor();
    }
}