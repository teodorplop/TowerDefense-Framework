using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSettingsPanel : MonoBehaviour {
	[SerializeField] private TMP_InputField levelNameField;
	[SerializeField] private TMP_InputField currencyField;
	[SerializeField] private TMP_InputField healthField;
	[Space(10), SerializeField] private Button saveButton;

	private LevelFile level;

	void Awake() {
		levelNameField.onValueChanged.AddListener(OnValueChanged);
		currencyField.onValueChanged.AddListener(OnValueChanged);
		healthField.onValueChanged.AddListener(OnValueChanged);
		saveButton.interactable = false;
	}

	public void Inject(LevelFile level) {
		this.level = level;

		levelNameField.text = level.name;
		currencyField.text = level.startingCurrency.ToString();
		healthField.text = level.startingHealth.ToString();
	}

	public void Save() {
		level.name = levelNameField.text;
		level.startingCurrency = short.Parse(currencyField.text);
		level.startingHealth = short.Parse(healthField.text);

		saveButton.interactable = false;
		// TODO: Add validators here
	}

	private void OnValueChanged(string value) {
		saveButton.interactable = true;
	}
}
