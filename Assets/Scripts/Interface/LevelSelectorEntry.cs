using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectorEntry : MonoBehaviour {
	[SerializeField] private Toggle toggle;
	[SerializeField] private TMP_Text levelNameText;

	private Image image;
	private Color evenColor, oddColor;

	private int index;
	private Action<int> onSelected;

	public void Inject(int index, string levelName) {
		this.index = index;
		levelNameText.text = levelName;

		if (image == null) {
			image = GetComponent<Image>();
			evenColor = image.color;
			oddColor = evenColor * 0.9f;
		}
		image.color = ((index & 1) == 0) ? evenColor : oddColor;
	}

	public void ForceSelect(bool isOn) {
		toggle.isOn = isOn;
	}

	public void Set_OnSelected(Action<int> action) {
		onSelected = action;
	}

	public void OnValueChanged(bool isOn) {
		if (isOn && onSelected != null) onSelected(index);
	}
}
