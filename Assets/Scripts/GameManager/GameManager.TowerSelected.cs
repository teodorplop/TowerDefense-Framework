using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {
	private Tower selectedTower;

	private IEnumerator TowerSelected_EnterState(object input) {
		selectedTower = (Tower)input;
		gameUI.SelectTower(selectedTower);
		glowEffect.Enable(selectedTower.gameObject);
		towerRangeIndicator.Enable(selectedTower.transform.position, selectedTower.CurrentLevel.attackRange, selectedTower.CurrentLevel.auraRange);
		EventManager.AddListener<TowerButtonEvent>(OnTowerButtonEvent);
		EventManager.AddListener<UpgradeTowerButtonEvent>(OnUpgradeTowerButtonEvent);
		EventManager.AddListener<SellTowerButtonEvent>(OnSellTowerButtonEvent);
		yield return null;
	}
	private IEnumerator TowerSelected_ExitState() {
		EventManager.RemoveListener<TowerButtonEvent>(OnTowerButtonEvent);
		EventManager.RemoveListener<UpgradeTowerButtonEvent>(OnUpgradeTowerButtonEvent);
		EventManager.RemoveListener<SellTowerButtonEvent>(OnSellTowerButtonEvent);
		if (selectedTower != null) {
			glowEffect.Disable(selectedTower.gameObject);
			selectedTower = null;
		}
		gameUI.SelectTower(null);
		towerRangeIndicator.Disable();
		yield return null;
	}

	private void TowerSelected_Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SetState(State.Idle);
			return;
		}

		TowerMouseSelection();
	}

	private void OnUpgradeTowerButtonEvent(UpgradeTowerButtonEvent evt) {
		if (!selectedTower.Upgradeable)
			gameUI.ShowFadingMessage(Input.mousePosition, "Already at max level.");
		else if (!player.CheckCurrency(selectedTower.NextLevel.cost))
			gameUI.ShowFadingMessage(Input.mousePosition, "Can't afford upgrade.");
		else {
			player.SubtractCurrency(selectedTower.NextLevel.cost);
			selectedTower.Upgrade();

			towerRangeIndicator.Enable(selectedTower.transform.position, selectedTower.CurrentLevel.attackRange, selectedTower.CurrentLevel.auraRange);
		}
	}

	private void OnSellTowerButtonEvent(SellTowerButtonEvent evt) {
		player.AddCurrency(selectedTower.SellPrice);
		Destroy(selectedTower.gameObject);
		selectedTower = null;
		SetState(State.Idle);
	}
}
