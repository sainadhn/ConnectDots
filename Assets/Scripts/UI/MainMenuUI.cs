using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : UIScreen
{
    private void Awake()
    {
        base.Awake();
    }

    public override void OnClick(string name)
    {
        base.OnClick(name);
        GameManager.Instance.loadedLevel = int.Parse(name);

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
