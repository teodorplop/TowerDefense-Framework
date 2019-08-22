using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LevelEditorManager {
	private List<LevelFile> officialLevels, customLevels;
	private Action<float> loadProgress;
	private Action<LevelFile> onEditorSave, onEditorDelete;

	private IEnumerator LoadEditor_EnterState(object input) {
		yield return StartCoroutine(gameLevelLoader.LoadLevel(level, true));
		loadProgress(0.5f);

		towerSpotParent = gameLevelLoader.TowerSpotParent;
		pathsParent = gameLevelLoader.PathsParent;
		Destroy(gameLevelLoader);

		var async = SceneManager.LoadSceneAsync("EditorUI", LoadSceneMode.Additive);
		while (!async.isDone) {
			loadProgress(0.5f + async.progress * 0.5f);
			yield return null;
		}

		editorUI = FindObjectOfType<EditorUI>();
		editorUI.LevelSettings.Inject(level);
		editorUI.WavesPanel.Inject(level);
		editorUI.LoadSavePanel.Inject(officialLevels, customLevels);
		editorUI.LoadSavePanel.Set_OnDelete(OnDeleteLevel);
		editorUI.LoadSavePanel.Set_OnLoad(OnLoadLevel);
		editorUI.LoadSavePanel.Set_OnSave(OnSaveLevel);

		yield return null;

		SetState(State.EditorIdle);
	}

	void OnDestroy() {
		EventManager.RemoveListener<EditorPathHighlightEvent>(OnEditorPathHighlightEvent);
	}

	private void OnSaveLevel(string fileName, bool official) {
		level.fileName = fileName;
		level.isOfficial = official;
		onEditorSave(level);
	}

	private void OnDeleteLevel(LevelFile level) {
		onEditorDelete(level);
	}

	private void OnLoadLevel(LevelFile level) {
		EventManager.Raise(new LoadLevelEvent(level));
	}
}
