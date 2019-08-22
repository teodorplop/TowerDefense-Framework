using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToScreen : MonoBehaviour {
	[SerializeField] private Vector3 offset = Vector3.zero;
	
	private RectTransform thisTransform;

	private Transform target;
	private new Camera camera;

	void Awake() {
		thisTransform = transform as RectTransform;
		gameObject.SetActive(false);
	}

	public void SetTarget(Transform target, Camera camera) {
		this.target = target;
		this.camera = camera;

		gameObject.SetActive(target != null);
	}

	void LateUpdate() {
		if (target != null) {
			Vector3 worldPos = target.position + offset;
			thisTransform.position = camera.WorldToScreenPoint(worldPos);
		}
	}
}
