using MessageTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAcks : MonoBehaviour
{
    private void Start()
    {
        MessageManager.messageDistribute[MessageType.KILL] += OnKill;
        MessageManager.messageDistribute[MessageType.CHAT] += UpdateChat;

        MessageManager.SendMessage(new Chat("Hola Jefe!"));
    }

    void OnKill(Message m)
    {
        Debug.Log("I've been killed!! - " + m.time);
    }

    private void UpdateChat(Message m)
    {
        Chat c = m as Chat;

        Debug.Log("Chat msg: " + c.chatMsg);
    }

    private void OnDestroy()
    {
        MessageManager.messageDistribute[MessageType.KILL] -= OnKill;
    }
}
