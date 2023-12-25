using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoBehaviour
{
    public GameObject[] obstacle;
    public GameObject[] bomb;
    public GameObject[] tanks;

    // Start is called before the first frame update
    void Start()
    {
        obstacle = AddObjects("FENCE");
        bomb = AddObjects("BOMB");
        tanks = AddObjects("TANK");
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)) SetSettingsTanks();
    }

    //return an array of all the same GameObjects with the given tag
    GameObject[] AddObjects(string tag)
    {
        GameObject[] allObjects =  GameObject.FindGameObjectsWithTag(tag);
        return allObjects;
    }

    //return a GameObject from an array given the id of the this
    public GameObject FindObjectbyID(int id, GameObject[] arrayObjects)
    {
        for (int i = 0; i <= arrayObjects.Length; i++)
        {
            if (id == i)
            {
                return arrayObjects[i];
            }
        }

        return null;
    }

    //TODO: Biel recive info (Color and String)
    public void SetSettingsTanks()
    {
        for(int i = 0; i < tanks.Length; i++)
        {
            ColorTank tankSettings = tanks[i].GetComponentInChildren<ColorTank>();
            //TODO: send the string in the message with the function SetName
            tankSettings.SetName();
            //TODO: swap Color.blue with the color in the message
            tankSettings.SetColorInGame(Color.blue);
        }
    }
}
