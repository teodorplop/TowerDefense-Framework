using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;

public static class CsvSerializer {
	private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
	private const short BUFFER = 256;

	public static T Deserialize<T>(byte[] data) where T : new() {
#if UNITY_EDITOR
		if (typeof(T).IsValueType) Debug.LogWarning(typeof(T) + " is value type. Deserializing value types is a bit slower.");
#endif

		object t = new T();
		char[] chrArray = new char[BUFFER];
		int chrArrayIdx = 0;
		bool inComment = false;
		FieldInfo field = null;
		string value = null;
		for (int i = 0; i < data.Length; ++i) {
			if (!IsValidChar(data[i])) continue;

			if (i > 0 && data[i] == '/' && data[i - 1] == '/') inComment = true;
			if (data[i] == '\n') inComment = false;

			if (inComment) continue;

			if (data[i] == ',') {
				while (chrArray[chrArrayIdx] == ' ') --chrArrayIdx; // Trim end
				string key = new string(chrArray, 0, chrArrayIdx);
				chrArrayIdx = 0;

				field = typeof(T).GetField(key, FLAGS);
				if (field == null)
					Debug.LogErrorFormat("Cannot find field {0} of type {1}.", key, typeof(T));
			} else if (data[i] == '\n') {
				if (field != null) {
					while (chrArray[chrArrayIdx] == ' ') --chrArrayIdx; // Trim end
					value = new string(chrArray, 0, chrArrayIdx);

					try {
						SetValue(field, t, value);
					} catch (Exception e) {
						Debug.LogError("CsvSerializer error: " + e.Message);
					}
				}
				chrArrayIdx = 0;
			} else {
				if (chrArrayIdx > 0 || data[i] != ' ') // Trim start
					chrArray[chrArrayIdx++] = (char)data[i];
			}
		}

		return (T)t;
	}

	public static List<T> DeserializeList<T>(byte[] data) where T : new() {
#if UNITY_EDITOR
		if (typeof(T).IsValueType) Debug.LogWarning(typeof(T) + " is value type. Deserializing value types is a bit slower.");
#endif

		List<T> list = new List<T>();

		List<FieldInfo> fields = new List<FieldInfo>();

		object t = new T();
		char[] chrArray = new char[BUFFER];
		bool firstLine = true, inComment = false;
		short fieldIdx = 0, chrArrayIdx = 0;

		for (int i = 0; i < data.Length; ++i) {
			if (!IsValidChar(data[i])) continue;

			if (i > 0 && data[i] == '/' && data[i - 1] == '/') inComment = true;
			if (data[i] == '\n') inComment = false;

			if (inComment) continue;

			if (data[i] != ',' && data[i] != '\n') {
				if (chrArrayIdx > 0 || data[i] != ' ') // Trim start
					chrArray[chrArrayIdx++] = (char)data[i];
			} else {
				if (firstLine) {
					if (chrArrayIdx > 0) {
						while (chrArray[chrArrayIdx] == ' ') --chrArrayIdx; // Trim end

						string fieldName = new string(chrArray, 0, chrArrayIdx);
						FieldInfo field = typeof(T).GetField(fieldName, FLAGS);
						if (field != null) fields.Add(field);
						else Debug.LogErrorFormat("Cannot find field {0} of type {1}.", fieldName, typeof(T));
					}

					if (data[i] == '\n') firstLine = false;
					chrArrayIdx = 0;
				} else {
					if (chrArrayIdx > 0) {
						while (chrArray[chrArrayIdx] == ' ') --chrArrayIdx; // Trim end

						try {
							if (fieldIdx < fields.Count)
								SetValue(fields[fieldIdx++], t, new string(chrArray, 0, chrArrayIdx));
						} catch (Exception e) {
							Debug.LogError("CsvSerializer error: " + e.Message);
						}
					}

					if (data[i] == '\n') {
						list.Add((T)t);
						fieldIdx = 0;
						t = new T();
					}
					chrArrayIdx = 0;
				}
			}
		}

		return list;
	}

	private static bool IsValidChar(byte chr) {
		return chr != 13; // Carriage return '\r'
	}
	private static void SetValue(FieldInfo field, object obj, string str) {
		Type type = field.FieldType;
		if (type == typeof(bool)) field.SetValue(obj, bool.Parse(str));
		else if (type == typeof(byte)) field.SetValue(obj, byte.Parse(str));
		else if (type == typeof(sbyte)) field.SetValue(obj, sbyte.Parse(str));
		else if (type == typeof(short)) field.SetValue(obj, short.Parse(str));
		else if (type == typeof(ushort)) field.SetValue(obj, ushort.Parse(str));
		else if (type == typeof(int)) field.SetValue(obj, int.Parse(str));
		else if (type == typeof(uint)) field.SetValue(obj, uint.Parse(str));
		else if (type == typeof(long)) field.SetValue(obj, long.Parse(str));
		else if (type == typeof(ulong)) field.SetValue(obj, ulong.Parse(str));
		else if (type == typeof(float)) field.SetValue(obj, float.Parse(str));
		else if (type == typeof(double)) field.SetValue(obj, double.Parse(str));
		else if (type == typeof(decimal)) field.SetValue(obj, decimal.Parse(str));
		else if (type == typeof(string)) field.SetValue(obj, str);
		else if (type.IsEnum) field.SetValue(obj, Enum.Parse(type, str));
		else field.SetValue(obj, TypeDescriptor.GetConverter(type).ConvertFrom(str));
	}
}
