using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    string serverIP = "127.0.0.1";

    public void SetIP(string ip)
    {
        serverIP = ip;
        StartConnection();
    }

    void StartConnection()
    {

    }
}
