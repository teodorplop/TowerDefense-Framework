using UnityEngine;
using System;
using System.Collections.Generic;

namespace GFX.Glow {
	public class GlowingObject : MonoBehaviour {
		private Renderer[] renderers;
		private List<Material> materials;

		[SerializeField] private Color glowColor = Color.black;
		[SerializeField, Layer] private int glowLayer;

		private LayerMask initialLayer;
		public Action onDestroy;

		public int GlowLayer {
			get { return glowLayer; }
			set {
				glowLayer = value;
				if (enabled) SetLayer(glowLayer);
			}
		}

		private void Awake() {
			renderers = GetComponentsInChildren<Renderer>();
			materials = new List<Material>();
			foreach (Renderer renderer in renderers)
				materials.Add(renderer.material); // TODO: Creating one material per glowing object. Improve this!!!

			SetGlowColor(glowColor);
		}
		void OnDestroy() {
			if (onDestroy != null) onDestroy();
		}

		void OnEnable() {
			initialLayer = gameObject.layer;
			SetLayer(glowLayer);
		}
		void OnDisable() {
			SetLayer(initialLayer);
		}

		private void SetLayer(int layer) {
			foreach (Renderer renderer in renderers)
				renderer.gameObject.layer = layer;
		}

		public void SetGlowColor(Color glowColor) {
			this.glowColor = glowColor;
			foreach (Material mat in materials) mat.SetColor("_GlowColor", glowColor);
		}
	}
}
