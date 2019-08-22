using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SingleWaveSpawner))]
public class WavesSpawner : MonoBehaviour {
	private SingleWaveSpawner singleWaveSpawner;

	private UnitFactory unitFactory;

	private List<Wave> waves;
	private List<TDPath> paths;
	private int spawnedIndex, finishedIndex;

	public event Action<int> onSingleSpawned, onSingleFinished;
	public event Action onSpawned, onFinished;

	void Awake() {
		singleWaveSpawner = GetComponent<SingleWaveSpawner>();
	}
	void OnDestroy() {
		onSingleSpawned = null;
		onSingleFinished = null;
		onSpawned = null;
		onFinished = null;
	}

	public void Inject(UnitFactory unitFactory, List<TDPath> paths) {
		singleWaveSpawner.Inject(this.unitFactory = unitFactory, this.paths = paths);
	}

	public void Spawn(List<Wave> waves) {
		this.waves = waves;
		singleWaveSpawner.Spawn(waves[spawnedIndex = finishedIndex = 0], OnWaveSpawned, OnWaveFinished);
	}
	
	public void SpawnNext() {
		singleWaveSpawner.Spawn(waves[finishedIndex], OnWaveSpawned, OnWaveFinished);
	}

	private void OnWaveSpawned() {
		if (onSingleSpawned != null) onSingleSpawned(spawnedIndex);

		++spawnedIndex;
		if (spawnedIndex == waves.Count) {
			if (onSpawned != null) onSpawned();
		} else {
			
		}
	}
	private void OnWaveFinished() {
		if (onSingleFinished != null) onSingleFinished(finishedIndex);

		++finishedIndex;
		if (finishedIndex == waves.Count) {
			if (onFinished != null) onFinished();
		} else {
			//singleWaveSpawner.Spawn(waves[finishedIndex], OnWaveSpawned, OnWaveFinished);
		}
	}
}
