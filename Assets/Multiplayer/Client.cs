using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    string serverIP = "127.0.0.1";

    Socket socket;
    EndPoint Remote;

    IPEndPoint ipep;

    byte[] data = new byte[1024];
    string stringData;
    int recv;

    public void SetIP()
    {
        InputField ipInput = FindObjectOfType<InputField>(); 

        string ip = ipInput.text.ToString();

        serverIP = ip;
        StartConnection(ip);
    }

    void StartConnection(string ip)
    {
        //Create IP info struct
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050); //TODO: Preguntar port

        //Open Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //Recive info in UDP
        RecieveData();

        Thread messageReciever = new Thread(MessageReciever);
        messageReciever.Start();
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
        }
        while (info != "stop");

        KillSocket();
    }

    void KillSocket()
    {
        socket.Close();

        Debug.Log("___CLIENT___\nSocket KILLED\n");
    }

    /*public void DisconnetMe()
    {
        stringData = "stop";
        data = Encoding.ASCII.GetBytes(stringData);
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);
    }*/
}
