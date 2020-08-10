using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// User interface screen. 
/// attach this to all ui screens and select the appropriate screen id
/// </summary>
public class UIScreen : MonoBehaviour 
{
	public enum ScreenType
	{
		Popup,
		FullScreen
	}

	public ScreenType	screenType;
	public ScreenManager.UIScreens	thisScreen;

	protected void Awake()
	{
		Button[] buttons = transform.GetComponentsInChildren<Button>(true);
		for (int i = 0; i < buttons.Length; i++)
		{
			Button clickedButton = buttons[i];
			clickedButton.onClick.AddListener(() => { OnClick(clickedButton.name); });
		}

		Toggle[] grp = transform.GetComponentsInChildren<Toggle>(true);
		foreach (Toggle t in grp)
		{
			Toggle currentToggle = t;
			currentToggle.onValueChanged.AddListener((bool value) => OnToggle(value, currentToggle.name));
		}
	}

    /// <summary>
    /// onClick listener for all buttons under the screen
    /// </summary>
    /// <param name="name"></param>
	public virtual void OnClick(string name)
    {
        // Play button click sound
    }

    /// <summary>
    /// onValueChanged listener for all toggles under the screen
    /// </summary>
    /// <param name="value"></param>
    /// <param name="toggleName"></param>
	public virtual void OnToggle(bool value, string toggleName)
    {
        // Play toggle sound
    }
}
