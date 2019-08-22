using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
	private int currency;
	private int maxHealth;
	private int health;

	public int Currency {
		get { return currency; }
	}

	public int Health {
		get { return health; }
		set {
			health = Mathf.Clamp(value, 0, maxHealth);
			if (onDirty != null) onDirty();
			if (health == 0 && onDied != null) onDied();
		}
	}
	public bool IsDead { get { return health == 0; } }

	public event Action onDirty;
	public event Action onDied;

	public Player(int currency, int health) {
		this.currency = currency;
		this.maxHealth = this.health = health;
	}

	public bool CheckCurrency(int value) {
		return currency >= value;
	}
	public void AddCurrency(int value) {
		if (value >= 0) {
			currency += value;
			if (onDirty != null) onDirty();
		}
	}
	public void SubtractCurrency(int value) {
		if (CheckCurrency(value)) {
			currency -= value;
			if (onDirty != null) onDirty();
		}
	}
}
