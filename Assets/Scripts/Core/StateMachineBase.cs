using System;
using UnityEngine;
using System.Reflection;

public abstract class StateMachineBase : MonoBehaviour {
	protected StateMachineHandler stateMachineHandler;
	protected virtual void Awake() {
		stateMachineHandler = gameObject.AddComponent<StateMachineHandler>();
	}
	
	private Enum _currentState;
	public Enum currentState {
		get { return _currentState; }
		set {
			_currentState = value;
			if (_currentState == null) {
				ConfigureEmptyState();
			} else {
				ConfigureCurrentState();
			}
		}
	}

	#region actions
	private static void DoNothing() { }

	private Action DoUpdate = DoNothing;
	private Action DoLateUpdate = DoNothing;
	private Action DoFixedUpdate = DoNothing;
	#endregion

	#region configuration
	private void ConfigureEmptyState() {
		DoUpdate = DoNothing;
		DoLateUpdate = DoNothing;
		DoFixedUpdate = DoNothing;
		
		useGUILayout = false;
	}
	private void ConfigureCurrentState() {
		DoUpdate = ConfigureDelegate<Action>(_currentState, "Update", DoNothing);
		DoLateUpdate = ConfigureDelegate<Action>(_currentState, "LateUpdate", DoNothing);
		DoFixedUpdate = ConfigureDelegate<Action>(_currentState, "FixedUpdate", DoNothing);

	    useGUILayout = false;
	}
	
	public T ConfigureField<T>(Enum state, string fieldRoot, T Default) {
		FieldInfo field = GetType().GetField(state.ToString() + "_" + fieldRoot, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		if (field != null) {
			return (T)field.GetValue(this);
		}
		return Default;
	}

	public T ConfigureDelegate<T>(Enum state, string methodRoot, T Default) where T : class {
		MethodInfo method = GetType().GetMethod(state.ToString() + "_" + methodRoot, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

		if (method != null) {
			return Delegate.CreateDelegate(typeof(T), this, method) as T;
		}

		return Default;
	}
	#endregion

	#region functions
	void Update() {
		DoUpdate();
	}
    void LateUpdate() {
		DoLateUpdate();
	}
    void FixedUpdate() {
		DoFixedUpdate();
	}
	#endregion
}
