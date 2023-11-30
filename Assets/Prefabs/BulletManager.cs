using MessageTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] GameObject bullet;

    private void Start()
    {
        MessageManager.messageDistribute[MessageType.SHOOT] += MessageShoot;
    }

    void MessageShoot(Message message)
    {
        Shoot s = message as Shoot;
        Shoot(s.pos, s.rot, Time.time - s.time);
    }

    public void Shoot(Vector3 pos, float angle, float delayedTime = 0.0f)
    {
        //Instanciate bullet
        Quaternion dir = Quaternion.AngleAxis(angle + 180, Vector3.forward);
        Vector3 spawnDist = dir * Vector3.up * 0.7f;
        GameObject b = Instantiate(bullet, pos + spawnDist, dir, transform);

        //Spawn further for the delayed time
        //b.transform.position += b.transform.up * speed * delayedTime;

        //Set speed
        b.GetComponent<Rigidbody2D>().velocity = b.transform.up * speed;
    }

    //TODO: Stop & Play with pause

    void StopBullets()
    {
        foreach(Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>()) { 
            rb.velocity = Vector3.zero;
        }
    }

    void ReplayBullets()
    {
        foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
        {
            rb.velocity = rb.transform.up * speed;
        }
    }
}