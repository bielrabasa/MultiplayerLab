using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoBehaviour
{
    public GameObject[] obstacle;
    public GameObject[] bomb;
    public static GameObject[] tanks;

    // Start is called before the first frame update
    void Start()
    {
        obstacle = AddObjects("FENCE");
        bomb = AddObjects("BOMB");
        tanks = AddObjects("TANK");
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

   
}
