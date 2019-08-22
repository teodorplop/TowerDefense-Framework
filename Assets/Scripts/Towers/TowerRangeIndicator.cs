using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRangeIndicator : MonoBehaviour {
	[SerializeField] private Renderer attackRangeRenderer;
	[SerializeField] private Renderer auraRangeRenderer;

	private Transform attackRangeTr;
	private Transform auraRangeTr;

	void Awake() {
		attackRangeTr = attackRangeRenderer.transform;
		auraRangeTr = auraRangeRenderer.transform;

		Disable();
	}

	public void Enable(Vector3 worldPos, float attackRange, float auraRange) {
		auraRangeTr.position = attackRangeTr.position = worldPos;
		auraRangeTr.localScale = new Vector3(auraRange, auraRange, 1);
		attackRangeTr.localScale = new Vector3(attackRange, attackRange, 1);
	}

	public void Disable() {
		auraRangeTr.position = attackRangeTr.position = new Vector3(0, -10000, 0);
	}
}
