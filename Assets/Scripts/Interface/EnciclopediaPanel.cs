using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnciclopediaPanel : MonoBehaviour {
	[SerializeField] private EnciclopediaEntry entryPrefab;

	private List<EnciclopediaEntry> entries;

	private Sprite[] enemiesSprites = new Sprite[0];
	private Unit.Data[] enemiesData;

	private Sprite[] towersSprites = new Sprite[0];
	private Tower.Data[] towersData;

	void Awake() {
		Init();
	}

	private void Init() {
		if (entries == null) {
			entries = new List<EnciclopediaEntry>();
			entries.Add(entryPrefab);
			entryPrefab.gameObject.SetActive(false);

			entryPrefab.SetOnSelected(delegate { OnEntrySelected(0); });
		}
	}

	public void InjectTowers(Sprite[] sprites, Tower.Data[] data) {
		Init();

		towersSprites = sprites;
		towersData = data;
		InstantiateEntries(sprites.Length);

		OnTowersToggle(true);
	}

	public void InjectEnemies(Sprite[] sprites, Unit.Data[] data) {
		Init();

		enemiesSprites = sprites;
		enemiesData = data;
		InstantiateEntries(sprites.Length);
	}

	private void InstantiateEntries(int count) {
		for (int i = entries.Count; i < count; ++i) {
			entries.Add(Instantiate(entryPrefab, entryPrefab.transform.parent));
			int idx = i;
			entries[i].SetOnSelected(delegate { OnEntrySelected(idx); });
			entries[i].gameObject.SetActive(false);
		}
	}

	public void OnTowersToggle(bool isOn) {
		if (isOn) SetEntries(towersSprites);
	}
	public void OnEnemiesToggle(bool isOn) {
		if (isOn) SetEntries(enemiesSprites);
	}

	private void SetEntries(Sprite[] sprites) {
		for (int i = 0; i < sprites.Length; ++i) {
			entries[i].Inject(sprites[i]);
			entries[i].gameObject.SetActive(true);
		}
		for (int i = sprites.Length; i < entries.Count; ++i)
			entries[i].gameObject.SetActive(false);

		if (sprites.Length > 0) {
			entries[0].ForceSelect(true);
			for (int i = 1; i < entries.Count; ++i) entries[i].ForceSelect(false);
		}
	}

	private void OnEntrySelected(int idx) {
	}
}
