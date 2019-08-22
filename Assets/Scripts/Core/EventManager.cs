using System.Collections.Generic;
using System;
using UnityEngine;

public class EventManager {
	private static EventManager instance = null;
	private static EventManager Instance { get { return instance ?? (instance = new EventManager()); } }

	public delegate void EventDelegate<T>(T e);

	private Dictionary<Type, Delegate> delegates = new Dictionary<Type, Delegate>();

	public static void AddListener<T>(EventDelegate<T> del) {
		Dictionary<Type, Delegate> delegates = Instance.delegates;
		
		if (delegates.ContainsKey(typeof(T))) {
			Delegate tempDel = delegates[typeof(T)];

			delegates[typeof(T)] = Delegate.Combine(tempDel, del);
		} else {
			delegates[typeof(T)] = del;
		}
	}

	public static void RemoveListener<T>(EventDelegate<T> del) {
		Dictionary<Type, Delegate> delegates = Instance.delegates;

		if (delegates.ContainsKey(typeof(T))) {
			Delegate currentDel = Delegate.Remove(delegates[typeof(T)], del);

			if (currentDel == null) {
				delegates.Remove(typeof(T));
			} else {
				delegates[typeof(T)] = currentDel;
			}
		}
	}

	public static void Raise<T>(T e) {
		Delegate del;
		if (Instance.delegates.TryGetValue(e.GetType(), out del)) del.DynamicInvoke(e);
	}
}
