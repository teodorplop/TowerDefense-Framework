using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavesPanel : MonoBehaviour {
	[SerializeField] private WaveContainer wavePrefab;
	[SerializeField] private Button saveButton;
	[SerializeField] private UnitSelectionPanel unitSelectionPanel;

	private List<WaveContainer> waveEntries;

	private LevelFile level;
	private List<Wave> waves;

	void Awake() {
		saveButton.interactable = false;
	}

	void OnEnable() {
		foreach (var entry in waveEntries)
			entry.SetNumberOfPaths(level.paths.Count);
	}

	private void CloneWaves() {
		waves = new List<Wave>(level.waves.Count);
		for (int i = 0; i < level.waves.Count; ++i) waves.Add(level.waves[i].Clone());
	}

	public void Inject(LevelFile level) {
		this.level = level;
		CloneWaves();

		if (waveEntries == null) {
			waveEntries = new List<WaveContainer>(level.waves.Count);
			wavePrefab.gameObject.SetActive(false);
		}

		// TODO: Duplicate code with WavesPanel. Think of a way to populate UI with generic data.
		for (int i = 0; i < waves.Count; ++i)
			NewWave(i);

		for (int i = waves.Count; i < waveEntries.Count; ++i)
			waveEntries[i].gameObject.SetActive(false);
	}

	public void Save() {
		level.waves = waves;
		CloneWaves();
		saveButton.interactable = false;
	}

	public void OnPlusButton() {
		waves.Add(new Wave());
		NewWave(waves.Count - 1);
		saveButton.interactable = true;
	}

	private void NewWave(int index) {
		WaveContainer wave;
		if (index == waveEntries.Count) {
			waveEntries.Add(wave = Instantiate(wavePrefab, wavePrefab.transform.parent));
			wave.Inject(unitSelectionPanel);
			wave.Set_OnDelete(OnWaveDeleted);
			wave.Set_OnModified(OnWaveModified);
		} else wave = waveEntries[index];

		wave.transform.SetSiblingIndex(waveEntries.Count - 1);
		wave.Inject(waves[index], index);
		wave.SetNumberOfPaths(level.paths.Count);

		wave.gameObject.SetActive(true);
	}

	private void OnWaveDeleted(int index) {
		waves.RemoveAt(index);
		for (int i = index; i < waves.Count; ++i)
			waveEntries[i].Inject(waves[i], i);
		waveEntries[waves.Count].gameObject.SetActive(false);
		saveButton.interactable = true;
	}
	private void OnWaveModified() {
		saveButton.interactable = true;
	}
}
