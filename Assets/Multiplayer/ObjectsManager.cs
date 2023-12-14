using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoBehaviour
{
    GameObject[] obstacle;

    // Start is called before the first frame update
    void Start()
    {
        obstacle = AddObjects("OBSTACLE");
    }
    
    //return an array of all the same GameObjects with the given tag
    GameObject[] AddObjects(string tag)
    {
        GameObject[] allObjects =  GameObject.FindGameObjectsWithTag(tag);
        return allObjects;
    }

    //return a GameObject from an array given the id of the this
    GameObject FindObjectbyID(int id, GameObject[] arrayObjects)
    {
        foreach (GameObject obj in arrayObjects) 
        {
            if (obj.GetInstanceID() == id)
            {
                return obj;
            }
        }
        return null;
    }
}
