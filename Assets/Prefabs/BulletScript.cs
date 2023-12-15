using MessageTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    float timer = 0.0f;
    bool bounce = false;

    public Vector2 bulletVelocity;

    GameState gameState;
    ObjectsManager oManager;

    private void Start()
    {
        timer = 0.0f;
        gameState = FindObjectOfType<GameState>();
        oManager = FindObjectOfType<ObjectsManager>();
    }

    private void Update()
    {
        if(!gameState.isGamePaused)
        {
            timer += Time.deltaTime;
            if (timer > 5.0f) Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TANK"))
        {
            if (FindObjectOfType<GameState>().IsOtherTank(collision.gameObject.transform))
            {
                MessageManager.SendMessage(MessageTypes.MessageType.KILL);
                collision.gameObject.SetActive(false);
            }

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("WALL"))
        {
            if(!bounce)
            {
                //bounce = true;
            }
            else 
            {
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("FENCE"))
        {
            for (int i = 0; i < oManager.obstacle.Length; i++)
            {
                if (oManager.obstacle[i] == collision.gameObject)
                {
                    MessageManager.SendMessage(new Obstacle(i, MessageType.FENCE));
                    gameState.DestroyFence(i);
                }
            }

            /*FindObjectOfType<GameState>().SendEvent(MultiplayerEvents.OBSTACLE, collision.transform);

            if (!bounce)
            {
                GetComponent<Rigidbody2D>().velocity = transform.up * (speed / 2);
                GetComponent<Rigidbody2D>().velocity = Vector3.Reflect(GetComponent<Rigidbody2D>().velocity, Vector3.right);
                bounce = true;
            }
            else
            {
                Destroy(gameObject);
            }*/
        }

        if (collision.gameObject.CompareTag("BOMB"))
        {
            for (int i = 0; i < oManager.bomb.Length; i++)
            {
                if (oManager.bomb[i] == collision.gameObject)
                {
                    MessageManager.SendMessage(new Obstacle(i, MessageType.BOMB));
                    gameState.DestroyBomb(i);
                }
            }
        }
    }
}
