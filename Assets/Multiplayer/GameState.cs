using System.Text;
using UnityEngine;
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

        MessageManager.messageDistribute[MessageType.POSITION] += MessagePosition;
        MessageManager.messageDistribute[MessageType.KILL] += MessageKill;

        MessageManager.messageDistribute[MessageType.PAUSE] += MessagePause;
        MessageManager.messageDistribute[MessageType.UNPAUSE] += MessagePause;
        MessageManager.messageDistribute[MessageType.RESET] += MessageReset;

        StartCoroutine(SendMyState());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SendPauseGame(!isGamePaused);
        }

        //Hold R
        if (Input.GetKeyDown(KeyCode.R)) startResetHoldTime = Time.time;
        if (Input.GetKey(KeyCode.R) && (Time.time - startResetHoldTime) >= R_HOLDING_TIME)
        {
            MessageManager.SendMessage(MessageType.RESET);
            ResetGame();
        }
    }

    void MessagePosition(Message message)
    {
        Position p = message as Position;
        otherPlayer.position = p.pos;
        otherPlayer.GetChild(0).rotation = Quaternion.Euler(0, 0, p.topRot);
        otherPlayer.GetChild(1).rotation = Quaternion.Euler(0, 0, p.botRot);
    }

    void MessageKill(Message message)
    {
        myPlayer.gameObject.SetActive(false);
    }

    void MessagePause(Message message)
    {
        //Pause or unpause
        SetPause(message.type == MessageType.PAUSE);
    }

    void MessageReset(Message message)
    {
        ResetGame();
    }
   
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
            MessageManager.SendMessage(new Position(myPlayer.position, 
                myPlayer.GetChild(0).rotation.eulerAngles.z, 
                myPlayer.GetChild(1).rotation.eulerAngles.z));
        }
    }

    //
    //  INGAME FUNCTIONALITIES
    //

    public void SendPauseGame(bool pause)
    {
        SetPause(pause);
        MessageManager.SendMessage(new Message(pause ? MessageType.PAUSE : MessageType.UNPAUSE));
    }

    void SetPause(bool pause)
    {
        postpo.gameObject.SetActive(pause);
        isGamePaused = pause;
    }

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

    public void ResetGame()
    {
        KillGame();
        SceneManager.LoadScene("MainScene");
    }

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
