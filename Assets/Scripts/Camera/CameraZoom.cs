using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {
	[SerializeField] private float minZoom = 0.0f;
	[SerializeField] private float maxZoom = 100.0f;
	[SerializeField] private float speed = 1;
	[SerializeField] private float smoothTime = 0.3f;

	private Transform thisTransform;
	private float velocity;
	private float currentVelocity;

	void Awake() {
		thisTransform = transform;
	}

	void Update() {
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0.0f) {
			velocity = -scroll * speed;
			currentVelocity = 0;
		}

		if (Mathf.Abs(velocity) > 0.05f) {
			float height = Mathf.Clamp(thisTransform.position.y + velocity * Time.deltaTime, minZoom, maxZoom);
			thisTransform.position = new Vector3(thisTransform.position.x, height, thisTransform.position.z);

			velocity = Mathf.SmoothDamp(velocity, 0.0f, ref currentVelocity, smoothTime);
		}
	}
}
