using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelEditorManager {
	private EditorUI editorUI;
	private int editorPathHighlighted = -1;

	private IEnumerator EditorIdle_EnterState(object input) {
		editorUI.Set_OnAddTowerSpotButton(delegate { SetState(State.PlaceTowerSpot); });
		editorUI.Set_OnAddPathButton(delegate { SetState(State.PlaceNewPath); });
		EventManager.AddListener<EditorPathHighlightEvent>(OnEditorPathHighlightEvent);

		yield return null;
	}

	private IEnumerator EditorIdle_ExitState() {
		editorUI.Set_OnAddTowerSpotButton(null);
		editorUI.Set_OnAddPathButton(null);
		EventManager.RemoveListener<EditorPathHighlightEvent>(OnEditorPathHighlightEvent);

		yield return null;
	}

	private void EditorIdle_Update() {
		if (Utils.MouseOverUI()) return;

		MouseSelection();
	}

	private void MouseSelection() {
		if (Input.GetMouseButtonDown(0)) {
			var selection = cameraController.Scan(Input.mousePosition, towerLayer);
			if (selection.HasValue) { SetState(State.SelectedTowerSpot, selection.Value.hitObject); return; }

			selection = cameraController.Scan(Input.mousePosition, pathLayer);
			if (selection.HasValue) { SetState(State.SelectedWaypoint, selection.Value.hitObject); return; }

			SetState(State.EditorIdle);
		}
	}
	
	private void OnEditorPathHighlightEvent(EditorPathHighlightEvent evt) {
		if (evt.pathIndex == -1) {
			if (editorPathHighlighted != -1) {
				glowEffect.Disable(pathsParent.GetChild(editorPathHighlighted).gameObject);
				editorPathHighlighted = -1;
			}
			return;
		}

		TDPath path = level.paths[evt.pathIndex];
		Vector2 middle = Vector2.zero;
		for (int i = 0; i < path.Count; ++i) middle += path[i];
		middle /= path.Count;

		float xOffset = 5.0f * Screen.width / Screen.height; // Offset a bit from the center of the view, as it's obstructed by the menu.
		cameraController.FocusOn(new Vector3(middle.x - xOffset, 0, middle.y), 25);

		glowEffect.Enable(pathsParent.GetChild(editorPathHighlighted = evt.pathIndex).gameObject);
	}
}
