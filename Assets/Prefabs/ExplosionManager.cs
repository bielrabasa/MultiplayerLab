using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosionManager : MonoBehaviour
{
    float timer = 0.0f;

    public float explosionTime = 2.0f;

    GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameState.isGamePaused)
        {
            timer += Time.deltaTime;

            if (timer < (explosionTime*0.5f)) 
            {
                this.transform.localScale = new Vector3((timer + 1), (timer + 1), (timer + 1));
            }
            else
            {
                float auxScale = this.transform.localScale.x;
                this.transform.localScale = new Vector3((auxScale - 0.001f), (auxScale - 0.001f), (auxScale - 0.001f));
                GetComponentInChildren<SpriteRenderer>().color = new Color (1,1,1, (explosionTime - timer));
            }

            if (timer > explosionTime) Destroy(gameObject);
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
    }
}
