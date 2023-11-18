using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TralScript : MonoBehaviour
{
    float timer = 0.0f;

    GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        timer = 2.0f;
        gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameState.isGamePaused)
        {
            timer -= Time.deltaTime;

            if (timer < 1.0f) GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, timer);
            if (timer < 0.0f) Destroy(gameObject);
        }

    }
}
