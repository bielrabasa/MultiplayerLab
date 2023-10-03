using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;

public class P2_Server : MonoBehaviour
{
    Socket socket;
    Socket client;
    Thread serverThread;

    [SerializeField] string message = "IM CONNECTED! Bieeeen ._.";

    void Start()
    {
        //Create IP info struct
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050); //TODO: Preguntar port

        //Open Socket
        CreateSocket(isUDP: false);

        //Bind Socket to network
        socket.Bind(ipep);


        //Thread Listen
        serverThread = new Thread(serverThreadStart);
        serverThread.Start();
        StartCoroutine(StopListening());
    }

    IEnumerator StopListening()
    {
        yield return new WaitForSeconds(5);

        if (serverThread.IsAlive) { 
            serverThread.Interrupt();
            KillSocket();

            Debug.Log("Connection time EXPIRED!");
        }
    }

    void serverThreadStart()
    {
        //Wait for client
        Debug.Log("Waiting for client...");
        socket.Listen(10); //TODO: Should be a thread

        //Bind with client
        client = socket.Accept();
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        Debug.Log("Connected with " + clientep.Address.ToString() +
            " at port " + clientep.Port);

        //SEND info
        byte[] data = new byte[1024];
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
