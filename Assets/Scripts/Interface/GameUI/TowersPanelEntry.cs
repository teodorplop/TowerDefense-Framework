using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class TowersPanelEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private int index;
	private Action<int> onTowerPressed;
	private Action<int> onEnter, onExit;

	public void Inject(int index, Sprite sprite, Action<int> onTowerPressed) {
		this.index = index;
		GetComponent<Image>().sprite = sprite;
		this.onTowerPressed = onTowerPressed;
	}
	public void SetOnHover(Action<int> onEnter, Action<int> onExit) {
		this.onEnter = onEnter;
		this.onExit = onExit;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (onEnter != null) onEnter(index);
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (onExit != null) onExit(index);
	}

	public void OnPress() {
		if (onTowerPressed != null) onTowerPressed(index);
	}
}
