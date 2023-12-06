using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;
using System;

public class P2_ServerMSG : MonoBehaviour
{
    //
    //  TCP Message Server
    //

    Socket socket;
    Socket client;
    Thread serverThread;

    void Start()
    {
        //Create IP info struct
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

        //Open Socket
        CreateSocket();

        //Bind Socket to network
        socket.Bind(ipep);

        serverThread = new Thread(ServerSearchClient);
        serverThread.Start();
        StartCoroutine(StopSearching());
    }

    IEnumerator StopSearching()
    {
        yield return new WaitForSeconds(15);

        serverThread.Abort();
    }

    void ServerSearchClient()
    {
        //Wait for client
        Debug.Log("___SERVER___\nWaiting for client...\n");
        socket.Listen(10);

        //Bind with client
        try 
        {
            client = socket.Accept();
        }
        catch 
        { 
            Debug.Log("Server stopped listening! "); 
            return;
        }

        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        Debug.Log("___SERVER___\nConnected with " + clientep.Address.ToString() +
            " at port " + clientep.Port + "\n");

        //Message Sender
        /*byte[] data = new byte[1024];
        for (int i = 0; i < 1000; i++)
        {
            string message = "\tRoger Puto (" + i + ")!";
            data = Encoding.ASCII.GetBytes(message);
            client.Send(data, data.Length, SocketFlags.None);
        }

        Close connection
        client.Close();
        KillSocket();*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            byte[] data = new byte[1024];
            string message = "MESSAGE sent at: " + Time.realtimeSinceStartup + " !!!!!!";
            data = Encoding.ASCII.GetBytes(message);
            client.Send(data, data.Length, SocketFlags.None);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            client.Send(Encoding.ASCII.GetBytes("stop"), 4, SocketFlags.None);
            client.Close();
            KillSocket();
        }
    }

    void CreateSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("___SERVER___\nSocket CREATED\n");
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
