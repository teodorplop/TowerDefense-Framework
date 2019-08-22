using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowersPanel : MonoBehaviour {
	[SerializeField] private TowersPanelEntry towerPrefab;
	[SerializeField] private TowerInfo towerInfo;

	private Tower.Data[] data;
	private TowersPanelEntry[] towerEntries;
	
	public void Inject(Tower.Data[] data, Sprite[] sprites) {
		this.data = data;

		towerEntries = new TowersPanelEntry[sprites.Length];
		if (sprites.Length > 0) towerPrefab.gameObject.SetActive(true);
		for (int i = 0; i < sprites.Length; ++i) {
			if (i == 0) towerEntries[i] = towerPrefab;
			else towerEntries[i] = Instantiate(towerPrefab, towerPrefab.transform.parent);

			towerEntries[i].Inject(i, sprites[i], OnTowerPressed);
			towerEntries[i].SetOnHover(OnTowerPointerEnter, OnTowerPointerExit);
		}
	}

	public void OnTowerPressed(int index) {
		EventManager.Raise(new TowerButtonEvent(index));
	}

	public void OnTowerPointerEnter(int index) {
		towerInfo.Show(data[index], 0);
	}
	public void OnTowerPointerExit(int index) {
		towerInfo.Hide();
	}
}
