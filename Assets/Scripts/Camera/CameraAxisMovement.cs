using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAxisMovement : MonoBehaviour {
	[SerializeField] private float speed = 1;

	private Transform thisTransform;

	private Vector3 lastMousePosition;
	private bool panning;

	void Awake() {
		thisTransform = transform;
	}

	void Update() {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		thisTransform.position += new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
	}
}
