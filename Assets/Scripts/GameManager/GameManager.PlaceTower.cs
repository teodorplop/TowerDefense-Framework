using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {
	private Tower towerInHand;

	private IEnumerator PlaceTower_EnterState(object input) {
		towerInHand = towerFactory.InstantiateTower((int)input, cameraController.ScreenToWorld(Input.mousePosition));
		yield return null;
	}
	private IEnumerator PlaceTower_ExitState() {
		towerSpotHighlighter.Disable();
		towerInHand = null;
		yield return null;
	}

	private void PlaceTower_Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Destroy(towerInHand.gameObject);
			towerInHand = null;
			SetState(State.Idle);
			return;
		}
		
		var mouseHit = cameraController.ScanFor<TowerSpotGrid>(Input.mousePosition, towerSpotLayer);
		if (mouseHit.HasValue) {
			TowerSpotGrid towerSpot = mouseHit.Value.hitObject;
			Vector3 hitPoint = mouseHit.Value.hitPoint;

			Vector2Int grid = towerSpot.WorldToGrid(hitPoint);
			bool available = towerSpot.IsAvailable(grid);
			hitPoint = towerSpot.GridToWorld(grid);
			towerSpotHighlighter.Enable(hitPoint, available ? Color.green : Color.red);
			towerInHand.transform.position = hitPoint;

			if (available && Input.GetMouseButtonDown(0)) {
				player.SubtractCurrency(towerInHand.CurrentLevel.cost);
				towerSpot.Occupy(grid, towerInHand);
				EventManager.Raise(new TowerSpawnedEvent(towerInHand));
				SetState(State.Idle);
			}
		} else {
			towerInHand.transform.position = cameraController.ScreenToWorld(Input.mousePosition);
			towerSpotHighlighter.Disable();
		}
	}
}
