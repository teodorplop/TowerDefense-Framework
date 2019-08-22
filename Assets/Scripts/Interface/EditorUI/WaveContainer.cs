using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveContainer : MonoBehaviour {
	[SerializeField] private TMP_Text title;
	[SerializeField] private SubwaveContainer subwavePrefab;
	
	private List<SubwaveContainer> subwaveEntries;

	private int numberOfPaths;
	private Wave wave;
	private int index;

	private UnitSelectionPanel unitSelectionPanel;
	private Action<int> onDelete;
	private Action onModified;

	public void Inject(UnitSelectionPanel unitSelectionPanel) {
		this.unitSelectionPanel = unitSelectionPanel;
	}
	public void Inject(Wave wave, int index) {
		this.wave = wave;
		this.index = index;

		title.text = "Wave " + index;

		if (subwaveEntries == null) {
			subwaveEntries = new List<SubwaveContainer>();
			subwavePrefab.gameObject.SetActive(false);
		}

		// TODO: Duplicate code with WavesPanel. Think of a way to populate UI with generic data.
		for (int i = 0; i < wave.subwaves.Count; ++i)
			NewSubwave(i);
		if (wave.subwaves.Count > 0) Expand(wave.subwaves.Count);

		for (int i = wave.subwaves.Count; i < subwaveEntries.Count; ++i)
			subwaveEntries[i].gameObject.SetActive(false);
	}

	public void SetNumberOfPaths(int count) {
		numberOfPaths = count;
		if (subwaveEntries != null) foreach (var entry in subwaveEntries) entry.SetNumberOfPaths(count);
	}

	public void OnPlusButton() {
		wave.subwaves.Add(new SubWave());
		NewSubwave(wave.subwaves.Count - 1);
		if (onModified != null) onModified();
	}

	private void NewSubwave(int index) {
		SubwaveContainer subwave;
		if (index == subwaveEntries.Count) {
			subwaveEntries.Add(subwave = Instantiate(subwavePrefab, subwavePrefab.transform.parent));
			subwave.Inject(unitSelectionPanel);
			subwave.Set_OnModified(OnSubwaveModified);
			subwave.Set_OnDelete(OnSubwaveDeleted);
		} else subwave = subwaveEntries[index];

		subwave.transform.SetSiblingIndex(subwaveEntries.Count);
		subwave.Inject(wave.subwaves, index);
		subwave.SetNumberOfPaths(numberOfPaths);
		subwave.gameObject.SetActive(true);

		Expand(1);
	}

	private void Expand(int value) {
		RectTransform rect = (transform as RectTransform);
		Vector2 size = rect.sizeDelta;
		size.y += (subwaveEntries[0].transform as RectTransform).sizeDelta.y * value;
		rect.sizeDelta = size;
	}

	private void OnSubwaveDeleted(int index) {
		wave.subwaves.RemoveAt(index);
		for (int i = index; i < wave.subwaves.Count; ++i)
			subwaveEntries[i].Inject(wave.subwaves, i);
		subwaveEntries[wave.subwaves.Count].gameObject.SetActive(false);
		Expand(-1);

		if (onModified != null) onModified();
	}

	private void OnSubwaveModified() {
		if (onModified != null) onModified();
	}

	public void Set_OnDelete(Action<int> action) {
		onDelete = action;
	}
	public void Set_OnModified(Action action) {
		onModified = action;
	}

	public void OnDelete() {
		if (onDelete != null) onDelete(index);
	}
}
