using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;
using System;

public class P2_Server : MonoBehaviour
{
    Socket socket;
    Thread serverThread;
    EndPoint Remote;
    [SerializeField] string message = "IM CONNECTED! Bieeeen ._.";

    //SEND info
    byte[] data = new byte[1024];

    int recv;

    void Start()
    {
        //Create IP info struct
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050); //TODO: Preguntar port

        //Open Socket
        CreateSocket();

        //Bind Socket to network
        socket.Bind(ipep);

        //Send info in UDP or TCP mode
        SendData();

    }

    IEnumerator StopListening()
    {
        yield return new WaitForSeconds(60);

        if (serverThread.IsAlive) { 
            serverThread.Interrupt(); //TODO: This causes errors
            KillSocket();

            Debug.Log("___SERVER___\nConnection time EXPIRED!\n");
        }
    }

    public void Connecting()
    {
        recv = socket.ReceiveFrom(data, ref Remote);

        Debug.Log("___SERVER___\nMessage received from:" + Remote.ToString()
            + Encoding.ASCII.GetString(data, 0, recv));

        //entenc que aqui em guardo els inputs del player??


        data = Encoding.ASCII.GetBytes(message);
        socket.SendTo(data, data.Length, SocketFlags.None, Remote);
    }

    void UDPserverThreadStart()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);
    }

    void CreateSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        Debug.Log("___SERVER___\nSocket CREATED\n");
    }

    void SendData()
    {
        serverThread = new Thread(UDPserverThreadStart);
        
        serverThread.Start();
        StartCoroutine(StopListening());
    }

    void KillSocket()
    {
        if (socket.Connected)
        {
            socket.Shutdown(SocketShutdown.Both);
        }

        socket.Close();
        Debug.Log("___SERVER___\nSocket KILLED\n");
    }
}
