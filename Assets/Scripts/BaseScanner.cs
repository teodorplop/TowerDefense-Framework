using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScanner<T> : MonoBehaviour where T : BaseScanner<T> {
	private static T instance;
	protected List<Transform> transforms;

	protected virtual void Awake() {
		if (instance != null) {
			Destroy(this);
			return;
		}

		instance = this as T;
		transforms = new List<Transform>();
	}
	protected virtual void OnDestroy() {
		if (instance == this)
			instance = null;
	}

	public static List<Transform> ScanFor(Vector3 position, float radius) {
		radius *= radius;

		List<Transform> units = new List<Transform>();
		foreach (var unit in instance.transforms)
			if ((position - unit.position).sqrMagnitude <= radius) units.Add(unit);

		return units;
	}
	public static int ScanFor(Vector3 position, float radius, Transform[] buffer) {
		radius *= radius;

		int count = 0;
		foreach (var item in instance.transforms)
			if ((position - item.position).sqrMagnitude <= radius) buffer[count++] = item;

		return count;
	}

	public static List<U> ScanFor<U>(Vector3 position, float radius) {
		radius *= radius;

		List<U> units = new List<U>();
		foreach (var item in instance.transforms)
			if ((position - item.position).sqrMagnitude <= radius) units.Add(item.GetComponent<U>());

		return units;
	}
	public static int ScanFor<U>(Vector3 position, float radius, U[] buffer) {
		radius *= radius;

		int count = 0;
		foreach (var items in instance.transforms)
			if ((position - items.position).sqrMagnitude <= radius) buffer[count++] = items.GetComponent<U>();

		return count;
	}
}
