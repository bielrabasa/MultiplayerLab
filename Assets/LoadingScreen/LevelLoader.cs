using UnityEngine;
using UnityEngine.SceneManagement;
public static class LevelLoader
{
    public static string nexrLevel;

    public static void LoadLevel(string nameLevel)
    {
        nexrLevel = nameLevel;
        //name of the loading scene you have
        SceneManager.LoadScene("LoadingScene");
    }
}
