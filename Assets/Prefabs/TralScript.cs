using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TralScript : MonoBehaviour
{
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1.0f) Destroy(gameObject);
    }
}
