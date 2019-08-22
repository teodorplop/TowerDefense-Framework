using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionEntry : MonoBehaviour {
	[SerializeField] private Toggle toggle;
	[SerializeField] private Image unitIcon;

	private int index;
	private Action<int> onSelected;

	public void Inject(int index, Sprite sprite, string tooltip, Action<int> onSelected) {
		this.index = index;
		this.onSelected = onSelected;

		unitIcon.sprite = sprite;
	}

	public void OnValueChanged(bool isOn) {
		if (isOn && onSelected != null) onSelected(index);
	}

	public void ForceSelect() {
		toggle.isOn = true;
	}
}
