using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorTank : MonoBehaviour
{
    [SerializeField] TMP_Text inGameTankName;
    [SerializeField] GameObject bottomTank;
    [SerializeField] TMP_InputField textName;

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

    public void SetName(string nameTank = "Player")
    {
        inGameTankName.text = nameTank;
    }

    public Color GetColor()
    {
        Color newColor = GetComponent<Image>().color;
        return newColor;
    }

    public string GetName()
    {
        string newName = textName.text;
        return newName;
    }
}
