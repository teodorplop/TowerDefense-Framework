using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct TowerSpot {
	public Vector2 position;
	public Vector2Int scale;

	public TowerSpot(Vector2 position, Vector2Int scale) {
		this.position = position;
		this.scale = scale;
	}
}

[Serializable]
public class TDPath {
	public List<Vector2> waypoints = new List<Vector2>();

	[NonSerialized] private bool dirty = true;
	[NonSerialized] private Vector3[] waypoints3D = null;
	
	public int Count { get { return waypoints.Count; } }

	public Vector2 this[int index] { get { return waypoints[index]; } set { waypoints[index] = value; } }

	public Vector3[] Waypoints3D {
		get {
			if (dirty) {
				if (waypoints3D == null) waypoints3D = new Vector3[waypoints.Count];
				else if (waypoints3D.Length != waypoints.Count) Array.Resize(ref waypoints3D, waypoints.Count);

				for (int i = 0; i < waypoints.Count; ++i)
					waypoints3D[i] = new Vector3(waypoints[i].x, 0.01f, waypoints[i].y);

				dirty = false;
			}

			return waypoints3D;
		}
	}

	public void Add(Vector2 waypoint) {
		waypoints.Add(waypoint);
		dirty = true;
	}
	public void RemoveAt(int index) {
		waypoints.RemoveAt(index);
		dirty = true;
	}
}

public struct SubWave {
	public byte unitId;
	public byte numberOfUnits;
	public float spawnDelay;
	public float spawnInterval;
	public byte pathId;

	public override string ToString() {
		return string.Format("unitId {0}; numberOfUnits {1}; spawnDelay {2}; spawnInterval {3}; pathId {4}", unitId, numberOfUnits, spawnDelay, spawnInterval, pathId);
	}
}

public class Wave {
	public List<SubWave> subwaves = new List<SubWave>();

	public Wave Clone() {
		Wave clone = new Wave();
		clone.subwaves = new List<SubWave>(subwaves);
		return clone;
	}

	public override string ToString() {
		StringBuilder sb = new StringBuilder();

		foreach (var sw in subwaves)
			sb.AppendLine(sw.ToString());

		return sb.ToString();
	}
}

public class LevelFile : IEquatable<LevelFile> {
	[NonSerialized] public string fileName;
	[NonSerialized] public DateTime modified;
	[NonSerialized] public bool isOfficial;

	// TODO: These should not be public.
	public string name = "New Level";
	public short startingCurrency = 0;
	public short startingHealth = 1;
	public List<TowerSpot> towerSpots = new List<TowerSpot>();
	public List<TDPath> paths = new List<TDPath>();
	public List<Wave> waves = new List<Wave>();

	public LevelFile Clone() {
		LevelFile clone = new LevelFile();
		clone.fileName = fileName;
		clone.modified = modified;
		clone.isOfficial = isOfficial;

		clone.name = name;
		clone.startingCurrency = startingCurrency;
		clone.startingHealth = startingHealth;

		clone.towerSpots = new List<TowerSpot>(towerSpots);
		clone.paths = new List<TDPath>(paths);
		clone.waves = new List<Wave>(waves.Count);
		for (int i = 0; i < waves.Count; ++i)
			clone.waves.Add(waves[i].Clone());

		return clone;
	}

	public bool Equals(LevelFile other) {
		return isOfficial == other.isOfficial && fileName == other.fileName;
	}
}
