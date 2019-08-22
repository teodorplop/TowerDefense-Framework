using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: implement a request dispatcher to have better control over what actions are accepted in every state.
public partial class GameManager : StateMachineBase {
	private enum State {
		None,
		Loading, Idle, PlaceTower, TowerSelected,
	};

	[SerializeField] private GameLevelLoader gameLevelLoader;
	[SerializeField] private WavesSpawner wavesSpawner;
	[SerializeField] private CameraController cameraController;
	[SerializeField] private GlowEffect glowEffect;
	[SerializeField] private TowerSpotHighlighter towerSpotHighlighter;
	[SerializeField] private TowerRangeIndicator towerRangeIndicator;
	[Space(10), SerializeField] private LayerMask towerSpotLayer;

	private GameUI gameUI;
	private HealthBars worldUI;

	private Transform towerSpotParent;
	private Transform pathsParent;

	private LevelFile level;
	private TowerFactory towerFactory;
	private UnitFactory unitFactory;
	private Player player;

	private void SetState(State state, object arg = null) { // Sending structs will make boxing, not very performant.
		stateMachineHandler.SetState(state, arg, this);
	}

	public void LoadGame(LevelFile level, TowerFactory towerFactory, UnitFactory unitFactory, Action<float> onProgress) {
		this.level = level;
		this.towerFactory = towerFactory;
		this.unitFactory = unitFactory;
		loadProgress = onProgress;

		SetState(State.Loading);
	}
}
