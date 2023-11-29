using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    [SerializeField] float topRotationSpeed = 10.0f;
    [SerializeField] float bottomRotationSpeed = 0.1f;
    Transform top;
    Transform bot;

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject trail;
    Transform trailStorage;
    [SerializeField] float trailSpawnDelay = 1.0f;

    //Multiplayer
    bool movementBlocked = false;

    //GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        top = transform.GetChild(0);
        bot = transform.GetChild(1);

        //gameState = FindObjectOfType<GameState>();
        trailStorage = GameObject.Find("Trails").transform;

        StartCoroutine(Trail());
    }

    // Update is called once per frame
    void Update()
    {
        //if (movementBlocked || gameState.isGamePaused) return;
        if (movementBlocked) return;

        //Shoot
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /*gameState.SendEvent(MultiplayerEvents.SHOOT);
            Shoot();*/
        }

        //Tank Movement
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)){ movement += Vector3.up;       }
        if (Input.GetKey(KeyCode.S)){ movement += Vector3.down;     }
        if (Input.GetKey(KeyCode.A)){ movement += Vector3.left;     }
        if (Input.GetKey(KeyCode.D)){ movement += Vector3.right;    }

        movement.Normalize();

        transform.Translate(movement * Time.deltaTime);

        //Visual Tank Rotation
        if(movement != Vector3.zero)
        {
            Quaternion newRot = Quaternion.AngleAxis(Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + 90, Vector3.forward);
            bot.rotation = Quaternion.Slerp(bot.rotation, newRot, bottomRotationSpeed * Time.deltaTime);
        }

        //Cannon Rotation
        Vector3 rot = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow)) rot += Vector3.up;
        if (Input.GetKey(KeyCode.DownArrow)) rot += Vector3.down;
        if (Input.GetKey(KeyCode.LeftArrow)) rot += Vector3.left;
        if (Input.GetKey(KeyCode.RightArrow)) rot += Vector3.right;
        rot.Normalize();

        if(rot != Vector3.zero)
        {
            Quaternion newAngle = Quaternion.AngleAxis(Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg + 90, Vector3.forward);
            top.rotation = Quaternion.Slerp(top.rotation, newAngle, topRotationSpeed * Time.deltaTime);
        }
    }

    public void Shoot()
    {
        Quaternion dir = Quaternion.AngleAxis(top.rotation.eulerAngles.z + 180, Vector3.forward);
        Vector3 spawnDist = dir * Vector3.up * 0.7f;
        GameObject b = Instantiate(bullet, transform.position + spawnDist, dir);
        b.GetComponent<BulletScript>().Shoot();
    }

    IEnumerator Trail()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(trailSpawnDelay);

            //if (!gameState.isGamePaused)
            //{
                bot.rotation.Normalize();

                Quaternion dir = Quaternion.AngleAxis(bot.rotation.eulerAngles.z + 180, Vector3.forward);
                Vector3 spawnDist = dir * Vector3.up * -0.2f;

                Instantiate(trail, transform.position + spawnDist, dir, trailStorage);
            //}
        }
    }

    //Multiplayer
    public void BlockMovement()
    {
        movementBlocked = true;
    }
}