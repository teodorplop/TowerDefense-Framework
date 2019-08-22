using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class FadingMessage : MonoBehaviour {
	[SerializeField] private TMP_Text message;
	[SerializeField] private float shownForSeconds = 3.0f;

	private RectTransform thisTransform;
	private CanvasGroup canvas;
	private Sequence fadeTween;

	void Awake() {
		thisTransform = transform as RectTransform;
		canvas = GetComponent<CanvasGroup>();
		fadeTween = DOTween.Sequence();
		fadeTween.Append(canvas.DOFade(1.0f, 0.25f)).AppendInterval(shownForSeconds).Append(canvas.DOFade(0.0f, 0.25f)).SetAutoKill(false).Pause();
	}

	public void Show(Vector3 screenPos, string message) {
		Vector2 size = thisTransform.sizeDelta;
		screenPos.x = Mathf.Clamp(screenPos.x, size.x / 2, Screen.width - size.x / 2);
		screenPos.y = Mathf.Clamp(screenPos.y, size.y / 2, Screen.height - size.y / 2);

		thisTransform.position = screenPos;
		this.message.text = message;
		fadeTween.Restart();
	}
}
