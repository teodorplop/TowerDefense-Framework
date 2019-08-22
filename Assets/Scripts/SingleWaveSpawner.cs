using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleWaveSpawner : MonoBehaviour {
	private struct SingleSpawn : IComparable<SingleSpawn> {
		public byte unitId;
		public float time;
		public byte pathId;

		public int CompareTo(SingleSpawn other) {
			return time.CompareTo(other.time);
		}
	}

	private class WaveSpawn {
		private SingleSpawn[] spawns;
		private int index;
		private float timer = 0.0f;
		private int unitFinished;

		public Action onWaveSpawned, onWaveFinished;

		public bool Spawned { get { return index >= spawns.Length; } }
		public bool Finished { get { return unitFinished >= spawns.Length; } }

		public WaveSpawn(Wave wave, Action onWaveSpawned, Action onWaveFinished) {
			this.onWaveSpawned = onWaveSpawned;
			this.onWaveFinished = onWaveFinished;
			
			int noSpawns = 0;
			foreach (var sw in wave.subwaves) noSpawns += sw.numberOfUnits;

			spawns = new SingleSpawn[noSpawns];
			int index = 0;
			foreach (var sw in wave.subwaves)
				for (int i = 0; i < sw.numberOfUnits; ++i)
					spawns[index++] = new SingleSpawn() { unitId = sw.unitId, time = sw.spawnDelay + sw.spawnInterval * i, pathId = sw.pathId };
			this.index = 0;
			unitFinished = 0;

			Array.Sort(spawns);
		}

		public void Advance(float time) {
			timer += time;
		}

		public bool NextSpawn(ref byte unitId, ref byte pathId) {
			if (index >= spawns.Length || spawns[index].time >= timer) return false;
			unitId = spawns[index].unitId;
			pathId = spawns[index].pathId;
			++index;
			return true;
		}

		public void UnitFinished() {
			++unitFinished;
			if (Finished && onWaveFinished != null) onWaveFinished();
		}
	}

	private UnitFactory unitFactory;
	private List<TDPath> paths;
	private List<WaveSpawn> waves;

	void Awake() {
		waves = new List<WaveSpawn>();
	}

	public void Inject(UnitFactory unitFactory, List<TDPath> paths) {
		this.unitFactory = unitFactory;
		this.paths = paths;
	}

	public void Spawn(Wave wave, Action onWaveSpawned, Action onWaveFinished) {
		waves.Add(new WaveSpawn(wave, onWaveSpawned, onWaveFinished));
	}

	void Update() {
		Unit unit;
		foreach (var w in waves) {
			w.Advance(Time.deltaTime);

			byte unitId = 0, pathId = 0;
			bool nextSpawn = w.NextSpawn(ref unitId, ref pathId);
			while (nextSpawn) {
				unit = unitFactory.InstantiateUnit(unitId);
				unit.SetWaypoints(paths[pathId].Waypoints3D);
				unit.onPathFinished += delegate { w.UnitFinished(); };
				unit.onUnitDied += delegate { w.UnitFinished(); } ;

				EventManager.Raise(new UnitSpawnedEvent(unit));

				nextSpawn = w.NextSpawn(ref unitId, ref pathId);
			}

			if (w.Spawned) if (w.onWaveSpawned != null) w.onWaveSpawned();
		}

		waves.RemoveAll(w => w.Spawned);
	}
}
