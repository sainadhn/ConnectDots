using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour 
{
	public enum UIScreens 
	{
		NONE,
		MainMenu,
		GameUI,
        Gameover,
    };

	private static	ScreenManager	instance = null;

	private 	Dictionary<UIScreens, GameObject> screenDict;

	public UIScreens	activeUIScreen;

	public static ScreenManager Instance {
		get {
			return instance;
		}
	}
	float durationForIdleCheck;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			screenDict = new Dictionary<UIScreens, GameObject>();
			DontDestroyOnLoad(this);
		}
		else
			Destroy(gameObject);
	}

	public void FindAllScreens()
	{
        GameObject[] canvas = GameObject.FindGameObjectsWithTag("Canvas");

        for(int i=0; i<canvas.Length; i++)
        {
            UIScreen[] screens = canvas[i].GetComponentsInChildren<UIScreen>(true);
            for (int j = 0; j < screens.Length; j++)
            {
                AddScreen(screens[j].thisScreen, screens[j].gameObject);
            }
        }
	}

	void AddScreen(UIScreens scr, GameObject screenObj)
	{
		GameObject screenObject = null;

		if (screenDict.TryGetValue(scr, out screenObject))
			screenDict[scr] = screenObj;
		else
			screenDict.Add(scr, screenObj);
	}

	
	public void ShowUIScreen(UIScreens screen)
	{
		screenDict[screen].SetActive(true);

		activeUIScreen = screen;
	}

	public GameObject	GetScreenObject(UIScreens	scr)
	{
		return screenDict[scr];
	}
}
