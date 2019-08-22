using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelsStack : MonoBehaviour {
	[SerializeField] private DOTweenAnimation overlay;

	private Stack<Panel> stack;

	void Awake() {
		stack = new Stack<Panel>();

		overlay.hasOnRewind = true;
		overlay.onRewind.AddListener(delegate { overlay.gameObject.SetActive(false); });
	}

	public void Show(Panel panel) {
		BringOverlayBehind(panel.transform);
		overlay.gameObject.SetActive(true);
		overlay.DOPlayForward();
		panel.Show();

		stack.Push(panel);
	}

	public void Hide() {
		if (stack.Count > 0) {
			stack.Pop().Hide();

			if (stack.Count > 0)
				BringOverlayBehind(stack.Peek().transform);
			else
				overlay.DOPlayBackwards();
		}
	}

	private void BringOverlayBehind(Transform panel) {
		if (overlay.transform.parent != panel.parent)
			overlay.transform.SetParent(panel.parent, true);

		overlay.transform.SetAsLastSibling();
		panel.SetAsLastSibling();
	}
}
