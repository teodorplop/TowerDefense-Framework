using UnityEngine;

namespace GFX.Glow {
	[RequireComponent(typeof(Camera))]
	public class GlowComposite : MonoBehaviour {
		[SerializeField, Range(0, 10)] private float glowIntensity = 2.0f;
		[Space(10), SerializeField] private Shader compositeShader;

		private Material compositeMaterial;

		private void Awake() {
			compositeMaterial = new Material(compositeShader);
		}

		private void OnRenderImage(RenderTexture src, RenderTexture dst) {
			compositeMaterial.SetFloat("_Intensity", glowIntensity);
			Graphics.Blit(src, dst, compositeMaterial);
		}
	}
}
