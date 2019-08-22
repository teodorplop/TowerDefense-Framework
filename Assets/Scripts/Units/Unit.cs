using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable {
	Vector3 Position { get; }

	float Health { get; }
	bool IsDead { get; }
	bool IsImmune { get; }
	void ApplyDamage(float damage);
}

public class Unit : MonoBehaviour, IAttackable {
	[Serializable]
	public struct Data {
		public string name;
		public float speed;
		public int maxHealth;
		public short damage;
		public short bounty;
	}

	private Transform thisTransform;
	private PathFollower pathFollower;
	[SerializeField] private Animator animator;
	[SerializeField] private Data data;

	private float health;
	private bool pathFinished = false;

	public Data UnitData { get { return data; } }

	public Vector3 Position { get { return thisTransform.position; } }
	public int MaxHealth { get { return data.maxHealth; } }
	public float Health { get { return health; } }
	public bool IsDead { get { return Health <= 0; } }
	public bool IsImmune { get { return IsDead || pathFinished; } }

	public event Action<Unit> onPathFinished;
	public event Action<Unit> onUnitDied;

	void Awake() {
		thisTransform = transform;
		pathFollower = gameObject.AddComponent<PathFollower>();
		pathFollower.Speed = data.speed;

		health = data.maxHealth;

		if (animator != null)
			animator.SetFloat("MovementSpeed", pathFollower.Speed);
	}

	public void SetWaypoints(Vector3[] waypoints) {
		pathFollower.SetPath(waypoints, OnPathFinished);
	}
	private void OnPathFinished() {
		pathFinished = true;
		animator.SetTrigger("Destination");

		if (onPathFinished != null) onPathFinished(this);
		onPathFinished = null;
	}

	public void ApplyDamage(float damage) {
		health = Mathf.Max(0, health - damage);
		if (IsDead) {
			if (onUnitDied != null) onUnitDied(this);
			onUnitDied = null;

			animator.SetTrigger("Destroyed");
			pathFollower.Stop();
			Destroy(gameObject, 5.0f);
		}
	}
}
