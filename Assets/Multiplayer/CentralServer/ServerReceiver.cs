using JetBrains.Annotations;
using MessageTypes;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.tvOS;

public class ServerReceiver : MonoBehaviour
{
    [HideInInspector] public Socket socket;
    [HideInInspector] public EndPoint remote;

    Thread receiver;

    // Start is called before the first frame update
    void Start()
    {
        receiver = new Thread(MessageReceiver);
        receiver.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MessageReceiver()
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

            Message m = Serializer.FromBytes(data, size);

            Debug.Log(m.type);

            //Add to process later
            /*lock (recievedMessages)
            {
                recievedMessages.Add(m);
            }

            lock (acks)
            {
                acks.Add(m.id);
            }*/
        }
    }
}
