using System.Text;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using MessageTypes;

public class GameState : MonoBehaviour
{
    const string BLUE_TANK_NAME = "Tank (blue)";
    const string RED_TANK_NAME = "Tank (red)";
    [SerializeField] float MESSAGE_SEND_DELAY = 1.0f;

    //Real ingame objects
    Transform otherPlayer;
    Transform myPlayer;
    //Server -> Blue
    //Client -> Red

    //State Game
    public bool isGamePaused;
    PostProcessVolume postpo;
    float startResetHoldTime;
    float R_HOLDING_TIME = 3.0f;

    void Start()
    {
        isGamePaused = false;
        startResetHoldTime = 0;

        postpo = FindAnyObjectByType<PostProcessVolume>();
        postpo.gameObject.SetActive(false);

        GetPlayers();

        MessageManager.messageDistribute[MessageType.POSITION] += UpdateOtherState;
        StartCoroutine(SendMyState());
    }

    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.P))
        {
            SendPauseGame(!isGamePaused);
        }

        //Hold R
        if (Input.GetKeyDown(KeyCode.R)) startResetHoldTime = Time.time;
        if (Input.GetKey(KeyCode.R) && (Time.time - startResetHoldTime) >= R_HOLDING_TIME)
        {
            SendEvent(MultiplayerEvents.RESET);
            ResetGame();
        }*/
    }

    void UpdateOtherState(Message message)
    {
        Debug.Log("Position Message Recieved in: " + message.time);

        Position p = message as Position;
        otherPlayer.position = p.pos;
        otherPlayer.GetChild(0).rotation = Quaternion.Euler(0, 0, p.topRot);
        otherPlayer.GetChild(1).rotation = Quaternion.Euler(0, 0, p.botRot);

        /*foreach (MultiplayerEvents e in otherState.events)
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

                case MultiplayerEvents.RESET:
                    ResetGame();
                    break;
                case MultiplayerEvents.OBSTACLE:
                    DestroyObstacle(obstacleToDestroy);
                    break;

                case MultiplayerEvents.BOMB:
                    SetBomb(obstacleToDestroy);
                    break;
            }
        }*/
    }

    void GetPlayers()
    {
        TankScript[] ts = FindObjectsOfType<TankScript>();
        foreach (TankScript t in ts)
        {
            if (t.gameObject.name == BLUE_TANK_NAME)
            {
                if (MessageManager.isServer)
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
                if (!MessageManager.isServer)
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
        while (true)
        {
            yield return new WaitForSecondsRealtime(MESSAGE_SEND_DELAY);
            SendStateOnce();
        }
    }

    void SendStateOnce()
    {
        Debug.Log("MessageSent");
        MessageManager.SendMessage(new Position(myPlayer.position, myPlayer.GetChild(0).rotation.eulerAngles.z, myPlayer.GetChild(1).rotation.eulerAngles.z));
    }

    //
    //  INGAME FUNCTIONALITIES
    //

    /*public void SendPauseGame(bool pause)
    {
        SetPause(pause);
        MessageManager.SendMessage(new Message(pause ? MessageType.PAUSE : MessageType.UNPAUSE));
    }

    void SetPause(bool pause)
    {
        postpo.gameObject.SetActive(pause);
        isGamePaused = pause;
    }*/

    void KillGame()
    {
        BulletScript[] bullets = FindObjectsOfType<BulletScript>();
        foreach(BulletScript b in bullets)
        {
            Destroy(b.gameObject);
        }

        TralScript[] trails = FindObjectsOfType<TralScript>();
        foreach (TralScript t in trails)
        {
            Destroy(t.gameObject);
        }

        StopCoroutine(SendMyState());
    }

    /*public void ResetGame()
    {
        KillGame();
        SceneManager.LoadScene("MainScene");
    }*/

    /*public void SendDestroyObstacle(GameObject GO)
    {
        obstacleToDestroy = GO;

        DestroyObstacle(obstacleToDestroy);

        SendEvent(MultiplayerEvents.OBSTACLE);
    }

    public void DestroyObstacle(GameObject GO)
    {
        GO.SetActive(false);
    }

    public void SetBomb(GameObject GO)
    {
        GO.SetActive(false);

        //StartCoroutine(ActiveBomb(GO));

        //GO.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        //GO.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0.22f, 0.22f);

    }

    IEnumerator ActiveBomb(GameObject GO)
    {
        yield return new WaitForSecondsRealtime(1);

        //Damage
        //Explotion(GO);

        GO.SetActive(false);
    }

    public void Explotion(GameObject GO)
    {
        Instantiate(GO, GO.transform);
    }*/
}
