using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitFactory", menuName = "ScriptableObjects/UnitFactory")]
public class UnitFactory : ScriptableObject {
	private static UnitFactory instance;

	[SerializeField] private Unit[] prefabs;
	[SerializeField] private Sprite[] sprites;
	private Unit.Data[] data;

	private Transform folder;

	// TODO: Don't send the actual references.
	public Sprite[] Sprites { get { return sprites; } }
	public Unit.Data[] Data { get { return data; } }

	public static int NumberOfUnits { get { return instance.prefabs.Length; } }

	void Awake() {
		if (instance != null) {
			Debug.LogError("UnitFactory: multiple instances.");
			return;
		}

		instance = this;

		data = new Unit.Data[prefabs.Length];
		for (int i = 0; i < prefabs.Length; ++i)
			data[i] = prefabs[i].UnitData;
	}

	public Unit InstantiateUnit(int id) {
		if (id >= prefabs.Length) {
			Debug.LogError("UnitFactory: No prefab for " + id);
			return null;
		}

		if (folder == null) folder = new GameObject("UnitFactory").transform;
		return Instantiate(prefabs[id], folder);
	}
	public static Sprite GetSprite(int id) {
		if (id >= instance.sprites.Length) {
			Debug.LogWarning("UnitFactory: No sprite for " + id);
			return null;
		}
		return instance.sprites[id];
	}
	public static Unit.Data GetData(int id) {
		if (id >= instance.data.Length) {
			Debug.LogWarning("UnitFactory: No data for " + id);
			return default(Unit.Data);
		}
		return instance.data[id];
	}
}
