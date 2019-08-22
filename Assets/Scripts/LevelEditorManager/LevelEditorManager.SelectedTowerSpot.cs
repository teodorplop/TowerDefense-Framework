using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelEditorManager {
	private GameObject selectedTowerSpot;

	private IEnumerator SelectedTowerSpot_EnterState(object input) {
		glowEffect.Enable(selectedTowerSpot = (GameObject)input);
		yield return null;
	}
	private IEnumerator SelectedTowerSpot_ExitState() {
		if (selectedTowerSpot != null) {
			glowEffect.Disable(selectedTowerSpot);
			selectedTowerSpot = null;
		}
		yield return null;
	}

	private void SelectedTowerSpot_Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SetState(State.EditorIdle);
			return;
		}

		if (Input.GetKeyDown(KeyCode.Delete)) {
			int towerSpotIdx = selectedTowerSpot.transform.GetSiblingIndex();
			level.towerSpots.RemoveAt(towerSpotIdx);
			Destroy(selectedTowerSpot);

			SetState(State.EditorIdle);
			return;
		}

		MouseSelection();
	}
}
