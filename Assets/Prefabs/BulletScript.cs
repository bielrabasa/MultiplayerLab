using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //do smth
    }

    public void Shoot()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }
}
