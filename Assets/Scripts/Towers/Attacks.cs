using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Target
public interface IAttackTarget {
	IAttackable GetTarget();
}

public class NoneAttackTarget : IAttackTarget {
	public IAttackable GetTarget() { return null; }
}

public class ClosestAttackTarget : IAttackTarget {
	private IAttackable[] buffer = new IAttackable[64];

	private Tower tower;
	private Transform towerTransform;
	private IAttackable target;

	public ClosestAttackTarget(Tower tower) {
		this.tower = tower;
		towerTransform = tower.transform;
	}

	public IAttackable GetTarget() {
		Vector3 position = towerTransform.position;
		float radius = tower.CurrentLevel.attackRange;

		if (target == null || target.IsDead || target.IsImmune || Vector3.Distance(position, target.Position) > radius)
			target = GetClosest(position, radius);
		return target;
	}

	private IAttackable GetClosest(Vector3 position, float radius) {
		int inRange = UnitScanner.ScanFor(position, radius, buffer);
		if (inRange == 0) return null;

		int closest = 0;
		float minDist = (position - buffer[0].Position).sqrMagnitude;
		for (int i = 1; i < inRange; ++i) {
			float dist = (position - buffer[i].Position).sqrMagnitude;
			if (dist < minDist) {
				minDist = dist;
				closest = i;
			}
		}

		return buffer[closest];
	}
}
#endregion

#region Attack
public interface IAttack {
	void Attack(float deltaTime, IAttackable target);
}

public class AttackCooldown {
	private Tower tower;
	private float cooldown;

	public AttackCooldown(Tower tower) {
		this.tower = tower;
		cooldown = tower.AttackInterval;
	}

	protected bool AttackReady(float deltaTime) {
		bool ready;
		cooldown -= deltaTime;
		if (ready = (cooldown <= 0)) cooldown = tower.AttackInterval;

		return ready;
	}
}

public class SingleTargetAttack : AttackCooldown, IAttack {
	private Tower tower;
	private IOnHitEffect attackAbility;

	public SingleTargetAttack(Tower tower, IOnHitEffect attackAbility) : base(tower) {
		this.tower = tower;
		this.attackAbility = attackAbility;
	}

	public void Attack(float deltaTime, IAttackable target) {
		if (target == null || !AttackReady(deltaTime)) return;
		
		target.ApplyDamage(tower.AttackDamage);
		attackAbility.Apply(target);
	}
}

public class AOEAttack : AttackCooldown, IAttack {
	private IAttackable[] buffer = new IAttackable[64];
	private Tower tower;
	private IOnHitEffect attackAbility;

	public AOEAttack(Tower tower, IOnHitEffect attackAbility) : base(tower) {
		this.tower = tower;
		this.attackAbility = attackAbility;
	}

	public void Attack(float deltaTime, IAttackable target) {
		if (target == null || !AttackReady(deltaTime)) return;

		float damage = tower.AttackDamage;

		target.ApplyDamage(damage);
		attackAbility.Apply(target);

		int count = UnitScanner.ScanFor(target.Position, tower.CurrentLevel.aoeRadius, buffer);
		for (int i = 0; i < count; ++i) {
			float distance = Vector3.Distance(target.Position, buffer[i].Position);
			buffer[i].ApplyDamage(damage / (distance + 1.0f));
			attackAbility.Apply(buffer[i]);
		}
	}
}
#endregion

#region AttackAbility
public interface IOnHitEffect {
	void Apply(IAttackable target);
}

public class NoneAbility : IOnHitEffect {
	public void Apply(IAttackable target) { }
}

public class SlowAbility : IOnHitEffect {
	public void Apply(IAttackable target) {

	}
}

public class StunAbility : IOnHitEffect {
	public void Apply(IAttackable target) {

	}
}
#endregion