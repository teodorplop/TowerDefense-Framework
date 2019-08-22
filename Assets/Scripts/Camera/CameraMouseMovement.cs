using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraMouseMovement : MonoBehaviour {
	private enum MouseButton { LeftClick, RightClick };

	[SerializeField] private MouseButton mouseButton = MouseButton.LeftClick;
	[SerializeField] private float threshold = 0;

	private Transform thisTransform;
	private new Camera camera;
	private Plane worldPlane;
	
	private Vector3 lastWorldPos;
	private bool panning;

	void Awake() {
		thisTransform = transform;
		camera = GetComponent<Camera>();
		worldPlane = new Plane(Vector3.up, 0);
	}

	void Update() {
		if (MouseOverUI()) {
			lastWorldPos = GetWorldPos(); // Smooth camera transitions when mouse is over ui elements.
			return;
		}

		if (Input.GetMouseButtonDown((int)mouseButton)) {
			lastWorldPos = GetWorldPos();
		} else if (Input.GetMouseButton((int)mouseButton)) {
			Vector3 worldPos = GetWorldPos();
			Vector3 dir = lastWorldPos - GetWorldPos();

			if (!panning && dir.magnitude > threshold) {
				lastWorldPos = worldPos;
				panning = true;
			}

			if (panning)
				thisTransform.position += lastWorldPos - GetWorldPos();
		} else if (Input.GetMouseButtonUp((int)mouseButton)) {
			panning = false;
		}
	}

	private static bool MouseOverUI() {
		return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
	}

	private Vector3 GetWorldPos() {
		Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
		float distance;
		worldPlane.Raycast(mouseRay, out distance);
		return mouseRay.GetPoint(distance);
	}
}
