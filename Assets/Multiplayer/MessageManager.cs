using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageTypes;

public class MessageManager : MonoBehaviour
{
    //Sent messages that haven't recieved acknowledgements for
    List<Message> sentMessages;

    //Other PCs recieved messages
    List<long> acks;

    long CurrentID;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        CurrentID = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    long NextID()
    {
        return CurrentID++;
    }

    //Message type & other data has to be set before
    public void SendMessage(Message message)
    {
        message.time = Time.time;
        message.id = NextID();

        sentMessages.Add(message);

        //TODO: Send message to other player
    }

    void OnRecievedMessage(Message message)
    {
        //TODO: search in sent messages and delete it
        
        switch (message.type)
        {
            case MessageType.ACKNOWLEDGEMENTS:
                //TODO: do smth
                break;
            case MessageType.CONFIRMATION:
                //TODO: do smth
                break;
            case MessageType.START:
                //TODO: do smth
                break;
            case MessageType.POSITION:
                //TODO: do smth
                break;
            case MessageType.SHOOT:
                //TODO: do smth
                break;
            case MessageType.PAUSE:
                //TODO: do smth
                break;
            case MessageType.UNPAUSE:
                //TODO: do smth
                break;
            case MessageType.DISCONNECT:
                //TODO: do smth
                break;
            case MessageType.CHAT:
                //TODO: do smth
                break;
        }
    }
}
