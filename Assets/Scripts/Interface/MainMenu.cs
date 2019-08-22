using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MainMenu : MonoBehaviour {
	[SerializeField] private EnciclopediaPanel enciclopedia;
	[SerializeField] private LevelSelectorPanel levelSelector;

	private Action onEditorButton;

	public EnciclopediaPanel Enciclopedia { get { return enciclopedia; } }
	public LevelSelectorPanel LevelSelector { get { return levelSelector; } }

	public void Set_OnEditorButton(Action action) {
		onEditorButton = action;
	}

	public void OnEditorButton() {
		if (onEditorButton != null) onEditorButton();
	}
}
