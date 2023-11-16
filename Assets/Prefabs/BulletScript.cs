using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;
    float timer = 0.0f;

    bool bounce = false;

    private void Start()
    {
        timer = 0.0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 5.0f) Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TANK"))
        {
            //Destroy(collision.gameObject);
            FindObjectOfType<GameState>().events.Add(MultiplayerEvents.KILL);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("WALL"))
        {
            if(!bounce)
            {
                GetComponent<Rigidbody2D>().velocity = transform.up * (speed / 2);
                GetComponent<Rigidbody2D>().velocity = Vector3.Reflect(GetComponent<Rigidbody2D>().velocity, Vector3.right);
                bounce = true;
            }
            else 
            {
                Destroy(gameObject);
            }
        }
    }

    public void Shoot()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }
}
