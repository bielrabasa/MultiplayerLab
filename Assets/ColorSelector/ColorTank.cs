using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorTank : MonoBehaviour
{
    [SerializeField] TMP_Text inGameTankName;
    [SerializeField] GameObject bottomTank;

    public void SetColor(Color newColor)
    {
        GetComponent<Image>().color = newColor;
        bottomTank.GetComponent<Image>().color = newColor;
    }

    public void SetColorInGame(Color newColor)
    {
        if (newColor == null) newColor = Color.white;

        GetComponent<SpriteRenderer>().color = newColor;
        bottomTank.GetComponent<SpriteRenderer>().color = newColor;
    }

    public void SetName(string name = "Player")
    {
        inGameTankName.text = name;
    }


    //TODO: Biel, extract the info amb les 2 funcions daqui sota
    Color GetColor()
    {
        Color newColor = GetComponent<Image>().color;
        return newColor;
    }

    string GetName()
    {
        string newName = transform.parent.GetComponentInChildren<TMP_InputField>().text;
        return newName;
    }
}
