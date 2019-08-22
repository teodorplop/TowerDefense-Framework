using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Panel : MonoBehaviour {
	private DOTweenAnimation showAnimation;

	void Awake() {
		showAnimation = GetComponent<DOTweenAnimation>();
		showAnimation.hasOnRewind = true;
		showAnimation.onRewind.AddListener(delegate { gameObject.SetActive(false); });
		// TODO: had to manually set OnRewind from the editor. Check why?
	}
	
	public void Show() {
		gameObject.SetActive(true);
		showAnimation.DOPlayForward();
	}

	public void Hide() {
		showAnimation.DOPlayBackwards();
	}
}
