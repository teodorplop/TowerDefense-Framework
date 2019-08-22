using UnityEngine;
using System;
using System.Collections;

public sealed class StateMachineHandler : MonoBehaviour {
	private class GameState {
		public Func<object, IEnumerator> EnterStateArgs;
		public Func<IEnumerator> ExitState;

		public static IEnumerator DoNothing() { yield break; }
		public static IEnumerator DoNothing(object arg) { yield break; }
		
		public GameState(Func<object, IEnumerator> enterState, Func<IEnumerator> exitState) {
			EnterStateArgs = enterState;
			ExitState = exitState;
		}
		public GameState() {
			EnterStateArgs = DoNothing;
			ExitState = DoNothing;
		}
	}

	private GameState _emptyState, _currentState;
	void Awake() {
		_emptyState = new GameState();
		_currentState = new GameState();
	}
	
	public void SetState(Enum state, object arg, StateMachineBase callingObject) {
		GameState oldGameState = _currentState;

		// Set object state to an empty one
		callingObject.currentState = null;
		_currentState = _emptyState;

		// Create the new state
		Func<object, IEnumerator> enterState = callingObject.ConfigureDelegate<Func<object, IEnumerator>>(state, "EnterState", GameState.DoNothing);
		Func<IEnumerator> exitState = callingObject.ConfigureDelegate<Func<IEnumerator>>(state, "ExitState", GameState.DoNothing);
		GameState newGameState = new GameState(enterState, exitState);

		StartCoroutine(ChangeState(state, arg, callingObject, oldGameState, newGameState));
	}
	private IEnumerator ChangeState(Enum state, object arg, StateMachineBase callingObject, GameState oldState, GameState newState) {
		yield return StartCoroutine(oldState.ExitState());
		yield return StartCoroutine(newState.EnterStateArgs(arg));

		callingObject.currentState = state;
		_currentState = newState;
	}
}
