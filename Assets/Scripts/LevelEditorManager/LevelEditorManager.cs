using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

// TODO: Perhaps it would have been better to have different state handlers than one big partial class.
public partial class LevelEditorManager : StateMachineBase {
	private enum State {
		None,
		LoadEditor, EditorIdle, PlaceTowerSpot, ResizeTowerSpot, SelectedTowerSpot, PlaceNewPath, SelectedWaypoint
	};

	[SerializeField] private GameLevelLoader gameLevelLoader;
	[SerializeField] private CameraController cameraController;
	[SerializeField] private GlowEffect glowEffect;
	[Space(10), SerializeField] private TowerSpotGrid towerSpotPrefab;
	[SerializeField] private PathIndicator pathIndicatorPrefab;
	[SerializeField] private LineRenderer pathRendererPrefab;
	[Space(10), SerializeField] private LayerMask towerLayer;
	[SerializeField] private LayerMask pathLayer;

	private Transform towerSpotParent;
	private Transform pathsParent;

	private LevelFile level;
	private UnitFactory unitFactory;
	
	private void SetState(State state, object arg = null) {
		stateMachineHandler.SetState(state, arg, this);
	}

	public void LoadEditor(LevelFile level,List<LevelFile> official, List<LevelFile> custom, Action<float> onProgress) {
		this.level = level;
		officialLevels = official;
		customLevels = custom;
		loadProgress = onProgress;

		SetState(State.LoadEditor);
	}
	public void SetEditorFunctions(Action<LevelFile> onSave, Action<LevelFile> onDelete) {
		onEditorSave = onSave;
		onEditorDelete = onDelete;
	}
}
