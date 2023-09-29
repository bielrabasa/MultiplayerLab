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

    // Start is called before the first frame update
    void Start()
    {
        top = transform.GetChild(0);
        bot = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        //Shoot
        if (Input.GetKeyDown(KeyCode.Space)) Shoot();

        //Tank Movement
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) movement += Vector3.up;
        if (Input.GetKey(KeyCode.S)) movement += Vector3.down;
        if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movement += Vector3.right;
        movement.Normalize();

        transform.Translate(movement * Time.deltaTime);

        //Visual Tank Rotation
        if(movement != Vector3.zero)
        {
            Quaternion newRot = Quaternion.AngleAxis(Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + 90, Vector3.forward);
            bot.rotation = Quaternion.Slerp(bot.rotation, newRot, bottomRotationSpeed);
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
            top.rotation = Quaternion.Slerp(top.rotation, newAngle, topRotationSpeed);
        }
    }

    void Shoot()
    {
        GameObject b = Instantiate(bullet, transform.position, 
            Quaternion.AngleAxis(top.rotation.eulerAngles.z + 180, Vector3.forward));
        b.GetComponent<BulletScript>().Shoot();
    }
}