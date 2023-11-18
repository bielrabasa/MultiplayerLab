using System.Text;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

public enum MultiplayerEvents
{
    SHOOT,
    KILL,
    DISCONNECT,
    PAUSE,
    UNPAUSE,
    NUMEVENTS
}
public struct PlayerState
{
    public float time;
    public Vector3 pos;
    public float topRot;
    public float botRot;
    public List<MultiplayerEvents> events;
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

    //EVENTS
    [HideInInspector] public List<MultiplayerEvents> events;

    //State Game
    public bool isGamePaused = false;
    PostProcessVolume postpo;

    void Start()
    {
        stopConnection = false;
        hasUpdated = false;
        recievingMessages = new Thread(RecieveOtherState);

        postpo = FindAnyObjectByType<PostProcessVolume>();
        postpo.gameObject.SetActive(false);

        multiplayerState = FindObjectOfType<MultiplayerState>();

        if (multiplayerState == null)
        {
            Debug.Log("Players not connected: game running in test mode.");
            return;
        }

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

        if(Input.GetKeyDown(KeyCode.P))
        {
            SendPauseGame(!isGamePaused);
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

        foreach (MultiplayerEvents e in otherState.events)
        {
            switch (e)
            {
                case MultiplayerEvents.SHOOT:
                    //Instanciate Bullet
                    otherPlayer.GetComponent<TankScript>().Shoot();
                    break;
                case MultiplayerEvents.KILL:
                    //Die
                    myPlayer.gameObject.SetActive(false);
                    break;

                case MultiplayerEvents.DISCONNECT:

                    break;

                case MultiplayerEvents.PAUSE:
                    SetPause(true);
                    break;

                case MultiplayerEvents.UNPAUSE:
                    SetPause(false);
                    break;
            }
        }
    }

    PlayerState GetMyState()
    {
        PlayerState myState = new PlayerState();

        myState.time = Time.time;
        myState.pos = myPlayer.position;
        myState.topRot = myPlayer.GetChild(0).rotation.eulerAngles.z;
        myState.botRot = myPlayer.GetChild(1).rotation.eulerAngles.z;
        myState.events = events;

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

            multiplayerState.socket.SendTo(messageData, messageData.Length, SocketFlags.None, multiplayerState.remote);

            //After sending, clear event list
            events.Clear();
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

            int size = multiplayerState.socket.ReceiveFrom(data, ref multiplayerState.remote);

            PlayerState message = FromBytes(data, size);

            if (message.time > otherState.time) 
            {
                otherState = message;
                hasUpdated = true;
            }
            else if (message.events.Count > 0)
            {
                otherState.events = message.events;
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


    //
    //  INGAME FUNCTIONALITIES
    //
    public void SendEvent(MultiplayerEvents e, Transform t = null)
    {
        //Only check death on other tank
        if(e == MultiplayerEvents.KILL)
        {
            if (t == null || t == myPlayer) return;

            t.gameObject.SetActive(false);
        }

        events.Add(e);
    }

    public void SendPauseGame(bool pause)
    {
        SetPause(pause);

        SendEvent(pause ? MultiplayerEvents.PAUSE : MultiplayerEvents.UNPAUSE);
    }

    void SetPause(bool pause)
    {
        postpo.gameObject.SetActive(pause);
        isGamePaused = pause;
    }

    public void ResetGame()
    {
        
    }
}
