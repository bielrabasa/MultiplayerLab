using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class fool : MonoBehaviour
{
    [SerializeField] string SceneName = "New Scene";
    [SerializeField] Text Console1;
    [SerializeField] Text Console2;

    string aux = "Soc Aux";

    void Update()
    {
        //transform.Rotate(Vector3.forward * 10.0f);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
    
    public void ConsoleLogs(string data)
    {
        Console2.text = Console1.text;
        Console1.text = data;   
    }
}
