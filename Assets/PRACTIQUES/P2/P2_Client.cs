using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class P2_Client : MonoBehaviour
{
    [SerializeField] string ip = "127.0.0.1";
    Socket socket;

    byte[] data = new byte[1024];
    string stringData;
    int recv;

    void Start()
    {
        //Create IP info struct
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), 9050); //TODO: Preguntar port

        //Open Socket
        CreateSocket(isUDP: false);

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

    void CreateSocket(bool isUDP)
    {
        if (isUDP)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        else
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Socket CREATED");
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
