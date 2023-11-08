using System.Text;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public struct PlayerState
{
    public float time;
    public Vector3 pos;
    public float topRot;
    public float botRot;
    public bool shotBullet;    
}

public class GameState : MonoBehaviour
{
    const int MESSAGE_PACK_SIZE = 1024;
    const string BLUE_TANK_NAME = "Tank (blue)";
    const string RED_TANK_NAME = "Tank (red)";
    [SerializeField] float MESSAGE_SEND_DELAY = 1.0f;

    //Real ingame objects
    Transform otherPlayer;
    Transform myPlayer;
    //Server -> Blue
    //Client -> Red

    //Multiplayer
    MultiplayerState multiplayerState;

    //State Update
    bool hasUpdated;
    PlayerState otherState;

    //Recieve messages
    bool stopConnection;
    Thread recievingMessages;

    void Start()
    {
        stopConnection = false;
        hasUpdated = false;
        recievingMessages = new Thread(RecieveOtherState);

        multiplayerState = FindObjectOfType<MultiplayerState>();

        Debug.Log("Is Server? " + multiplayerState.isServer);

        GetPlayers();

        StartDataTransfer();
    }

    void Update()
    {
        if (hasUpdated)
        {
            UpdateOtherState();
            hasUpdated = false;
        }
    }

    void StartDataTransfer()
    {
        recievingMessages.Start();
        StartCoroutine(SendMyState());
    }

    void UpdateOtherState()
    {
        otherPlayer.position = otherState.pos;
        otherPlayer.GetChild(0).rotation = Quaternion.Euler(0, 0, otherState.topRot);
        otherPlayer.GetChild(1).rotation = Quaternion.Euler(0, 0, otherState.botRot);

        if(otherState.shotBullet)
        {
            //TODO: Instanciate bullet from data
        }
    }

    PlayerState GetMyState()
    {
        PlayerState myState = new PlayerState();

        myState.time = Time.time;
        myState.pos = myPlayer.position;
        myState.topRot = myPlayer.GetChild(0).rotation.eulerAngles.z;
        myState.botRot = myPlayer.GetChild(1).rotation.eulerAngles.z;
        myState.shotBullet = false; //TODO: Check if bullet has been shot

        return myState;
    }

    void GetPlayers()
    {
        TankScript[] ts = FindObjectsOfType<TankScript>();
        foreach (TankScript t in ts)
        {
            if (t.gameObject.name == BLUE_TANK_NAME)
            {
                if (multiplayerState.isServer)
                {
                    myPlayer = t.gameObject.transform;
                }
                else
                {
                    otherPlayer = t.gameObject.transform;
                    t.BlockMovement();
                }
            }
            else if(t.gameObject.name == RED_TANK_NAME)
            {
                if (!multiplayerState.isServer)
                {
                    myPlayer = t.gameObject.transform;
                }
                else
                {
                    otherPlayer = t.gameObject.transform;
                    t.BlockMovement();
                }
            }
        }
    }

    IEnumerator SendMyState()
    {
        while (!stopConnection)
        {
            yield return new WaitForSecondsRealtime(MESSAGE_SEND_DELAY);

            byte[] messageData = ToBytes(GetMyState());

            //TODO: Send message correctly
            multiplayerState.socket.SendTo(messageData, messageData.Length, SocketFlags.None, multiplayerState.remote);
        }
    }

    //
    //  THREAD
    //
    void RecieveOtherState()
    {
        while (!stopConnection)
        {
            byte[] data = new byte[MESSAGE_PACK_SIZE];
            //TODO: Recieve message correctly
            int size = multiplayerState.socket.ReceiveFrom(data, ref multiplayerState.remote);

            PlayerState message = FromBytes(data, size);

            if (message.time > otherState.time)
            {
                //PROBLEMS? Maybe needs (lock)
                otherState = message;
                hasUpdated = true;
            }
        }
    }
    

    //
    //  TOOLS
    //
    byte[] ToBytes(PlayerState ps) //ps -> json -> bytes
    {
        string json = JsonUtility.ToJson(ps); 
        return Encoding.ASCII.GetBytes(json);
    }

    PlayerState FromBytes(byte[] data, int size) //bytes -> json -> ps
    {
        string json = Encoding.ASCII.GetString(data, 0, size);
        return JsonUtility.FromJson<PlayerState>(json);
    }
}
