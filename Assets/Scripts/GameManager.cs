using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void EventDelegate(params object[] args);
public class GameManager : MonoBehaviour
{

    TextAsset levelsData;
    private static GameManager instance;

    JSONNode levelsJson;
    [HideInInspector] public int colorsInPuzzle;

    [HideInInspector]public int loadedLevel;
    [HideInInspector] public int pipesConnected;
    [HideInInspector] public int gridsFilledPercent;
    [HideInInspector] public int noOfMoves;

    public static GameManager Instance 
    {
        get => instance;
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        levelsData = Resources.Load<TextAsset>("LevelData");
        levelsJson = JSON.Parse(levelsData.text);

        loadedLevel = 1;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        ScreenManager.Instance.FindAllScreens();
    }

    public JSONNode    GetCurrentLevelJsonNode()
    {
        string lvl = "Level" + loadedLevel;

        colorsInPuzzle = levelsJson[lvl].Count;
        return levelsJson[lvl];
    }

    public int ColorsInPuzzle
    {
        get => colorsInPuzzle;
    }
}
