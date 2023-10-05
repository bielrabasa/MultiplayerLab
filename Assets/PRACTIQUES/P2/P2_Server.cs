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
    Socket client;
    Thread serverThread;

    [SerializeField] string message = "IM CONNECTED! Bieeeen ._.";

    //SEND info
    byte[] data = new byte[1024];

    int recv;

    void Start()
    {
        //Create IP info struct
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050); //TODO: Preguntar port

        //Open Socket
        CreateSocket(isUDP: false);

        //Bind Socket to network
        socket.Bind(ipep);

        //Send info in UDP or TCP mode
        SendData(isUDP: false);

    }

    IEnumerator StopListening()
    {
        yield return new WaitForSeconds(15);

        if (serverThread.IsAlive) { 
            serverThread.Interrupt(); //TODO: This causes errors
            KillSocket();

            Debug.Log("Connection time EXPIRED!");
        }
    }

    void serverThreadStart()
    {
        //Wait for client
        Debug.Log("Waiting for client...");
        socket.Listen(10);

        //Bind with client
        client = socket.Accept(); //TODO: Peta aquí al interrumpir el thread
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        Debug.Log("Connected with " + clientep.Address.ToString() +
            " at port " + clientep.Port);

        
        data = Encoding.ASCII.GetBytes(message);
        client.Send(data, data.Length, SocketFlags.None);
        Debug.Log("Data sent!");

        //Close connection
        client.Close();
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

    void SendData(bool isUDP)
    {
        if (isUDP)
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);

            recv = socket.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            data = Encoding.ASCII.GetBytes(message);
            socket.SendTo(data, data.Length, SocketFlags.None, Remote);
        }
        else
        {
            //Thread Listen
            serverThread = new Thread(serverThreadStart);
            serverThread.Start();
            StartCoroutine(StopListening());
        }
    }

    void KillSocket()
    {
        if (socket.Connected)
        {
            socket.Shutdown(SocketShutdown.Both);
        }

        socket.Close();
        Debug.Log("Socket KILLED");
    }
}
