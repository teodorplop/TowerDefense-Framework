using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour {
	[SerializeField] private TMP_Text currencyText;
	[SerializeField] private TMP_Text healthText;

	private Player player;

	public void Inject(Player player) {
		this.player = player;
		OnPlayerDirty();

		player.onDirty += OnPlayerDirty;
		player.onDied += OnPlayerDied;
	}

	void OnDestroy() {
		if (player != null) {
			player.onDirty -= OnPlayerDirty;
			player.onDied -= OnPlayerDied;
			player = null;
		}
	}

	private void OnPlayerDied() {
		player = null;
	}

	private void OnPlayerDirty() {
		currencyText.text = player.Currency.ToString();
		healthText.text = player.Health.ToString();
	}
}
