using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnciclopediaEntry : MonoBehaviour {
	[SerializeField] private Toggle toggle;
	[SerializeField] private Image icon;
	private Action onSelected;

	public void Inject(Sprite sprite) {
		icon.sprite = sprite;
	}

	public void SetOnSelected(Action onSelected) {
		this.onSelected = onSelected;
	}

	public void OnValueChanged(bool value) {
		if (value && onSelected != null) onSelected();
	}

	public void ForceSelect(bool isOn) {
		toggle.isOn = isOn;
	}
}
