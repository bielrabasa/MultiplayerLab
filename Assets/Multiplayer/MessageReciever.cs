using MessageTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MessageReciever
{
    //public abstract List<MessageType> recievingMessageTypes;
    public abstract void OnMessageRecieve(Message msg);

    //TODO: Look how interfaces function
}
