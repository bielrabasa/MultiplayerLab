using System.Text;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
using System.Net;

public struct PlayerState
{
    public float time;
    public Transform transform;
    public bool shotBullet;    
}

public class GameState : MonoBehaviour
{
    const int MESSAGE_PACK_SIZE = 1024;

    //Real ingame objects
    [SerializeField] Transform otherPlayer;
    [SerializeField] Transform myPlayer;

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
    }

    void Update()
    {
        if (hasUpdated)
        {
            UpdateOtherState();
            hasUpdated = false;
        }
    }

    void UpdateOtherState()
    {
        //TODO: Update all scene from the otherState struct
    }

    void SendMyState()
    {
        PlayerState myState = new PlayerState();
        myState.time = Time.time;
        myState.transform = myPlayer;
        myState.shotBullet = false; //TODO: Check if bullet has been shot

        byte[] messageData = ToBytes(myState);

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
