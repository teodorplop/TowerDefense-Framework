using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Target
public interface IBuffTarget {
	Tower[] GetTargets();
}

public class NoneBuffTarget : IBuffTarget {
	private Tower[] targets = new Tower[0];
	public Tower[] GetTargets() { return targets; }
}

public class AuraBuffTarget : IBuffTarget {
	private Tower[] buffer = new Tower[64];
	private Tower tower;
	private Transform towerTransform;

	public AuraBuffTarget(Tower tower) {
		this.tower = tower;
		towerTransform = tower.transform;
	}

	public Tower[] GetTargets() {
		int count = TowerScanner.ScanFor(towerTransform.position, tower.CurrentLevel.auraRange, buffer);
		for (int i = count; i < buffer.Length; ++i) buffer[i] = null;

		return buffer;
	}
}
#endregion

#region Buff
public interface IBuff {
	void Apply(Tower[] targets);
}

public class AttackSpeedBuff : IBuff {
	private Tower tower;

	public AttackSpeedBuff(Tower tower) {
		this.tower = tower;
	}

	public void Apply(Tower[] targets) {
		for (int i = 0; i < targets.Length && targets[i]; ++i)
			targets[i].SetAttackSpeedBuff(tower.CurrentLevel.attackSpeedAura);
	}
}

public class AttackDamageBuff : IBuff {
	private Tower tower;

	public AttackDamageBuff(Tower tower) {
		this.tower = tower;
	}

	public void Apply(Tower[] targets) {
		for (int i = 0; i < targets.Length && targets[i]; ++i)
			targets[i].SetAttackSpeedBuff(tower.CurrentLevel.attackDamageAura);
	}
}
#endregion