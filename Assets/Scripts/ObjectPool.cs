using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component {
	private T prefab;
	private List<T> busy, free;

	public ObjectPool(T prefab) {
		busy = new List<T>();
		free = new List<T>();

		SetFree(this.prefab = prefab);
	}

	public T GetFree() {
		T obj = null;
		if (free.Count == 0) obj = Object.Instantiate(prefab, prefab.transform.parent);
		else obj = free[0];

		SetBusy(obj);
		return obj;
	}

	public void SetFree(T obj) {
		busy.Remove(obj);
		free.Add(obj);
		obj.gameObject.SetActive(false);
	}

	private void SetBusy(T obj) {
		free.Remove(obj);
		busy.Add(obj);
		obj.gameObject.SetActive(true);
	}
}
