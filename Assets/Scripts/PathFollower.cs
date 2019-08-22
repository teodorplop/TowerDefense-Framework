using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEditor;

public class PathFollower : MonoBehaviour {
	[SerializeField] private float moveSpeed = 1.0f;
	[SerializeField] private float rotateSpeed = 20.0f;
	[SerializeField] private Vector3[] waypoints;

	private Transform thisTransform;
	private Action callback;
	private int idx;

	public float Speed { get { return moveSpeed; } set { moveSpeed = value; } }
	
	void Awake() {
		thisTransform = transform;
	}

	public void SetPath(Vector3[] waypoints, Action callback) {
		this.waypoints = waypoints;
		this.callback = callback;

		thisTransform.position = waypoints[0];
		if (waypoints.Length > 1) thisTransform.rotation = Quaternion.LookRotation(waypoints[1] - thisTransform.position);

		SetWaypoint(1);

		// TODO: Should I use this, does it give enough control?
		//Tween path = transform.DOPath(waypoints, moveSpeed, PathType.Linear, PathMode.Full3D).SetEase(Ease.Linear).SetSpeedBased(true).SetLookAt(0.01f).SetAutoKill(true);
		//path.OnComplete(delegate { if (callback != null) callback(); });
	}
	public void Stop() {
		waypoints = null;
	}

	private bool SetWaypoint(int idx) {
		this.idx = idx;

		if (idx >= waypoints.Length) {
			if (callback != null) callback();

			waypoints = null;
			callback = null;
			return false;
		}

		return true;
	}

	void Update() {
		if (waypoints == null) return;

		float dt = Time.deltaTime;
		while (dt >= Mathf.Epsilon) {
			Vector3 targetVector = waypoints[idx] - thisTransform.position;
			float distance = targetVector.magnitude;
			
			Quaternion targetRotation = Quaternion.LookRotation(targetVector);
			thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, targetRotation, dt * rotateSpeed);

			if (dt * moveSpeed >= distance) { // make sure we don't pass waypoints
				thisTransform.position = waypoints[idx];
				if (!SetWaypoint(idx + 1)) return;

				dt -= distance / moveSpeed;
			} else {
				thisTransform.Translate(Vector3.forward * dt * moveSpeed);
				dt = 0;
			}
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (waypoints == null) return;

		for (int i = 0; i < waypoints.Length - 1; ++i)
			Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);

		for (int i = 0; i < waypoints.Length; ++i)
			Gizmos.DrawSphere(waypoints[i], 0.1f);
	}
#endif
}
