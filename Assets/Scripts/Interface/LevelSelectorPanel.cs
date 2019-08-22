using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectorPanel : MonoBehaviour {
	private enum Mode { Official, Custom };

	[SerializeField] private LevelSelectorEntry levelEntryPrefab;
	[Space(10), SerializeField] private TMP_Text levelTitleText;
	[SerializeField] private Button launchButton;

	private List<LevelSelectorEntry> levelEntries;

	private Action<LevelFile> onLaunch;
	private List<LevelFile> official, custom;
	private Mode mode;
	
	private LevelFile selectedLevel;

	public void Inject(List<LevelFile> official, List<LevelFile> custom) {
		this.official = official;
		this.custom = custom;

		levelEntries = new List<LevelSelectorEntry>();
		levelEntryPrefab.gameObject.SetActive(false);

		mode = Mode.Official;
		SetLevels(official);
	}
	public void Set_OnLaunch(Action<LevelFile> onLaunch) {
		this.onLaunch = onLaunch;
	}

	public void OnOfficialToggle(bool isOn) {
		if (isOn) {
			mode = Mode.Official;
			SetLevels(official);
		}
	}
	public void OnCustomToggle(bool isOn) {
		if (isOn) {
			mode = Mode.Custom;
			SetLevels(custom);
		}
	}
	public void OnLaunch() {
		if (selectedLevel != null)
			onLaunch(selectedLevel);
	}

	private void SetLevels(List<LevelFile> levels) {
		for (int i = 0; i < levels.Count; ++i) {
			LevelSelectorEntry entry = GetEntry(i);
			entry.Inject(i, levels[i].name);
			entry.ForceSelect(false);
		}
		for (int i = levels.Count; i < levelEntries.Count; ++i)
			levelEntries[i].gameObject.SetActive(false);
		
		if (levels.Count == 0) SelectLevel(-1);
		else {
			SelectLevel(0);
			levelEntries[0].ForceSelect(true);
		}
	}

	private LevelSelectorEntry GetEntry(int index) {
		LevelSelectorEntry entry;
		if (index == levelEntries.Count) {
			levelEntries.Add(entry = Instantiate(levelEntryPrefab, levelEntryPrefab.transform.parent));
			entry.Set_OnSelected(OnEntrySelected);
		} else entry = levelEntries[index];
		entry.gameObject.SetActive(true);

		return entry;
	}

	private void OnEntrySelected(int index) {
		SelectLevel(index);
	}

	private void SelectLevel(int index) {
		if (index == -1) selectedLevel = null;
		else {
			if (mode == Mode.Custom) selectedLevel = custom[index];
			else if (mode == Mode.Official) selectedLevel = official[index];
		}

		if (selectedLevel != null) {
			levelTitleText.text = selectedLevel.name;
			launchButton.gameObject.SetActive(true);
		} else {
			levelTitleText.text = string.Empty;
			launchButton.gameObject.SetActive(false);
		}
	}
}
