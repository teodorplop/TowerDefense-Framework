#region Editor
public struct EditorPathHighlightEvent {
	public int pathIndex;
	public EditorPathHighlightEvent(int pathIndex) {
		this.pathIndex = pathIndex;
	}
}

public struct LevelDeletedEvent {
	public LevelFile level;
	public LevelDeletedEvent(LevelFile level) {
		this.level = level;
	}
}

public struct LevelSavedEvent {
	public LevelFile level;
	public LevelSavedEvent(LevelFile level) {
		this.level = level;
	}
}

public struct LoadLevelEvent {
	public LevelFile level;
	public LoadLevelEvent(LevelFile level) {
		this.level = level;
	}
}
#endregion

#region Game
public struct StartLevelButtonEvent { }

public struct NextWaveButtonEvent { }

public struct UnitSpawnedEvent {
	public Unit unit;
	public UnitSpawnedEvent(Unit unit) {
		this.unit = unit;
	}
}

public struct TowerSpawnedEvent {
	public Tower tower;
	public TowerSpawnedEvent(Tower tower) {
		this.tower = tower;
	}
}

public struct TowerButtonEvent {
	public int towerId;
	public TowerButtonEvent(int towerId) {
		this.towerId = towerId;
	}
}

public struct UpgradeTowerButtonEvent {
}

public struct SellTowerButtonEvent {
}

public struct RestartGameEvent {
}

public struct ExitToMenuEvent {
}
#endregion