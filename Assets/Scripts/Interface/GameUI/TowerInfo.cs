using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using DG.Tweening;

// TODO: ugly class, but we're in a hurry!
public class TowerInfo : MonoBehaviour {
	[SerializeField] private DOTweenAnimation showAnimation;
	[Space(10), SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_Text costText;
	[SerializeField] private TMP_Text detailsText;
	[SerializeField] private TMP_Text sellText;
	[Space(10), SerializeField] private GameObject sellButton;
	[SerializeField] private GameObject upgradeButton;

	private string upgColor;
	private string badColor;

	private bool visible = false;
	private Tower tower;

	void Awake() {
		gameObject.SetActive(false);
		upgColor = "#" + ColorUtility.ToHtmlStringRGB(Color.green);
		badColor = "#" + ColorUtility.ToHtmlStringRGB(Color.red);
	}

	public void Show(Tower tower) {
		this.tower = tower;
		if (tower == null) { Hide(); return; }
		
		sellText.text = "Sell: " + tower.SellPrice;

		Show(tower.data, tower.LevelIdx, tower.Upgradeable);
	}

	public void Show(Tower.Data tower, int level, bool showUpgrade = false) {
		visible = true;

		int cost = showUpgrade ? tower.levels[level + 1].cost : tower.levels[level].cost;

		titleText.text = tower.towerName;
		costText.text = level == tower.levels.Count - 1 ? string.Empty : "Cost: " + cost;
		detailsText.text = GetAttackText(tower, level, showUpgrade) + '\n' + GetBuffText(tower, level, showUpgrade);
		
		upgradeButton.SetActive(showUpgrade);
		sellButton.SetActive(level > 0);
		sellText.gameObject.SetActive(level > 0);

		gameObject.SetActive(true);
		showAnimation.DOPlayForward();
	}

	public void Hide() {
		this.tower = null;
		visible = false;

		showAnimation.DOPlayBackwards();
	}

	private string GetAttackText(Tower.Data tower, int level, bool showUpgrade) {
		if (tower.attackTarget == Tower.AttackTarget.None)
			return "No attack";

		string dmgUpgText = string.Empty, intUpgTex = string.Empty, rangeUpgText = string.Empty;
		if (showUpgrade && level + 1 < tower.levels.Count) {
			dmgUpgText = GetUpgText(tower.levels[level + 1].attackDamage - tower.levels[level].attackDamage);
			intUpgTex = GetUpgText(tower.levels[level + 1].attackInterval - tower.levels[level].attackInterval, false);
			rangeUpgText = GetUpgText(tower.levels[level + 1].attackRange - tower.levels[level].attackRange);
		}

		return string.Format("Attack Target: Closest\nAttack Type: {0}\nOn Hit Effect: {1}\nDamage: {2} {3}\nInterval: {4} {5}\nRange: {6} {7}", 
			tower.attackType, tower.onHitEffect, 
			tower.levels[level].attackDamage, dmgUpgText, 
			tower.levels[level].attackInterval, intUpgTex,
			tower.levels[level].attackRange, rangeUpgText);
	}
	private string GetBuffText(Tower.Data tower, int level, bool showUpgrade) {
		if (tower.buffTarget == Tower.BuffTarget.None)
			return "No buff";

		string percUpgText = string.Empty;
		string rangeUpgText = string.Empty;
		if (showUpgrade && level + 1 < tower.levels.Count) {
			if (tower.buffType == Tower.BuffType.AttackDamage)
				percUpgText = GetUpgText(Mathf.RoundToInt((tower.levels[level + 1].attackDamageAura - tower.levels[level].attackDamageAura) * 100));
			else
				percUpgText = GetUpgText(Mathf.RoundToInt((tower.levels[level + 1].attackSpeedAura - tower.levels[level].attackSpeedAura) * 100));
			rangeUpgText = GetUpgText(tower.levels[level + 1].auraRange - tower.levels[level].auraRange);
		}

		float auraPercentage = tower.buffType == Tower.BuffType.AttackDamage ? tower.levels[level].attackDamageAura : tower.levels[level].attackSpeedAura;
		return string.Format("Aura: {0}\nAura Percentage: {1}% {2}\nAura Range: {3} {4}", tower.buffType, Mathf.RoundToInt(auraPercentage * 100), percUpgText, tower.levels[level].auraRange, rangeUpgText);
	}

	private string GetUpgText(float value, bool plus = true) {
		string upgText = string.Empty;
		if (value > 0.01f)
			upgText = string.Format("<color={0}>+{1}</color>", plus ? upgColor : badColor, value);
		else if (value < -0.01f)
			upgText = string.Format("<color={0}>{1}</color>", plus ? badColor : upgColor, value);

		return upgText;
	}

	public void OnUpgrade() {
		if (visible) {
			EventManager.Raise(new UpgradeTowerButtonEvent());
			Show(tower);
		}
	}

	public void OnSell() {
		if (visible) EventManager.Raise(new SellTowerButtonEvent());
	}
}
