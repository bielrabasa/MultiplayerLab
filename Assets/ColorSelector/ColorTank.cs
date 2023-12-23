using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorTank : MonoBehaviour
{
    public void SetColor(Color newColor)
    {
        GetComponent<Image>().color = newColor;
        Debug.Log(newColor);
    }
}
