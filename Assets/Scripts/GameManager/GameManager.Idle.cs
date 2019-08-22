using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {
	private IEnumerator Idle_EnterState(object input) {
		EventManager.AddListener<TowerButtonEvent>(OnTowerButtonEvent);
		yield return null;
	}
	private IEnumerator Idle_ExitState() {
		EventManager.RemoveListener<TowerButtonEvent>(OnTowerButtonEvent);
		yield return null;
	}

	private void Idle_Update() {
		TowerMouseSelection();
	}

	private void TowerMouseSelection() {
		MouseHit? mouseHit = null;
		bool mouseClick = false;

		if (!Utils.MouseOverUI()) {
			mouseHit = cameraController.Scan(Input.mousePosition);
			mouseClick = Input.GetMouseButtonDown(0);
		}

		if (!mouseHit.HasValue) {
			towerSpotHighlighter.Disable();
			if (mouseClick) SetState(State.Idle);
			return;
		}

		TowerSpotGrid towerSpot = mouseHit.Value.hitObject.GetComponent<TowerSpotGrid>();
		if (towerSpot != null) {
			Vector3 hitPoint = mouseHit.Value.hitPoint;
			Vector2Int grid = towerSpot.WorldToGrid(hitPoint);
			hitPoint = towerSpot.GridToWorld(grid);
			Tower tower = towerSpot.GetTower(grid);

			if (tower != null) {
				towerSpotHighlighter.Enable(hitPoint, Color.green);

				if (mouseClick)
					SetState(State.TowerSelected, tower);
			} else {
				if (mouseClick)
					SetState(State.Idle);

				towerSpotHighlighter.Disable();
			}
		} else
			towerSpotHighlighter.Disable();
	}
	
	private void OnTowerButtonEvent(TowerButtonEvent evt) {
		int price = towerFactory.Data[evt.towerId].levels[0].cost;
		if (!player.CheckCurrency(price)) {
			gameUI.ShowFadingMessage(Input.mousePosition, "Can't afford tower.");
		} else
			SetState(State.PlaceTower, evt.towerId);
	}
}
