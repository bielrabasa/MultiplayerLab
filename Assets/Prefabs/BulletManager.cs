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
        DontDestroyOnLoad(gameObject); //TODO: destroy on real change scene

        if (MessageManager.messageDistribute.Count == 0) return;
        MessageManager.messageDistribute[MessageType.SHOOT] += MessageShoot;
        MessageManager.messageDistribute[MessageType.PAUSE] += MessagePause;
        MessageManager.messageDistribute[MessageType.UNPAUSE] += MessageUnpause;
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
        //TODO: Test Spawnpos

        //Set speed
        b.GetComponent<Rigidbody2D>().AddForce(b.transform.up * speed);
    }


    void MessagePause(Message message)
    {
        StopBullets();
    }

    void MessageUnpause(Message message)
    {
        ReplayBullets();
    }

    public void StopBullets()
    {
        //TODO: stop bullet counting time (set isStopped to true)
        
        foreach(Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>()) {
            rb.gameObject.GetComponent<BulletScript>(); //TODO: save rb.velocity as bulletVelocity from BulletScript 
            rb.velocity = Vector3.zero;
        }
    }

    public void ReplayBullets()
    {
        foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
        {
            rb.gameObject.GetComponent<BulletScript>(); //TODO: read bulletVelocity from BulletScript to set the rb velocity
            rb.velocity = rb.transform.up; //TODO: not workin well
        }
    }
}
