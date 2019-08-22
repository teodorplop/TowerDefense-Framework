using System;
using UnityEngine;

namespace GFX.Glow {
	[RequireComponent(typeof(Camera))]
	public class GlowPrePass : MonoBehaviour {
		[SerializeField]
		private int blurIterations = 4;
		[SerializeField, Tooltip("Will not update in play mode.")]
		private int blurDownsample = 1;

		[Space(10), SerializeField]
		private Shader glowShader;
		[SerializeField]
		private Shader blurShader;

		private new Camera camera;
		private RenderTexture prePass;
		private RenderTexture blur;
		private Material blurMaterial;

		private void Awake() {
			prePass = new RenderTexture(Screen.width, Screen.height, 24);
			blur = new RenderTexture(Screen.width >> blurDownsample, Screen.height >> blurDownsample, 0);

			Shader.SetGlobalTexture("_GlowPrePassTex", prePass);
			Shader.SetGlobalTexture("_GlowBlurTex", blur);

			blurMaterial = new Material(blurShader);
			blurMaterial.SetVector("_BlurSize", new Vector2(blur.texelSize.x * 1.5f, blur.texelSize.y * 1.5f));

			camera = GetComponent<Camera>();
			camera.targetTexture = prePass;
			camera.SetReplacementShader(glowShader, "");
			camera.enabled = true;
		}

		private void OnRenderImage(RenderTexture src, RenderTexture dst) {
			Graphics.Blit(src, blur);
			RenderTexture temp = RenderTexture.GetTemporary(blur.width, blur.height);
			for (int i = 0; i < blurIterations; ++i) {
				Graphics.Blit(blur, temp, blurMaterial);
				Graphics.Blit(temp, blur);
			}
			RenderTexture.ReleaseTemporary(temp);

			Graphics.Blit(src, dst);
		}
	}
}
