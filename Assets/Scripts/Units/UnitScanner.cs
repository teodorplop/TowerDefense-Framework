using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScanner : BaseScanner<UnitScanner> {
	protected override void Awake() {
		base.Awake();
		EventManager.AddListener<UnitSpawnedEvent>(OnUnitSpawned);
	}
	protected override void OnDestroy() {
		base.OnDestroy();
		EventManager.RemoveListener<UnitSpawnedEvent>(OnUnitSpawned);
	}

	private void OnUnitSpawned(UnitSpawnedEvent evt) {
		transforms.Add(evt.unit.transform);

		evt.unit.onUnitDied += OnUnitRemoved;
		evt.unit.onPathFinished += OnUnitRemoved;
	}
	private void OnUnitRemoved(Unit unit) {
		transforms.Remove(unit.transform);
	}
}
