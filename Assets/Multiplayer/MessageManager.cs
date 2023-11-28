using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageTypes;
using System;

public class MessageManager : MonoBehaviour
{
    //Sent messages that haven't recieved acknowledgements for
    static List<Message> sentMessages;

    //Other PCs recieved messages
    static List<long> acks;

    //All objects that need to be sent messages
    public static Dictionary<MessageType, Action<Message>> messageDistribute = new();

    //Current Acknowledgement number
    static uint currentID = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        for(int i = 0; i < (int)MessageType._MESSAGE_TYPE_COUNT; i++)
        {
            messageDistribute.Add((MessageType)i, null);
        }
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

        sentMessages.Add(message);

        //TODO: Send message to other player
    }

    static void OnRecievedMessage(Message message)
    {
        //TODO: search in sent messages and delete it

        //Send message to subscribed objects
        messageDistribute[message.type]?.Invoke(message);
    }
}
