using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageTypes;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class MessageManager : MonoBehaviour
{
    //Sent messages that haven't recieved acknowledgements for
    static List<Message> sentMessages = new();
    static List<Message> recievedMessages = new();

    //Other PCs recieved messages
    static List<uint> acks = new();

    //All objects that need to be sent messages
    [HideInInspector] public static Dictionary<MessageType, Action<Message>> messageDistribute = new();

    //Current Acknowledgement number
    static uint currentID = 0;

    //Network
    [HideInInspector] public static Socket socket;
    [HideInInspector] public static EndPoint remote;
    public static bool isServer = true;
    static Thread messageReciever = new(MessageReciever);



    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if(messageDistribute.Count == 0) 
        {
            for (int i = 0; i < (int)MessageType._MESSAGE_TYPE_COUNT; i++)
            {
                messageDistribute.Add((MessageType)i, null);
            }
        }
    }

    private void Start()
    {
        //Subscribe to acknowledgement messages
        messageDistribute[MessageType.ACKNOWLEDGEMENTS] += OnAcknowledgementsRecieved;
    }

    private void Update()
    {
        //Process recieved messages
        foreach(Message m in recievedMessages) OnRecievedMessage(m);
        recievedMessages.Clear();
    }

    static uint NextID()
    {
        return ++currentID;
    }

    //Message type & other data has to be set before
    public static void SendMessage(Message message)
    {
        message.time = Time.time;
        message.id = NextID();

        //sentMessages.Add(message);

        //Send message
        byte[] messageData = ToBytes(message);
        socket.SendTo(messageData, messageData.Length, SocketFlags.None, remote);
    }

    public static void SendMessage(MessageType type)
    {
        Message message = new Message(type);
        message.time = Time.time;
        message.id = NextID();

        //sentMessages.Add(message);

        //Send message
        byte[] messageData = ToBytes(message);
        socket.SendTo(messageData, messageData.Length, SocketFlags.None, remote);
    }

    static void OnRecievedMessage(Message message)
    {
        //Send message to subscribed objects
        messageDistribute[message.type]?.Invoke(message);
    }

    static void OnAcknowledgementsRecieved(Message m)
    {
        /*Acknowledgements a = m as Acknowledgements;

        //Remove the Acknowledged messages
        for (int i = sentMessages.Count - 1; i >= 0; i--)
        {
            if (a.acks.Contains(sentMessages[i].id)) sentMessages.Remove(sentMessages[i]);
        }*/
    }

    static void StopComunication()
    {
        messageReciever.Abort();
        //TODO: stop sending akcs
    }

    public static void StartComunication()
    {
        messageReciever.Start();

        //TODO: Start sending acknowledgements

    }

    static void MessageReciever()
    {
        while (true)
        {
            byte[] data = new byte[1024];
            int size;

            try
            {
                size = socket.ReceiveFrom(data, ref remote);
            }
            catch
            {
                Debug.Log("Stopped recieving messages.");
                return;
            }

            //Add to process later
            recievedMessages.Add(FromBytes(data, size));
            Debug.Log("Manager Recieved a message!");
        }
    }

    //From message to Json string
    static string ToJson(Message m)
    {
        return JsonUtility.ToJson(m);
    }

    //From message to Bytes
    static byte[] ToBytes(Message m)
    {
        return Encoding.ASCII.GetBytes(ToJson(m));
    }

    //From bytes to message
    static Message FromBytes(byte[] data, int size)
    {
        return FromJson(Encoding.ASCII.GetString(data, 0, size));
    }

    //From Json string to message
    static Message FromJson(string json)
    {
        Message m = JsonUtility.FromJson<Message>(json);

        //Check to reDeserialize in case of inherited class
        switch (m.type)
        {
            case MessageType.ACKNOWLEDGEMENTS:
                {
                    m = JsonUtility.FromJson<Acknowledgements>(json);
                    break;
                }
            case MessageType.POSITION:
                {
                    m = JsonUtility.FromJson<Position>(json);
                    break;
                }
            case MessageType.SHOOT:
                {
                    m = JsonUtility.FromJson<Shoot>(json);
                    break;
                }
            case MessageType.CHAT:
                {
                    m = JsonUtility.FromJson<Chat>(json);
                    break;
                }
        }

        return m;
    }
}
