using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelEditorManager {
	private GameObject selectedWaypoint;
	private LineRenderer selectedPathRenderer;
	private PathIndicator selectedPathIndicator;
	private int selectedPathIdx;
	private int selectedWaypointIdx;
	
	private IEnumerator SelectedWaypoint_EnterState(object input) {
		selectedWaypoint = (GameObject)input;
		selectedPathRenderer = selectedWaypoint.transform.parent.GetComponent<LineRenderer>();
		selectedPathIndicator = selectedPathRenderer.transform.GetComponentInChildren<PathIndicator>();

		selectedPathIdx = selectedPathRenderer.transform.GetSiblingIndex();
		selectedWaypointIdx = selectedWaypoint.transform.GetSiblingIndex();

		glowEffect.Enable(selectedWaypoint);

		yield return null;
	}
	private IEnumerator SelectedWaypoint_ExitState() {
		if (selectedWaypoint != null) {
			glowEffect.Disable(selectedWaypoint);
			selectedWaypoint = null;
		}
		if (selectedPathRenderer != null) {
			glowEffect.Disable(selectedPathRenderer.gameObject);
			selectedPathRenderer = null;
		}
		yield return null;
	}

	private void SelectedWaypoint_Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SetState(State.EditorIdle);
			return;
		}

		bool selectEntirePath = Input.GetKey(KeyCode.LeftShift);
		if (selectEntirePath) glowEffect.Enable(selectedPathRenderer.gameObject);
		else glowEffect.Disable(selectedPathRenderer.gameObject);

		if (Input.GetKeyDown(KeyCode.Delete)) {
			if (selectedPathRenderer.positionCount == 2) selectEntirePath = true;

			if (selectEntirePath) {
				level.paths.RemoveAt(selectedPathIdx);
				Destroy(selectedPathRenderer.gameObject);
			} else {
				level.paths[selectedPathIdx].RemoveAt(selectedWaypointIdx);
				Destroy(selectedWaypoint);
				
				selectedPathRenderer.RemovePosition(selectedWaypointIdx);
				selectedPathIndicator.RemoveWaypoint(selectedWaypointIdx);
			}

			SetState(State.EditorIdle);
			return;
		}

		MouseSelection();
	}
}
