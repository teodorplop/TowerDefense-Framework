using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelLoader : MonoBehaviour {
	[SerializeField] private TowerSpotGrid towerSpotPrefab;
	[SerializeField] private PathIndicator pathIndicatorPrefab;
	[SerializeField] private LineRenderer pathRendererPrefab;

	private Transform towerSpotParent;
	private Transform pathsParent;

	public Transform TowerSpotParent { get { return towerSpotParent; } }
	public Transform PathsParent { get { return pathsParent; } }

	public IEnumerator LoadLevel(LevelFile level, bool addWaypoints = false) {
		towerSpotParent = new GameObject("TowerSpots").transform;
		for (int i = 0; i < level.towerSpots.Count; ++i) {
			Vector2 position = level.towerSpots[i].position;
			Vector2 scale = level.towerSpots[i].scale;
			scale *= towerSpotPrefab.CellSize;

			Transform towerSpot = Instantiate(towerSpotPrefab, towerSpotParent).transform;
			Vector3 localPosition = towerSpot.localPosition;
			Vector3 localScale = towerSpot.localScale;
			localPosition = new Vector3(position.x, localPosition.y, position.y);
			localScale = new Vector3(scale.x, localScale.y, scale.y);

			towerSpot.localPosition = localPosition;
			towerSpot.localScale = localScale;

			yield return null;
		}

		pathsParent = new GameObject("Paths").transform;
		for (int i = 0; i < level.paths.Count; ++i) {
			LineRenderer pathRenderer = Instantiate(pathRendererPrefab, pathsParent);
			PathIndicator pathIndicator = Instantiate(pathIndicatorPrefab, pathRenderer.transform);

			pathRenderer.name = i.ToString();

			Vector3[] waypoints = level.paths[i].Waypoints3D;

			if (addWaypoints) yield return StartCoroutine(AddWaypoints(pathRenderer.transform, waypoints));

			pathRenderer.positionCount = waypoints.Length;
			pathRenderer.SetPositions(waypoints);
			
			pathIndicator.SetWaypoints(waypoints);

			yield return null;
		}
	}

	private IEnumerator AddWaypoints(Transform parent, Vector3[] waypoints) {
		for (int i = 0; i < waypoints.Length; ++i) {
			Transform wp = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
			wp.name = i.ToString();
			wp.GetComponent<Collider>().isTrigger = true;
			wp.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			wp.parent = parent;
			wp.SetSiblingIndex(i);
			wp.gameObject.layer = pathRendererPrefab.gameObject.layer;
			wp.position = waypoints[i] + new Vector3(0, 0.25f, 0);

			yield return null;
		}
	}
}
