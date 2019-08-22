using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
	[SerializeField] private Image fillImage;

	public float Progress {
		get { return fillImage.fillAmount; }
		set { fillImage.fillAmount = value; }
	}
}
