using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeedrunManager : MonoBehaviour
{
    public static SpeedrunManager Instance;

    [Header("Nomes das Cenas")]
    public string startScene = "A_Vila_Abertura";
    public string endScene = "Winning_Scene";
    public string resultTextObjName = "SpeedRunTextAdd";

    private DateTime startTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == startScene)
        {
            SaveStartTime();
        }

        if (sceneName == endScene)
        {
            DisplayFinalTime();
        }
    }

    void SaveStartTime()
    {
        startTime = DateTime.Now;
        PlayerPrefs.SetString("SpeedrunStart", startTime.ToBinary().ToString());
        PlayerPrefs.Save();

        Debug.Log("Speedrun começou às: " + startTime);
    }

    void DisplayFinalTime()
    {
        if (!PlayerPrefs.HasKey("SpeedrunStart"))
        {
            Debug.LogWarning("Não existe tempo inicial guardado.");
            return;
        }

        long binary = Convert.ToInt64(PlayerPrefs.GetString("SpeedrunStart"));
        DateTime storedStartTime = DateTime.FromBinary(binary);
        DateTime now = DateTime.Now;

        TimeSpan diff = now - storedStartTime;

        Debug.Log("Tempo final: " + diff);

        ShowTimeOnUI(diff);
    }

    void ShowTimeOnUI(TimeSpan time)
    {
        GameObject obj = GameObject.Find(resultTextObjName);

        if (obj == null)
        {
            Debug.LogWarning("Objeto de texto '" + resultTextObjName + "' não encontrado.");
            return;
        }

        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        if (text == null) return;

        text.text = "You Completed the Game in : " + FormatTime(time);
    }

    string FormatTime(TimeSpan t)
    {
        return $"{t.Minutes:00}:{t.Seconds:00}";
    }
}
