using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageTypes;

public class DataTank : MonoBehaviour
{
    public static string[] names;
    public static Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        names = new string[GeneralServer.MAX_PLAYERS];
        colors = new Color[GeneralServer.MAX_PLAYERS];

        if (MessageManager.messageDistribute.Count == 0) return;
        MessageManager.messageDistribute[MessageType.SETTINGS] += MessageSettings;
    }

    private void OnDestroy()
    {
        if (MessageManager.messageDistribute.Count == 0) return;
        MessageManager.messageDistribute[MessageType.POSITION] -= MessageSettings;
    }

    public void MessageSettings(Message m)
    {
        Settings s = m as Settings;

        SaveDataTank(s.playerID, s.tankName, s.color);
        SetSettingsTanks();
    }

    public void SaveDataTank(int id, string name, Color color)
    {
        names[id] = name;
        colors[id] = color;
    }

    public void SetSettingsTanks()
    {
        for (int i = 0; i < ObjectsManager.tanks.Length; i++)
        {
            ColorTank tankSettings = ObjectsManager.tanks[i].GetComponentInChildren<ColorTank>();

            if(names[i] == "" || names[i] == null) tankSettings.SetName("Player " + (i + 1));
            else tankSettings.SetName(names[i]);

            tankSettings.SetColorInGame(colors[i]);
        }
    }
}
