using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TMP_Text))]
public class TMP_SizeFitter : MonoBehaviour {
	[SerializeField] private Vector2 offset = Vector2.one;
	[SerializeField] private bool horizontal = true;
	[SerializeField] private bool vertical = true;

	private RectTransform rect;
	private TMP_Text text;

	void Awake() {
		rect = GetComponent<RectTransform>();
		text = GetComponent<TMP_Text>();
	}

	void LateUpdate() {
		if (text.renderedWidth > 0 && text.renderedHeight > 0) {
			Vector2 size = new Vector2(text.renderedWidth, text.renderedHeight) + offset;
			if (!horizontal) size.x = rect.sizeDelta.x;
			if (!vertical) size.y = rect.sizeDelta.y;

			rect.sizeDelta = size;
		}
	}
}
