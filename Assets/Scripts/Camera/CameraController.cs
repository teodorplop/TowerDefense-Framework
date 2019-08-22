using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public struct MouseHit {
	public GameObject hitObject;
	public Vector3 hitPoint;

	public MouseHit(GameObject hitObject, Vector3 hitPoint) {
		this.hitObject = hitObject;
		this.hitPoint = hitPoint;
	}

	public static implicit operator GameObject(MouseHit mouseHit) {
		return mouseHit.hitObject;
	}
}

public struct MouseHit<T> {
	public T hitObject;
	public Vector3 hitPoint;

	public MouseHit(T hitObject, Vector3 hitPoint) {
		this.hitObject = hitObject;
		this.hitPoint = hitPoint;
	}

	public static implicit operator T(MouseHit<T> mouseHit) {
		return mouseHit.hitObject;
	}
}

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
	private Transform thisTransform;
	private new Camera camera;
	private Plane worldPlane = new Plane(new Vector3(0, 1, 0), 0);

	public Camera Camera { get { return camera; } }

	void Awake() {
		thisTransform = transform;
		camera = GetComponent<Camera>();
	}

	public Vector3 ScreenToWorld(Vector3 mousePosition) {
		Ray mouseRay = camera.ScreenPointToRay(mousePosition);
		float distance;
		worldPlane.Raycast(mouseRay, out distance);
		return mouseRay.GetPoint(distance);
	}

	public MouseHit? Scan(Vector3 mousePosition) {
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(mousePosition);
		if (!Physics.Raycast(ray, out hit, camera.farClipPlane)) return null;
		return new MouseHit(hit.collider.gameObject, hit.point);
	}
	public MouseHit? Scan(Vector3 mousePosition, LayerMask layer) {
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(mousePosition);
		if (!Physics.Raycast(ray, out hit, camera.farClipPlane, layer)) return null;
		return new MouseHit(hit.collider.gameObject, hit.point);
	}

	public MouseHit<T>? ScanFor<T>(Vector3 mousePosition) where T : Component {
		MouseHit? obj = Scan(mousePosition);
		if (obj == null) return null;
		return new MouseHit<T>(obj.Value.hitObject.GetComponent<T>(), obj.Value.hitPoint);
	}
	public MouseHit<T>? ScanFor<T>(Vector3 mousePosition, LayerMask layer) where T : Component {
		MouseHit? obj = Scan(mousePosition, layer);
		if (obj == null) return null;
		return new MouseHit<T>(obj.Value.hitObject.GetComponent<T>(), obj.Value.hitPoint);
	}

	public void FocusOn(Vector3 position, float distance) {
		position -= thisTransform.forward * distance;
		thisTransform.DOMove(position, 0.5f).SetAutoKill(true);
	}
}
