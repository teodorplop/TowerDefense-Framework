using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelEditorManager {
	private Vector3 resizeStartingPos;
	private Vector3 resizeStartingScale;
	private float resizeCellSize;

	private IEnumerator ResizeTowerSpot_EnterState(object input) {
		editorUI.SetGuidanceMessage(@"Resize tower spots
Left Click: Commit tower spots
Escape: Cancel");

		resizeStartingPos = inHandtowerSpot.transform.position;
		resizeStartingScale = inHandtowerSpot.transform.localScale;
		resizeCellSize = inHandtowerSpot.CellSize;
		yield return null;
	}
	private IEnumerator ResizeTowerSpot_ExitState() {
		editorUI.SetGuidanceMessage(string.Empty);
		inHandTowerSpotMaterial = null;
		yield return null;
	}

	private void ResizeTowerSpot_Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Destroy(inHandtowerSpot.gameObject);
			SetState(State.EditorIdle);
			return;
		}

		Vector3 worldPos = cameraController.ScreenToWorld(Input.mousePosition);

		Vector3 dif = worldPos - resizeStartingPos;
		int signX = dif.x < 0 ? -1 : 1, signZ = dif.z < 0 ? -1 : 1;

		int scaleX = Mathf.RoundToInt(Mathf.Abs(dif.x / resizeCellSize));
		int scaleZ = Mathf.RoundToInt(Mathf.Abs(dif.z / resizeCellSize));
		scaleX = Mathf.Abs(scaleX) + 1;
		scaleZ = Mathf.Abs(scaleZ) + 1;

		Vector3 snapPos = new Vector3(resizeCellSize * (scaleX - 1) * signX, 0, resizeCellSize * (scaleZ - 1) * signZ);
		Vector3 middle = resizeStartingPos + snapPos / 2;

		Vector3 scale = resizeStartingScale;
		scale.x *= scaleX;
		scale.z *= scaleZ;

		inHandtowerSpot.transform.position = middle;
		inHandtowerSpot.transform.localScale = scale;
		inHandtowerSpot.UpdateGrid();

		bool available = CheckSpotAvailable(middle, scale);
		inHandTowerSpotMaterial.color = available ? Color.white : Color.red;
		if (available && Input.GetMouseButtonDown(0)) {
			middle.y = towerSpotPrefab.transform.position.y;
			inHandtowerSpot.transform.position = middle;

			inHandtowerSpot = null;

			level.towerSpots.Add(new TowerSpot(new Vector2(middle.x, middle.z), new Vector2Int(scaleX, scaleZ)));

			SetState(State.EditorIdle);
		}
	}

	private bool CheckSpotAvailable(Vector3 spotPos, Vector3 spotScale) {
		Vector2 spotPos2D = new Vector2(spotPos.x, spotPos.z);
		Vector2 spotScale2D = new Vector2(spotScale.x, spotScale.z);
		Rect spotRect = new Rect(spotPos2D - spotScale2D / 2, spotScale2D);

		for (int i = 0; i < level.towerSpots.Count; ++i) {
			Vector2 pos = level.towerSpots[i].position;
			Vector2 scale = level.towerSpots[i].scale;
			scale *= resizeCellSize;
			Rect rect = new Rect(pos - scale / 2, scale);

			if (spotRect.Overlaps(rect)) return false;
		}
		return true;
	}
}
