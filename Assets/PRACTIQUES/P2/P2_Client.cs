using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

public class P2_Client : MonoBehaviour
{
    [SerializeField] bool isUDP = false;
    [SerializeField] string ip = "127.0.0.1";
    Socket socket;

    byte[] data = new byte[1024];
    string stringData;
    int recv;

    IPEndPoint ipep;

    void Start()
    {
        //Create IP info struct
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050); //TODO: Preguntar port

        //Open Socket
        CreateSocket();

        //Recive info in UDP or TCP mode
        RecieveData();
       
    }

    void CreateSocket()
    {
        if (isUDP)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        else
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("___CLIENT___\nSocket CREATED\n");
    }

    void RecieveData()
    {
        if (isUDP)
        {
            stringData = "Hello, IM A CLIENT UDP!";
            data = Encoding.ASCII.GetBytes(stringData);
            socket.SendTo(data, data.Length, SocketFlags.None, ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;

            data = new byte[1024];
            recv = socket.ReceiveFrom(data, ref Remote);

            Debug.Log("___CLIENT___\nMessage received from:" + Remote.ToString() 
                + Encoding.ASCII.GetString(data, 0, recv));

            KillSocket();
        }
        else
        {
            try
            {
                socket.Connect(ipep); //Connect to ip with socket
            }
            catch (SocketException e)
            {
                Debug.Log("___CLIENT___\nConnection FAILED: Unable to connect to server.\nError: " + e.ToString());
                return;
            }

            //Recieve data and read as string
            recv = socket.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("___CLIENT___\nData RECIEVED:\n" + stringData);

            KillSocket();
        }
    }

    void ShutdownSocket()
    {
        //socket.Shutdown(SocketShutdown.Receive);
        //socket.Shutdown(SocketShutdown.Send);
        //socket.Shutdown(SocketShutdown.Both);

        //TODO: Preguntar
    }

    void KillSocket()
    {
        ShutdownSocket();
        socket.Close();

        Debug.Log("___CLIENT___\nSocket KILLED\n");
    }
}
