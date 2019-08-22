using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelEditorManager {
	private TowerSpotGrid inHandtowerSpot;
	private Material inHandTowerSpotMaterial;

	private IEnumerator PlaceTowerSpot_EnterState(object input) {
		editorUI.SetGuidanceMessage(@"Left Click: Place starting point
Left Shift: Free movement
Escape: Cancel");

		inHandtowerSpot = Instantiate(towerSpotPrefab, cameraController.ScreenToWorld(Input.mousePosition), Quaternion.identity);
		inHandtowerSpot.transform.localScale = new Vector3(inHandtowerSpot.CellSize, inHandtowerSpot.transform.localScale.y, inHandtowerSpot.CellSize);
		inHandtowerSpot.name = level.towerSpots.Count.ToString();
		inHandtowerSpot.transform.parent = towerSpotParent;
		inHandTowerSpotMaterial = inHandtowerSpot.GetComponent<Renderer>().material;
		yield return null;
	}
	private IEnumerator PlaceTowerSpot_ExitState() {
		editorUI.SetGuidanceMessage(string.Empty);
		yield return null;
	}

	private void PlaceTowerSpot_Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Destroy(inHandtowerSpot.gameObject);
			inHandTowerSpotMaterial = null;
			SetState(State.EditorIdle);
			return;
		}

		Vector3 worldPos = cameraController.ScreenToWorld(Input.mousePosition);
		if (!Input.GetKey(KeyCode.LeftShift)) worldPos = worldPos.RoundToInt();
		worldPos.y = towerSpotPrefab.transform.position.y + 0.05f;
		inHandtowerSpot.transform.position = worldPos;

		bool available = CheckSpotAvailable(worldPos, inHandtowerSpot.transform.localScale);
		inHandTowerSpotMaterial.color = available ? Color.white : Color.red;

		if (available && Input.GetMouseButtonDown(0)) SetState(State.ResizeTowerSpot);
	}

	private bool CheckSpotAvailable(Vector3 spotPos) {
		for (int i = 0; i < level.towerSpots.Count; ++i) {
			Vector2 pos = level.towerSpots[i].position;
			Vector2 scale = level.towerSpots[i].scale;
			scale *= towerSpotPrefab.CellSize;
			Rect rect = new Rect(pos - scale / 2, scale);

			if (rect.Contains(new Vector2(spotPos.x, spotPos.z))) return false;
		}
		return true;
	}
}
