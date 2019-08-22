using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DropdownPathHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private TMP_Text text;

	void Awake() {
		text = GetComponent<TMP_Text>();
	}

	void OnDisable() {
		EventManager.Raise(new EditorPathHighlightEvent(-1));
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (text.text.Length <= 5) return;

		int pathIdx;
		if (int.TryParse(text.text.Substring(5), out pathIdx))
			EventManager.Raise(new EditorPathHighlightEvent(pathIdx));
	}

	public void OnPointerExit(PointerEventData eventData) {
		EventManager.Raise(new EditorPathHighlightEvent(-1));
	}
}
