using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ColorPickButton : MonoBehaviour
{
    //https://www.youtube.com/watch?v=M6kEW6feh7g
    public UnityEvent<Color> ColorPickerEvent;
    
    [SerializeField] Texture2D colorChart;
    [SerializeField] GameObject chart;
    [SerializeField] RectTransform cursor;
    [SerializeField] Image button;
    [SerializeField] Image cursorColor;
    public void PickColor(BaseEventData data)
    {
        PointerEventData pointer = data as PointerEventData;

        cursor.position = pointer.position;

        Color pickedColor = colorChart.GetPixel(
            (int)((cursor.localPosition.x + 150) * (colorChart.width / transform.GetChild(0).GetComponent<RectTransform>().rect.width)),
            (int)((cursor.localPosition.y + 200) * (colorChart.height / transform.GetChild(0).GetComponent<RectTransform>().rect.height)));

        button.color = pickedColor;
        cursorColor.color = pickedColor;
        ColorPickerEvent.Invoke(pickedColor);
    }
}