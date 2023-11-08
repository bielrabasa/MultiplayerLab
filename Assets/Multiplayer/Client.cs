using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    Thread messageReciever;
    Thread waitForStart;

    string serverIP = "127.0.0.1";

    Socket socket;
    EndPoint remote;
    IPEndPoint ipep;

    bool startGame;
    bool startConnection;


    private void Start()
    {
        startGame = false;
        startConnection = false;
        messageReciever = new Thread(ConnectToServer);
        waitForStart = new Thread(WaitForStart);

        StartCoroutine(FillInputField());
    }

    private void Update()
    {
        if (startConnection)
        {
            FullyConnected();
            startConnection = false;
        }

        if(startGame)
        {
            ChangeScene();
            startGame = false;
        }
    }

    IEnumerator FillInputField()
    {
        //Get IP without last digits
        string myIp = GetMyIp();
        int i = myIp.LastIndexOf('.');
        myIp = myIp.Substring(0, i + 1);

        //Fill input
        InputField ipInput = FindObjectOfType<InputField>();
        ipInput.text = myIp;

        ipInput.Select();
        yield return new WaitForEndOfFrame();
        //Needs to wait to set cursor
        ipInput.caretPosition = ipInput.text.Length;
        ipInput.ForceLabelUpdate();
    }

    public void SetIP()
    {
        InputField ipInput = FindObjectOfType<InputField>();
        string ip = ipInput.text.ToString();
        serverIP = ip;

        StartConnection();
    }

    void StartConnection()
    {
        ClientSetup();
        messageReciever.Start();
    }

    void ClientSetup()
    {
        ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9889);

        //Open Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //Set port 0 to send messages
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)sender;
    }

    void SendConfirmation()
    {
        byte[] data =  Encoding.ASCII.GetBytes("ClientConnected");
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);
    }

    //THREAD
    void ConnectToServer()
    {
        SendConfirmation();

        byte[] data = new byte[1024];
        int recv;

        try
        {
            recv = socket.ReceiveFrom(data, ref remote);
        }
        catch
        {
            Debug.Log("Client stopped listening!");
            return;
        }

        string message = Encoding.ASCII.GetString(data, 0, recv);

        if(message == "ServerConnected")
        {
            startConnection = true;
        }
        else
        {
            Debug.Log("Incorrect confirmation message: " + message);
        }
    }

    void FullyConnected()
    {
        TransferInformation();
        waitForStart.Start();
    }

    void TransferInformation()
    {
        MultiplayerState ms = FindObjectOfType<MultiplayerState>();
        ms.socket = socket;
        ms.remote = remote;
        ms.isServer = false;
    }

    void WaitForStart() //TODO: function to abort
    {
        Debug.Log("Waiting for the Server to Start...");

        byte[] data = new byte[1024];
        int recv;

        try
        {
            recv = socket.ReceiveFrom(data, ref remote);
        }
        catch
        {
            Debug.Log("Client did not want to wait for Start!");
            return;
        }

        string message = Encoding.ASCII.GetString(data, 0, recv);

        //ChangeScene
        if (message == "StartGame")
        {
            startGame = true;
        }
        else
        {
            Debug.Log("Message recieved to start game is INCORRECT: " + message);
        }
    }
    void ChangeScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    string GetMyIp()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "";
    }
}
