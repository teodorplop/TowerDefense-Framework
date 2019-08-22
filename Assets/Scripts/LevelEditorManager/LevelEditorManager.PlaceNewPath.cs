using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelEditorManager {
	private LineRenderer inHandPathRenderer;
	private List<Transform> inHandWaypoints;
	private Material inHandWaypointMaterial;
	private TDPath inHandPath;

	private IEnumerator PlaceNewPath_EnterState(object input) {
		editorUI.SetGuidanceMessage(@"Left Click: Place waypoint and create new one
Space: Place waypoint and commit path
Left Shift: Free movement
Escape: Cancel");

		inHandWaypoints = new List<Transform>();

		inHandPathRenderer = Instantiate(pathRendererPrefab);
		inHandPathRenderer.transform.parent = pathsParent;
		inHandWaypoints.Add(GameObject.CreatePrimitive(PrimitiveType.Cube).transform);
		inHandWaypoints[0].GetComponent<Collider>().isTrigger = true;
		inHandWaypoints[0].localScale = new Vector3(0.5f, 0.5f, 0.5f);
		inHandWaypoints[0].parent = inHandPathRenderer.transform;
		inHandWaypoints[0].gameObject.layer = pathRendererPrefab.gameObject.layer;
		inHandWaypointMaterial = inHandWaypoints[0].GetComponent<Renderer>().material;

		Vector3 worldPos = cameraController.ScreenToWorld(Input.mousePosition);
		inHandWaypoints[0].position = worldPos + new Vector3(0, 0.3f, 0);

		inHandPathRenderer.positionCount = 1;
		inHandPathRenderer.SetPosition(0, worldPos + new Vector3(0, 0.05f, 0));

		inHandPathRenderer.name = level.paths.Count.ToString();

		level.paths.Add(inHandPath = new TDPath());

		inHandWaypoints[0].name = "0";

		yield return null;
	}
	private IEnumerator PlaceNewPath_ExitState() {
		editorUI.SetGuidanceMessage(string.Empty);

		inHandPathRenderer = null;
		inHandWaypoints = null;
		inHandWaypointMaterial = null;
		inHandPath = null;
		yield return null;
	}

	private void PlaceNewPath_Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			level.paths.RemoveAt(level.paths.Count - 1);
			Destroy(inHandPathRenderer.gameObject);
			inHandWaypoints = null;
			SetState(State.EditorIdle);
			return;
		}

		Vector3 worldPos = cameraController.ScreenToWorld(Input.mousePosition);
		if (!Input.GetKey(KeyCode.LeftShift)) worldPos = worldPos.RoundToInt();
		inHandWaypoints[inHandWaypoints.Count - 1].position = worldPos + new Vector3(0, 0.3f, 0);
		inHandPathRenderer.SetPosition(inHandPathRenderer.positionCount - 1, worldPos + new Vector3(0, 0.05f, 0));

		bool available = CheckSpotAvailable(worldPos);
		inHandWaypointMaterial.color = available ? Color.white : Color.red;
		
		if (available) {
			if (Input.GetMouseButtonDown(0)) {
				inHandPath.Add(new Vector2(worldPos.x, worldPos.z));

				++inHandPathRenderer.positionCount;
				inHandPathRenderer.SetPosition(inHandPathRenderer.positionCount - 1, worldPos);

				inHandWaypoints.Add(Instantiate(inHandWaypoints[0], inHandWaypoints[0].parent));
				inHandWaypoints[inHandWaypoints.Count - 1].name = (inHandWaypoints.Count - 1).ToString();
			} else if (Input.GetKeyDown(KeyCode.Space)) {
				inHandPath.Add(new Vector2(worldPos.x, worldPos.z));

				Vector3[] waypoints = new Vector3[inHandWaypoints.Count];
				for (int i = 0; i < inHandWaypoints.Count; ++i) {
					waypoints[i] = new Vector3(inHandPath[i].x, 0.01f, inHandPath[i].y);
					inHandWaypoints[i].position = waypoints[i] + new Vector3(0, 0.25f, 0);
				}

				inHandPathRenderer.SetPositions(waypoints);
				var pathIndicator = Instantiate(pathIndicatorPrefab, inHandPathRenderer.transform);
				pathIndicator.SetWaypoints(waypoints);

				SetState(State.EditorIdle);
			}
		}
	}
}
