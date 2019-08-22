using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadSaveFileEntry : MonoBehaviour {
	[SerializeField] private Toggle toggle;
	[SerializeField] private Image customLevelIcon;
	[SerializeField] private TMP_Text fileNameText;
	[SerializeField] private TMP_Text levelNameText;
	[SerializeField] private TMP_Text modifiedText;

	private Image image;
	private Color evenColor;
	private Color oddColor;

	private int index;
	private Action<int> onSelected, onDelete, onLoad;

	public void Inject(int index, bool official, string fileName, string levelName, DateTime modified) {
		if (image == null) {
			image = GetComponent<Image>();
			evenColor = image.color;
			oddColor = evenColor * 0.9f;
		}

		this.index = index;
		image.color = ((index & 1) == 0) ? evenColor : oddColor;

		customLevelIcon.enabled = official;
		fileNameText.text = fileName;
		levelNameText.text = levelName;
		modifiedText.text = "Not yet implemented.";//modified.ToString();
	}

	public void ForceSelect(bool isOn) {
		toggle.isOn = isOn;
	}

	public void Set_OnSelected(Action<int> action) {
		onSelected = action;
	}
	public void Set_OnDelete(Action<int> action) {
		onDelete = action;
	}
	public void Set_OnLoad(Action<int> action) {
		onLoad = action;
	}

	public void OnValueChanged(bool isOn) {
		if (isOn && onSelected != null) onSelected(index);
	}

	public void OnDelete() {
		if (onDelete != null) onDelete(index);
	}
	public void OnLoad() {
		if (onLoad != null) onLoad(index);
	}
}
