using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ByteSerializer {
	public static byte[] Serialize<T>(T serializableObject) {
		T obj = serializableObject;
		using (MemoryStream stream = new MemoryStream()) {
			System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T));
			x.Serialize(stream, obj);
			return stream.ToArray();
		}
	}

	public static T Deserialize<T>(byte[] serilizedBytes) {
		System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T));
		using (MemoryStream stream = new MemoryStream(serilizedBytes)) {
			return (T)x.Deserialize(stream);
		}
	}

}
