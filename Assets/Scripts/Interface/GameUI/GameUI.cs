using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {
	[SerializeField] private GameObject startLevelButton;
	[SerializeField] private GameObject nextWaveButton;
	[SerializeField] private TowersPanel towersPanel;
	[SerializeField] private FadingMessage fadingMessage;
	[SerializeField] private PlayerStats playerStats;
	[SerializeField] private TowerInfo selectedTower;
	[SerializeField] private GameOverUI gameOverUI;

	private new Camera camera;

	public TowersPanel TowersPanel { get { return towersPanel; } }
	public PlayerStats PlayerStats { get { return playerStats; } }

	public void Inject(Camera camera) {
		this.camera = camera;
	}

	public void EnableStartButton(bool enabled) {
		startLevelButton.SetActive(enabled);
	}

	public void EnableNextWaveButton(bool enabled) {
		nextWaveButton.SetActive(enabled);
	}

	public void SelectTower(Tower tower) {
		selectedTower.Show(tower);
	}

	public void ShowFadingMessage(Vector3 screenPos, string message) {
		fadingMessage.Show(screenPos, message);
	}

	public void ShowGameOver(bool success) {
		gameOverUI.Show(success);
	}

	public void OnStartLevelButton() {
		EventManager.Raise(new StartLevelButtonEvent());
	}

	public void OnNextWaveButton() {
		EventManager.Raise(new NextWaveButtonEvent());
	}

	public void OnUpgradeButton() {
		EventManager.Raise(new UpgradeTowerButtonEvent());
	}

	public void OnSellButton() {
		EventManager.Raise(new SellTowerButtonEvent());
	}
}
