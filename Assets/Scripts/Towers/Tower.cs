using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
	[Serializable]
	public struct Level {
		public ushort cost;

		public float attackRange;
		public float attackDamage;
		public float attackInterval;
		public float aoeRadius;

		public float slowPercentage;
		public float slowDuration;
		public float stunChance;
		public float stunDuration;

		public float auraRange;
		public float attackSpeedAura;
		public float attackDamageAura;
	}
	[Serializable]
	public struct Data {
		public string towerName;
		public AttackTarget attackTarget;
		public AttackType attackType;
		public OnHitEffect onHitEffect;
		public BuffTarget buffTarget;
		public BuffType buffType;
		public List<Level> levels;
	}

	public enum AttackTarget { None, Closest }
	public enum AttackType { SingleTarget, AOE }
	public enum OnHitEffect { None, Slow, Stun }
	public enum BuffTarget { None, Aura }
	public enum BuffType { AttackSpeed, AttackDamage }

	[SerializeField] public Data data;
	[SerializeField] public Transform turret;
	
	private int levelIdx = 0;
	private int totalPrice = 0;
	public int LevelIdx { get { return levelIdx; } }
	public Level CurrentLevel { get { return data.levels[levelIdx]; } }
	public bool Upgradeable { get { return levelIdx + 1 < data.levels.Count; } }
	public Level NextLevel { get { return data.levels[levelIdx + 1]; } }
	public int SellPrice { get { return (totalPrice + 1) >> 1; } }

	private float attackSpeedBuff = 0.0f;
	public float AttackInterval { get { return CurrentLevel.attackInterval * (1.0f - attackSpeedBuff); } }

	private float attackDamageBuff = 0.0f;
	public float AttackDamage { get { return CurrentLevel.attackDamage * (1.0f + attackDamageBuff); } }

	private IAttackTarget iAttackTarget;
	private IAttack iAttack;
	private IOnHitEffect iOnHitEffect;
	private IBuffTarget iBuffTarget;
	private IBuff iBuff;

	public event Action<Tower> onTowerDestroyed;

	void Awake() {
		totalPrice = data.levels[0].cost;
		Initialize();
	}
	void OnDestroy() {
		if (onTowerDestroyed != null) {
			onTowerDestroyed(this);
			onTowerDestroyed = null;
		}
	}

	// TODO: Is it worth to use reflection on this one?
	private void Initialize() {
		switch (data.attackTarget) {
			case AttackTarget.Closest:
				iAttackTarget = new ClosestAttackTarget(this);
				break;
			default:
				iAttackTarget = new NoneAttackTarget();
				break;
		}

		switch (data.onHitEffect) {
			case OnHitEffect.Slow:
				iOnHitEffect = new SlowAbility();
				break;
			case OnHitEffect.Stun:
				iOnHitEffect = new StunAbility();
				break;
			default:
				iOnHitEffect = new NoneAbility();
				break;
		}

		switch (data.attackType) {
			case AttackType.SingleTarget:
				iAttack = new SingleTargetAttack(this, iOnHitEffect);
				break;
			case AttackType.AOE:
				iAttack = new AOEAttack(this, iOnHitEffect);
				break;
		}

		switch (data.buffTarget) {
			case BuffTarget.Aura:
				iBuffTarget = new AuraBuffTarget(this);
				break;
			default:
				iBuffTarget = new NoneBuffTarget();
				break;
		}

		switch (data.buffType) {
			case BuffType.AttackDamage:
				iBuff = new AttackDamageBuff(this);
				break;
			case BuffType.AttackSpeed:
				iBuff = new AttackSpeedBuff(this);
				break;
		}
	}

	public void Upgrade() {
		if (Upgradeable)
			totalPrice += data.levels[++levelIdx].cost;
	}

	void Update() {
		var target = iAttackTarget.GetTarget();
		AimAt(target);

		iAttack.Attack(Time.deltaTime, target);
		
		iBuff.Apply(iBuffTarget.GetTargets());
	}

	private void AimAt(IAttackable target) {
		Quaternion targetRotation = target != null ? Quaternion.LookRotation(target.Position - turret.position) : Quaternion.identity;
		turret.rotation = Quaternion.Slerp(turret.rotation, targetRotation, Time.deltaTime * 20);
	}

	public void SetAttackSpeedBuff(float value) {
		attackSpeedBuff = Mathf.Min(value, 0.95f);
	}
	public void SetAttackDamageBuff(float value) {
		attackDamageBuff = value;
	}
}
