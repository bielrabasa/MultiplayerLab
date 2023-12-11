using MessageTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAcks : MonoBehaviour
{
    private void Start()
    {
        if (MessageManager.messageDistribute.Count == 0) return;
        MessageManager.messageDistribute[MessageType.KILL] += OnKill;
        MessageManager.messageDistribute[MessageType.CHAT] += UpdateChat;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MessageManager.SendMessage(new Chat("Hola Jefe! Soc el: " + (MessageManager.isServer?"Server":"Client")));
        }

        if(Input.GetKeyDown(KeyCode.K)) 
        {
            MessageManager.SendMessage(new Message(MessageType.KILL));
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            GameObject testingID = GameObject.Find("GameObjectID");
            int id = testingID.GetInstanceID();
            Object[] allObjects = GameObject.FindGameObjectsWithTag("OBSTACLE");

            foreach(GameObject go in allObjects)
            {
                Debug.Log(go.GetInstanceID());

                if (id == go.GetInstanceID())
                {
                    Debug.Log(go.name);
                }
            }
        }
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
        if (MessageManager.messageDistribute.Count == 0) return;
        MessageManager.messageDistribute[MessageType.KILL] -= OnKill;
    }
}