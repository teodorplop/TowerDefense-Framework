using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitToMenuButton : MonoBehaviour {
	public void OnPress() {
		EventManager.Raise(new ExitToMenuEvent());
	}
}
