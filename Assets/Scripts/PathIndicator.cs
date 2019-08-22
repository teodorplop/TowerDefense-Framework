using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PathIndicator : MonoBehaviour {
	[SerializeField] private float duration = 1.0f;
	[SerializeField] private float delay = 1.0f;

	private Transform thisTransform;
	private Vector3[] waypoints;
	private new Sequence animation;

	void Awake() {
		thisTransform = transform;
	}

	public void SetWaypoints(Vector3[] waypoints) {
		this.waypoints = waypoints;
		for (int i = 0; i < waypoints.Length; ++i) waypoints[i].y += 0.05f;

		StartAnimation();
	}

	public void RemoveWaypoint(int idx) {
		for (int i = idx; i < waypoints.Length - 1; ++i) waypoints[i] = waypoints[i + 1];
		Array.Resize(ref waypoints, waypoints.Length - 1); // Not great, recreating the array.

		StartAnimation();
	}

	private void StartAnimation() {
		if (animation != null) animation.Kill();

		thisTransform.position = waypoints[0];

		animation = DOTween.Sequence();
		var tweener = thisTransform.DOPath(waypoints, duration, PathType.Linear, PathMode.Full3D).SetEase(Ease.Linear);

		animation.Append(tweener);
		animation.AppendInterval(delay);
		animation.SetLoops(-1, LoopType.Restart);

		animation.OnStepComplete(delegate {
			// Hack to restart the animation without leaving a trail behind
			gameObject.SetActive(false);
			thisTransform.position = waypoints[0];
			gameObject.SetActive(true);
		});
	}

	void OnDestroy() {
		if (animation != null) animation.Kill(false);
	}
}
