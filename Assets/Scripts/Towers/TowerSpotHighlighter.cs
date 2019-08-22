using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpotHighlighter : MonoBehaviour {
	[SerializeField] private TowerSpotGrid towerSpotGridPrefab;
	[SerializeField] private Renderer highlightObject;

	private Transform highlightTransform;
	private Material mat;

	void Awake() {
		highlightTransform = highlightObject.transform;
		mat = highlightObject.material;

		Disable();
	}

	void Start() {
		highlightTransform.localScale = new Vector3(towerSpotGridPrefab.CellSize, towerSpotGridPrefab.CellSize, 1);
	}

	public void Enable(Vector3 worldPos, Color32 color) {
		worldPos.y += 0.02f;
		highlightTransform.position = worldPos;

		mat.color = color;
	}

	public void Disable() {
		highlightTransform.position = new Vector3(0, -10000, 0);
	}
}
