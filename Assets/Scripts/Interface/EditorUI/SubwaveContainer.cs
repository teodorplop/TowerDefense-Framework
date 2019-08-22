using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubwaveContainer : MonoBehaviour {
	[SerializeField] private Image unitIcon;
	[SerializeField] private TMP_InputField countField;
	[SerializeField] private TMP_InputField delayField;
	[SerializeField] private TMP_InputField intervalField;
	[SerializeField] private TMP_Dropdown pathDropdown;
	private List<string> dropdownOptions;

	private UnitSelectionPanel unitSelectionPanel;
	private Action onModified;
	private Action<int> onDelete;

	private List<SubWave> subwaves;
	private int index;

	void Awake() {
		countField.onValueChanged.AddListener(OnCountModified);
		delayField.onValueChanged.AddListener(OnDelayModified);
		intervalField.onValueChanged.AddListener(OnIntervalModified);
		pathDropdown.onValueChanged.AddListener(OnPathModified);
	}

	public void Inject(UnitSelectionPanel unitSelectionPanel) {
		this.unitSelectionPanel = unitSelectionPanel;
	}
	public void Inject(List<SubWave> subwaves, int index) {
		this.subwaves = subwaves;
		this.index = index;

		unitIcon.sprite = UnitFactory.GetSprite(subwaves[index].unitId);
		countField.text = subwaves[index].numberOfUnits.ToString();
		delayField.text = subwaves[index].spawnDelay.ToString("F2");
		intervalField.text = subwaves[index].spawnInterval.ToString("F2");
	}
	public void SetNumberOfPaths(int count) {
		if (dropdownOptions == null) dropdownOptions = new List<string>();

		if (count < dropdownOptions.Count) dropdownOptions.RemoveRange(count, dropdownOptions.Count - count);
		while (dropdownOptions.Count < count) dropdownOptions.Add("Path " + dropdownOptions.Count.ToString());

		int value = pathDropdown.value;
		pathDropdown.ClearOptions();
		pathDropdown.AddOptions(dropdownOptions);
		pathDropdown.value = value;
	}
	public void Set_OnModified(Action action) {
		onModified = action;
	}
	public void Set_OnDelete(Action<int> action) {
		onDelete = action;
	}

	public void OnUnitIcon() {
		if (unitSelectionPanel != null)
			unitSelectionPanel.Show(
				subwaves[index].unitId,
				delegate(int newUnitId) {
					SubWave subwave = subwaves[index];
					subwave.unitId = (byte)newUnitId;
					subwaves[index] = subwave;

					unitIcon.sprite = UnitFactory.GetSprite(subwaves[index].unitId);
				});
	}
	public void OnDelete() {
		if (onDelete != null) onDelete(index);
	}

	// TODO: Add validators here
	private void OnCountModified(string value) {
		SubWave subwave = subwaves[index];
		subwave.numberOfUnits = byte.Parse(value);
		subwaves[index] = subwave;

		if (onModified != null) onModified();
	}
	private void OnDelayModified(string value) {
		SubWave subwave = subwaves[index];
		subwave.spawnDelay = float.Parse(value);
		subwaves[index] = subwave;

		if (onModified != null) onModified();
	}
	private void OnIntervalModified(string value) {
		SubWave subwave = subwaves[index];
		subwave.spawnInterval = float.Parse(value);
		subwaves[index] = subwave;

		if (onModified != null) onModified();
	}
	private void OnPathModified(int value) {
		SubWave subwave = subwaves[index];
		subwave.pathId = (byte)value;
		subwaves[index] = subwave;

		if (onModified != null) onModified();
	}
}
