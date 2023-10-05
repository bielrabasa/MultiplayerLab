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
    bool isUDP = false;
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
        CreateSocket();

        //Bind Socket to network
        socket.Bind(ipep);

        //Send info in UDP or TCP mode
        SendData();

    }

    IEnumerator StopListening()
    {
        yield return new WaitForSeconds(15);

        if (serverThread.IsAlive) { 
            serverThread.Interrupt(); //TODO: This causes errors
            KillSocket();

            Debug.Log("___SERVER___\nConnection time EXPIRED!\n");
        }
    }

    void serverThreadStart()
    {
        //Wait for client
        Debug.Log("___SERVER___\nWaiting for client...\n");
        socket.Listen(10);

        //Bind with client
        client = socket.Accept(); //TODO: Peta aquí al interrumpir el thread
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        Debug.Log("___SERVER___\nConnected with " + clientep.Address.ToString() +
            " at port " + clientep.Port + "\n");
        
        data = Encoding.ASCII.GetBytes(message);
        client.Send(data, data.Length, SocketFlags.None);
        Debug.Log("___SERVER___\nData sent!\n");

        //Close connection
        client.Close();
        KillSocket();
    }

    void CreateSocket()
    {
        if (isUDP)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        else
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("___SERVER___\nSocket CREATED\n");
    }

    void SendData()
    {
        if (isUDP)
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);

            recv = socket.ReceiveFrom(data, ref Remote);
            
            Debug.Log("___SERVER___\nMessage received from:" + Remote.ToString());
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
        Debug.Log("___SERVER___\nSocket KILLED\n");
    }
}
