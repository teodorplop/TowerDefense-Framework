using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {
	private Action<float> loadProgress;

	private IEnumerator Loading_EnterState(object input) {
		player = new Player(level.startingCurrency, level.startingHealth);

		yield return StartCoroutine(gameLevelLoader.LoadLevel(level));
		loadProgress(0.4f);

		towerSpotParent = gameLevelLoader.TowerSpotParent;
		pathsParent = gameLevelLoader.PathsParent;
		Destroy(gameLevelLoader);

		var async = SceneManager.LoadSceneAsync("GameUI", LoadSceneMode.Additive);
		while (!async.isDone) {
			loadProgress(0.4f + async.progress * 0.55f);
			yield return null;
		}

		gameUI = FindObjectOfType<GameUI>();
		gameUI.Inject(cameraController.Camera);
		gameUI.TowersPanel.Inject(towerFactory.Data, towerFactory.Sprites);
		gameUI.PlayerStats.Inject(player);

		wavesSpawner.Inject(unitFactory, level.paths);
		wavesSpawner.onSingleFinished += OnSingleFinished;

		yield return null;

		loadProgress(1.0f);

		gameUI.EnableStartButton(true);
		EventManager.AddListener<StartLevelButtonEvent>(OnStartLevelButtonEvent);
		EventManager.AddListener<NextWaveButtonEvent>(OnNextWaveButtonEvent);
		EventManager.AddListener<UnitSpawnedEvent>(OnUnitSpawnedEvent);

		SetState(State.Idle);
	}

	void OnDestroy() {
		EventManager.RemoveListener<StartLevelButtonEvent>(OnStartLevelButtonEvent);
		EventManager.RemoveListener<NextWaveButtonEvent>(OnNextWaveButtonEvent);
		EventManager.RemoveListener<UnitSpawnedEvent>(OnUnitSpawnedEvent);
	}

	private void OnStartLevelButtonEvent(StartLevelButtonEvent evt) {
		wavesSpawner.Spawn(level.waves);
		gameUI.EnableStartButton(false);
		EventManager.RemoveListener<StartLevelButtonEvent>(OnStartLevelButtonEvent);
	}

	private void OnNextWaveButtonEvent(NextWaveButtonEvent evt) {
		wavesSpawner.SpawnNext();
		gameUI.EnableNextWaveButton(false);
	}

	private void OnUnitSpawnedEvent(UnitSpawnedEvent evt) {
		evt.unit.onPathFinished += OnUnitPathFinished;
		evt.unit.onUnitDied += OnUnitDied;
	}

	private void OnUnitPathFinished(Unit unit) {
		player.Health -= unit.UnitData.damage;
		if (player.IsDead) gameUI.ShowGameOver(false);
	}
	private void OnUnitDied(Unit unit) {
		player.AddCurrency(unit.UnitData.bounty);
	}

	private void OnSingleFinished(int index) {
		if (index + 1 < level.waves.Count) gameUI.EnableNextWaveButton(true);
		else gameUI.ShowGameOver(true);
	}
}
