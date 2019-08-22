using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScanner : BaseScanner<UnitScanner> {
	protected override void Awake() {
		base.Awake();
		EventManager.AddListener<TowerSpawnedEvent>(OnTowerSpawned);
	}
	protected override void OnDestroy() {
		base.OnDestroy();
		EventManager.RemoveListener<TowerSpawnedEvent>(OnTowerSpawned);
	}

	private void OnTowerSpawned(TowerSpawnedEvent evt) {
		transforms.Add(evt.tower.transform);
		evt.tower.onTowerDestroyed += OnTowerDestroyed;
	}
	private void OnTowerDestroyed(Tower tower) {
		transforms.Remove(tower.transform);
	}
}
