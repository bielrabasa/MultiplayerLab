using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fool : MonoBehaviour
{
    [SerializeField] string SceneName = "New Scene";

    void Update()
    {
        //transform.Rotate(Vector3.forward * 10.0f);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
