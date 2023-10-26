using System.Text;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UIElements;
using UnityEditor;

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

    //Real ingame objects
    Transform otherPlayer;
    Transform myPlayer;
    //Server -> Blue
    //Client -> Red

    //Multiplayer
    Socket mpSocket;
    EndPoint mpRemote;
    bool isServer; //TODO: Needs to be set from outside

    //State Update
    bool hasUpdated;
    PlayerState otherState;

    //Recieve messages
    bool stopConnection;
    Thread recievingMessages; //TODO: Activate Thread on connection

    void Start()
    {
        stopConnection = false;
        hasUpdated = false;
        recievingMessages = new Thread(RecieveOtherState);

        //TODO: ERASE, this is a test
        isServer = false;
        GetPlayers();
        Debug.Log("JSON: \n" + JsonUtility.ToJson(GetMyState()));
        //
    }

    void Update()
    {
        //TODO: TEST, erase this
        if(Input.GetKeyDown(KeyCode.Space))
        {
            otherState = GetMyState();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateOtherState();
        }
        //
        
        if (hasUpdated)
        {
            UpdateOtherState();
            hasUpdated = false;
        }
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
                if (isServer)
                {
                    myPlayer = t.gameObject.transform;
                    Debug.Log("Server Set");
                }
                else
                {
                    otherPlayer = t.gameObject.transform;
                    t.BlockMovement();
                }
            }
            else if(t.gameObject.name == RED_TANK_NAME)
            {
                if (!isServer)
                {
                    myPlayer = t.gameObject.transform;
                    Debug.Log("Client Set");
                }
                else
                {
                    otherPlayer = t.gameObject.transform;
                    t.BlockMovement();
                }
            }
        }
    }

    void SendMyState()
    {
        byte[] messageData = ToBytes(GetMyState());

        //TODO: Send message correctly
        mpSocket.SendTo(messageData, messageData.Length, SocketFlags.None, mpRemote);
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
            int size = mpSocket.ReceiveFrom(data, ref mpRemote);

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
