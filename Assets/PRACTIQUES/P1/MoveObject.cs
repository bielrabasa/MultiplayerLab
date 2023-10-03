using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class MoveObject : MonoBehaviour
{
    Thread myThread;

    // Start is called before the first frame update
    void Start()
    {
        myThread = new Thread(testThreadStart);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1));

        //Thread
        if (Input.GetKeyDown(KeyCode.Return))
        {
            myThread.Start();
            Debug.Log("Thread started");
        }

        //Main
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 10000000; i++)
            {
                float a = Mathf.Sqrt(Mathf.Cos(i));
            }
            Debug.Log("Main finished");
        }
    }

    //Test Thread
    void testThreadStart()
    {
        for (int i = 0; i < 10000000; i++)
        {
            float a = Mathf.Sqrt(Mathf.Cos(i));
        }

        Debug.Log("Thread finished");
    }
}
