using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadSavePanel : MonoBehaviour {
	[SerializeField] private LoadSaveFileEntry filePrefab;
	[Space(10), SerializeField] private Toggle officialToggle;
	[SerializeField] private TMP_InputField fileNameInput;
	[SerializeField] private Button saveButton;
	
	private List<LevelFile> levels;
	private List<LoadSaveFileEntry> fileEntries;
	private int selectedEntry = -1;

	private Action<string, bool> onSave;
	private Action<LevelFile> onDelete, onLoad;

	void Awake() {
#if !UNITY_EDITOR
		officialToggle.gameObject.SetActive(false);
#endif
		fileNameInput.onValueChanged.AddListener(OnFileNameInput);

		EventManager.AddListener<LevelDeletedEvent>(OnLevelDeleted);
		EventManager.AddListener<LevelSavedEvent>(OnLevelSaved);
	}
	void OnDestroy() {
		EventManager.RemoveListener<LevelDeletedEvent>(OnLevelDeleted);
		EventManager.RemoveListener<LevelSavedEvent>(OnLevelSaved);
	}

	public void Set_OnSave(Action<string, bool> action) {
		onSave = action;
	}
	public void Set_OnDelete(Action<LevelFile> action) {
		onDelete = action;
	}
	public void Set_OnLoad(Action<LevelFile> action) {
		onLoad = action;
	}

	public void Inject(List<LevelFile> official, List<LevelFile> custom) {
		levels = new List<LevelFile>();
		levels.AddRange(official);
		levels.AddRange(custom);

		fileEntries = new List<LoadSaveFileEntry>();
		filePrefab.gameObject.SetActive(false);

		for (int i = 0; i < levels.Count; ++i) {
			var fileEntry = GetEntry(i);
			fileEntry.Inject(i, levels[i].isOfficial, levels[i].fileName, levels[i].name, levels[i].modified);
		}
	}

	private LoadSaveFileEntry GetEntry(int idx) {
		LoadSaveFileEntry entry;
		if (idx == fileEntries.Count) {
			fileEntries.Add(entry = Instantiate(filePrefab, filePrefab.transform.parent));
			entry.Set_OnDelete(OnFileEntryDeleted);
			entry.Set_OnLoad(OnFileEntryLoaded);
			entry.Set_OnSelected(OnFileEntrySelected);
		} else
			entry = fileEntries[idx];
		entry.gameObject.SetActive(true);

		return entry;
	}

	private void OnFileEntryDeleted(int idx) {
		if (onDelete != null) onDelete(levels[idx]);
	}
	private void OnFileEntryLoaded(int idx) {
		if (onLoad != null) onLoad(levels[idx]);
	}
	private void OnFileEntrySelected(int idx) {
		selectedEntry = idx;

		fileNameInput.text = levels[idx].fileName;
		officialToggle.isOn = levels[idx].isOfficial;
		saveButton.interactable = true;
	}
	private void OnFileNameInput(string value) {
		int index = levels.FindIndex(obj => obj.fileName == value);
		if (index == -1) {
			if (selectedEntry != -1) {
				fileEntries[selectedEntry].ForceSelect(false);
				selectedEntry = -1;
			}
			return;
		}

		fileEntries[selectedEntry = index].ForceSelect(true);
	}

	public void Save() {
		if (onSave != null) onSave(fileNameInput.text, officialToggle.isOn);
	}

	private void OnLevelDeleted(LevelDeletedEvent evt) {
		int idx = levels.IndexOf(evt.level);
		if (idx != -1) {
			levels.RemoveAt(idx);
			for (int i = idx; i < levels.Count; ++i)
				fileEntries[i].Inject(i, levels[i].isOfficial, levels[i].fileName, levels[i].name, levels[i].modified);
			fileEntries[levels.Count].gameObject.SetActive(false);
		}
	}
	private void OnLevelSaved(LevelSavedEvent evt) {
		int idx = levels.FindIndex(obj => obj.Equals(evt.level));
		if (idx == -1) {
			levels.Add(evt.level);
			idx = levels.Count - 1;
		}

		var fileEntry = GetEntry(idx);
		fileEntry.Inject(idx, evt.level.isOfficial, evt.level.fileName, evt.level.name, evt.level.modified);
	}
}
