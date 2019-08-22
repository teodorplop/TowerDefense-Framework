using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro {
	[CreateAssetMenu(fileName = "TMP_FileNameValidator", menuName = "ScriptableObjects/TMP_FileNameValidator")]
	public class TMP_FileNameValidator : TMP_InputValidator {
		private readonly char[] invalidChars = Path.GetInvalidFileNameChars();

		// TODO: Bug when selecting entire text and pressing anything else than Backspace & Delete.
		public override char Validate(ref string text, ref int pos, char ch) {
			if (Array.IndexOf(invalidChars, ch) != -1)
				return (char)0x00;

			text = text.Insert(pos++, ch.ToString());
			return ch;
		}
	}
}
