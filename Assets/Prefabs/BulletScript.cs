using MessageTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float timer = 0.0f;

    public Vector2 bulletVelocity;
    [SerializeField] GameObject explosion;

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
            if (timer > 10.0f) Destroy(gameObject);
        }

        //Yes, this is ugly, but nothing else works
        if (transform.position.z != 0.0f)
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TANK"))
        {
            if (FindObjectOfType<GameState>().IsOtherTank(collision.gameObject.transform))
            {
                MessageManager.SendMessage(MessageTypes.MessageType.KILL);
                collision.gameObject.SetActive(false);
                gameState.EndGame("You Win");
            }
        }

        if (collision.gameObject.CompareTag("FENCE"))
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("BOMB"))
        {
            Transform auxiliarTransform = collision.transform;
            Instantiate(explosion, auxiliarTransform.position, auxiliarTransform.rotation);

            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}
