using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : UIScreen
{
    private void Awake()
    {
        base.Awake();
    }

    public override void OnClick(string name)
    {
        base.OnClick(name);
        switch(name)
        {
            case "Menu":
                SceneManager.LoadScene("MainMenu");
                break;

            case "NextLevel":
                GameManager.Instance.loadedLevel++;
                if (GameManager.Instance.loadedLevel > 10)
                    GameManager.Instance.loadedLevel = 1;
                SceneManager.LoadScene("GameScene");
                break;
        }
    }
}
