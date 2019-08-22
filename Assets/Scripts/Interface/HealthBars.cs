using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBars : MonoBehaviour {
	[SerializeField] private ProgressBar healthBarPrefab;
	[SerializeField] private Vector3 offset;

	private ObjectPool<ProgressBar> healthBarsPool;
	private Dictionary<Unit, ProgressBar> healthBars;

	void Awake() {
		healthBarsPool = new ObjectPool<ProgressBar>(healthBarPrefab);
		healthBars = new Dictionary<Unit, ProgressBar>();
		EventManager.AddListener<UnitSpawnedEvent>(OnUnitSpawned);
	}
	void OnDestroy() {
		EventManager.RemoveListener<UnitSpawnedEvent>(OnUnitSpawned);
	}

	private void OnUnitSpawned(UnitSpawnedEvent evt) {
		healthBars.Add(evt.unit, healthBarsPool.GetFree());

		evt.unit.onPathFinished += OnUnitGone;
		evt.unit.onUnitDied += OnUnitGone;
	}
	private void OnUnitGone(Unit unit) {
		healthBarsPool.SetFree(healthBars[unit]);
		healthBars.Remove(unit);
	}

	void LateUpdate() {
		foreach (var hb in healthBars) {
			hb.Value.Progress = hb.Key.Health / hb.Key.MaxHealth;
			hb.Value.transform.position = hb.Key.transform.position + offset;
		}
	}
}
