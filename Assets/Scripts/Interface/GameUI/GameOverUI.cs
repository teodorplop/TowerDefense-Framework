using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour {
	[SerializeField] private TMP_Text gameOverText;

	public void Show(bool success) {
		gameOverText.text = success ? "Level Finished!" : "Level Failed :(";

		gameObject.SetActive(true);
	}

	public void OnRestart() {
		EventManager.Raise(new RestartGameEvent());
	}

	public void OnExit() {
		EventManager.Raise(new ExitToMenuEvent());
	}
}
