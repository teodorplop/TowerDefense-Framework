using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utils {
	public static Vector3Int RoundToInt(this Vector3 v) {
		return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
	}
	public static Vector3Int FloorToInt(this Vector3 v) {
		return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
	}

	public static bool MouseOverUI() {
		return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
	}

	public static T AddMissingComponent<T>(this GameObject obj) where T : Component {
		T component = obj.GetComponent<T>();
		if (component == null) component = obj.AddComponent<T>();
		return component;
	}

	public static void RemovePosition(this LineRenderer line, int index) {
		int count = line.positionCount;
		if (index >= count) { Debug.LogError("Index " + index + " out of line renderer range.", line); return; }

		if (index == count - 1) { --line.positionCount; return; } // OPTIMIZATIONS, HEH! <3

		--count;
		Vector3 lastPos = line.GetPosition(count);
		line.positionCount = count;

		Vector3[] positions = new Vector3[count];
		line.GetPositions(positions);
		for (int i = index; i < count - 1; ++i) positions[i] = positions[i + 1];
		positions[count - 1] = lastPos;

		line.SetPositions(positions);
	}

	public static void SetLayer(this GameObject obj, int layer) {
		if (obj == null) return;

		obj.layer = layer;
		foreach (Transform child in obj.transform)
			child.gameObject.SetLayer(layer);
	}
}

// This class needs LayerAttributeEditor to get the job done.
public class LayerAttribute : PropertyAttribute {}