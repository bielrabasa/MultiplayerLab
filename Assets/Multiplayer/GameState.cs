using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using Unity.Collections;
using UnityEditor.VersionControl;
using UnityEngine;

public struct BulletState
{
    public bool shot;
    public float angle;

    public BulletState(bool s) {
        shot = s;
        angle = 0;
    }
}
public struct PlayerState
{
    public float time;
    public Transform transform;
    public BulletState bulletState;    
}
public class GameState : MonoBehaviour
{
    const int MESSAGE_PACK_SIZE = 1024;

    [SerializeField]
    Transform otherPlayer;
    [SerializeField]
    Transform myPlayer;

    bool hasUpdated;
    PlayerState otherState;
    Thread recievingMessages; //TODO: Activate Thread on connection
    void Start()
    {
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
        myState.bulletState = new BulletState(false); //TODO: Erase (its a test)
        //TODO: Check if bullet has been shot

        byte[] messageData = ToBytes(myState);
        //TODO: Send message
    }

    //THREAD
    void RecieveOtherState()
    {
        //TODO: Always (while true) Recive message (this is a thread)

        PlayerState message = new PlayerState();
        message.time = 2.0f; //TODO: Erase

        if(message.time > otherState.time)
        {
            //PROBLEMS? Maybe needs (lock)
            otherState = message;
            hasUpdated = true;
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
