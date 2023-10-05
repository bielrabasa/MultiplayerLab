using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

public class P2_Client : MonoBehaviour
{
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
        CreateSocket(isUDP: true);

        //Recive info in UDP or TCP mode
        RecieveData(isUDP: true);
       
    }

    void CreateSocket(bool isUDP)
    {
        if (isUDP)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        else
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Socket CREATED");
    }

    void RecieveData(bool isUDP)
    {
        if (isUDP)
        {
            stringData = "Hello, thats UDP";
            data = Encoding.ASCII.GetBytes(stringData);
            socket.SendTo(data, data.Length, SocketFlags.None, ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;

            data = new byte[1024];
            recv = socket.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            Debug.Log("Socket CLOSE");
            socket.Close();
        }
        else
        {
            try
            {
                socket.Connect(ipep); //Connect to ip with socket
            }
            catch (SocketException e)
            {
                Debug.Log("Connection FAILED: Unable to connect to server.\nError: " + e.ToString());
                return;
            }

            //Recieve data and read as string
            recv = socket.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Data RECIEVED:\n" + stringData);

            KillSocket();
        }
    }

    void ShutdownSocket()
    {
        //socket.Shutdown(SocketShutdown.Receive);
        //socket.Shutdown(SocketShutdown.Send);
        socket.Shutdown(SocketShutdown.Both);

        //TODO: Preguntar
    }

    void KillSocket()
    {
        ShutdownSocket();
        socket.Close();

        Debug.Log("Socket KILLED");
    }
}
