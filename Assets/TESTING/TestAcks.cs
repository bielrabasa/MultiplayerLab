using MessageTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAcks : MonoBehaviour
{
    private void Awake()
    {
        /*MessageManager.messageDistribute[MessageType.KILL] += OnKill;
        MessageManager.messageDistribute[MessageType.CHAT] += OnKill;
        MessageManager.messageDistribute[MessageType.ACKNOWLEDGEMENTS] += OnKill;
        MessageManager.messageDistribute[MessageType.CONFIRMATION] += OnKill;

        MessageManager.SendMessage(new Chat("Hola Jefe!"));*/
    }

    void OnKill(Message m)
    {
        Debug.Log("I've been killed!! - " + m.type);
    }

    private void OnDestroy()
    {
        MessageManager.messageDistribute[MessageType.KILL] -= OnKill;
    }
}
