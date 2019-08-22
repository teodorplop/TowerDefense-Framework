using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFX.Glow;

public class GlowEffect : MonoBehaviour {
	[SerializeField] private Color color = Color.cyan;
	[SerializeField, Layer] private int glowLayer;
	
	private Dictionary<int, GlowingObject> glowingObjects;

	void Awake() {
		glowingObjects = new Dictionary<int, GlowingObject>();
	}

	public void Enable(GameObject obj) {
		int id = obj.GetInstanceID();
		if (glowingObjects.ContainsKey(id)) return;
		
		var glow = obj.GetComponent<GlowingObject>();
		if (glow == null) {
			glow = obj.AddComponent<GlowingObject>();
			glow.GlowLayer = glowLayer;
			glow.SetGlowColor(Color.cyan);
		}
		glow.enabled = true;
		
		glowingObjects.Add(id, glow);
		glow.onDestroy = delegate { glowingObjects.Remove(id); }; // TODO: think of a nicer way for this.
	}

	public void Disable(GameObject obj) {
		int id = obj.GetInstanceID();
		GlowingObject glowObject = null;

		if (glowingObjects.TryGetValue(id, out glowObject)) {
			glowObject.enabled = false;
			glowingObjects.Remove(id);
		}
	}
}
