using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerFactory", menuName = "ScriptableObjects/TowerFactory")]
public class TowerFactory : ScriptableObject {
	private static TowerFactory instance;

	[SerializeField] private Tower[] prefabs;
	[SerializeField] private Sprite[] sprites;
	private Tower.Data[] data;

	private Transform folder;
	
	public Sprite[] Sprites { get { return sprites; } }
	public Tower.Data[] Data { get { return data; } }

	public static int NumberOfTowers { get { return instance.prefabs.Length; } }

	void Awake() {
		if (instance != null) {
			Debug.LogError("TowerFactory: multiple instances.");
			return;
		}

		instance = this;
		data = new Tower.Data[prefabs.Length];
		for (int i = 0; i < data.Length; ++i)
			data[i] = prefabs[i].data;
	}

	public Tower InstantiateTower(int id, Vector3 position) {
		if (id >= prefabs.Length) {
			Debug.LogError("TowerFactory: No prefab for " + id);
			return null;
		}

		if (folder == null) folder = new GameObject("TowerFactory").transform;
		return Instantiate(prefabs[id], position, Quaternion.identity, folder);
	}
	
	public static Sprite GetSprite(int id) {
		if (id >= instance.sprites.Length) {
			Debug.LogWarning("TowerFactory: No sprite for " + id);
			return null;
		}
		return instance.sprites[id];
	}
}
