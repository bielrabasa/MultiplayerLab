using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;
using System;

public class P2_ClientMSG : MonoBehaviour
{
    [SerializeField] string ip = "127.0.0.1";
    Socket socket;


    void Start()
    {
        //Create IP info struct
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), 9050); //TODO: Preguntar port

        //Open Socket
        CreateSocket();

        try
        {
            socket.Connect(ipep); //Connect to ip with socket
        }
        catch (SocketException e)
        {
            Debug.Log("___CLIENT___\nConnection FAILED: Unable to connect to server.\nError: " + e.ToString());
            return;
        }

        //Message Reciever
        byte[] data = new byte[1024];

        for (int i = 0; i < 1000; i++)
        {
            int size = socket.Receive(data);
            Debug.Log("___CLIENT___\nMessage RECIEVED:\n" + Encoding.ASCII.GetString(data, 0, size));
        }

        KillSocket();
    }

    void CreateSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("___CLIENT___\nSocket CREATED\n");
    }

    void KillSocket()
    {
        if (socket.Connected)
        {
            socket.Shutdown(SocketShutdown.Both);
        }

        socket.Close();
        Debug.Log("___CLIENT___\nSocket KILLED\n");
    }
}
