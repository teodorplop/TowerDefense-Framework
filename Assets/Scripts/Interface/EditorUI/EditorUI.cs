using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class EditorUI : MonoBehaviour {
	[SerializeField] private LevelSettingsPanel levelSettingsPanel;
	[SerializeField] private WavesPanel wavesPanel;
	[SerializeField] private UnitSelectionPanel unitSelectionPanel;
	[SerializeField] private LoadSavePanel loadSavePanel;
	[SerializeField] private TMP_Text guidanceMessage;
	[SerializeField] private GameObject guidanceObject;

	public LevelSettingsPanel LevelSettings { get { return levelSettingsPanel; } }
	public WavesPanel WavesPanel { get { return wavesPanel; } }
	public UnitSelectionPanel UnitSelectionPanel { get { return unitSelectionPanel; } }
	public LoadSavePanel LoadSavePanel { get { return loadSavePanel; } }

	private Action onAddPathButton, onAddTowerSpotButton;

	public void SetGuidanceMessage(string text) {
		if (string.IsNullOrEmpty(text))
			guidanceObject.SetActive(false);
		else {
			guidanceMessage.text = text;
			guidanceObject.SetActive(true);
		}
	}

	public void Set_OnAddPathButton(Action action) {
		onAddPathButton = action;
	}

	public void Set_OnAddTowerSpotButton(Action action) {
		onAddTowerSpotButton = action;
	}

	public void OnPaths() {
		if (onAddPathButton != null) onAddPathButton();
	}

	public void OnTowerPlacement() {
		if (onAddTowerSpotButton != null) onAddTowerSpotButton();
	}
}
