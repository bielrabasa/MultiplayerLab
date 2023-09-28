using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 10.0f;
    Transform top;
    Transform bot;

    // Start is called before the first frame update
    void Start()
    {
        top = transform.GetChild(0);
        bot = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) movement += Vector3.up;
        if (Input.GetKey(KeyCode.S)) movement += Vector3.down;
        if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movement += Vector3.right;

        transform.Translate(movement.normalized * Time.deltaTime);
        //

        //Rotation
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            top.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            top.transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
        }
    }
}
