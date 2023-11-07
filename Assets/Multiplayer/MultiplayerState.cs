using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MultiplayerState : MonoBehaviour
{
    [HideInInspector] public Socket socket;
    [HideInInspector] public EndPoint remote;

    [HideInInspector] public List<string> ipList;

    public bool isServer = true;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
