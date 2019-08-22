using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {
	[SerializeField] private float magnitude = 0.05f;
	[SerializeField] private float frequency = 3f;
	private float offset;

	private Transform thisTransform;
	private float anchor;

	void Start() {
		thisTransform = transform;
		anchor = thisTransform.localPosition.y;
		offset = Random.Range(0, Mathf.PI);
		frequency += Random.Range(-.5f, 0.5f);
	}
	
	void Update() {
		float hover = magnitude * (1 + Mathf.Sin(Time.time * frequency + offset));
		thisTransform.localPosition = new Vector3(thisTransform.localPosition.x, anchor + hover, thisTransform.localPosition.z);
	}
}
