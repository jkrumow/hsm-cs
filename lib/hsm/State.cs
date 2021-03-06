﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public static class ExtensionMethods {
		public static T OnEnter<T>(this T state, Action action) where T : State {
			state.enterAction = action;
			state.enterActionWithData = null;
			return state;
		}

		public static T OnEnter<T>(this T state, Action<Dictionary<string, object>> action) where T : State {
			state.enterActionWithData = action;
			state.enterAction = null;
			return state;
		}

		public static T OnEnter<T>(this T state, Action<State, State, Dictionary<string, object>> action) where T : State {
			state.enterActionWithStatesAndData = action;
			state.enterActionWithData = null;
			state.enterAction = null;
			return state;
		}

		public static T OnExit<T>(this T state, Action action) where T : State {
			state.exitAction = action;
			state.exitActionWithData = null;
			return state;
		}

		public static T OnExit<T>(this T state, Action<Dictionary<string, object>> action) where T : State {
			state.exitActionWithData = action;
			state.exitAction = null;
			return state;
		}

		public static T OnExit<T>(this T state, Action<State, State, Dictionary<string, object>> action) where T : State {
			state.exitActionWithStatesAndData = action;
			state.exitActionWithData = null;
			state.exitAction = null;
			return state;
		}
		
		public static T AddHandler<T>(this T state, string eventName, State target) where T : State {
			state.createHandler(eventName, target, TransitionKind.External, null, null);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind) where T : State {
			state.createHandler(eventName, target, kind, null, null);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, Action<Dictionary<string, object>> action) where T : State {
			state.createHandler(eventName, target, TransitionKind.External, action, null);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind, Action<Dictionary<string, object>> action) where T : State {
			state.createHandler(eventName, target, kind, action, null);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, Func<Dictionary<string, object>, bool> guard) where T : State {
			state.createHandler(eventName, target, TransitionKind.External, null, guard);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind, Func<Dictionary<string, object>, bool> guard) where T : State {
			state.createHandler(eventName, target, kind, null, guard);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind, Action<Dictionary<string, object>> action, Func<Dictionary<string, object>, bool> guard) where T : State {
			state.createHandler(eventName, target, kind, action, guard);
			return state;
		}
	}
	
	public class State {
		[SerializeField]
		public string id;
		public StateMachine owner;
		public Action enterAction = null;
		public Action<Dictionary<string, object>> enterActionWithData = null;
		public Action<State, State, Dictionary<string, object>> enterActionWithStatesAndData = null;
		public Action exitAction = null;
		public Action<Dictionary<string, object>> exitActionWithData = null;
		public Action<State, State, Dictionary<string, object>> exitActionWithStatesAndData = null;
		public Dictionary<string, List<Handler>> handlers = new Dictionary<string, List<Handler>>();

		public State(string pId) {
			id = pId;
		}

		public virtual void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			if (enterAction != null) {
				enterAction.Invoke();
			}
			if (enterActionWithData != null) {
				enterActionWithData.Invoke(data);
			}
			if (enterActionWithStatesAndData != null) {
				enterActionWithStatesAndData.Invoke(sourceState, targetstate, data);
			}
		}
		
		public virtual void Exit(State sourceState, State targetstate, Dictionary<string, object> data) {
			if (exitAction != null) {
				exitAction.Invoke();
			}
			if (exitActionWithData != null) {
				exitActionWithData.Invoke(data);
			}
			if (exitActionWithStatesAndData != null) {
				exitActionWithStatesAndData.Invoke(sourceState, targetstate, data);
			}
		}

		public void createHandler(string eventName, State target, TransitionKind kind, Action<Dictionary<string, object>> action, Func<Dictionary<string, object>, bool> guard) {
			Handler handler = new Handler(target, kind, action, guard);
			if (!handlers.ContainsKey(eventName)) {
				handlers[eventName] = new List<Handler>();
			}
			handlers[eventName].Add(handler);
		}

		public bool hasAncestor(State other) {
			if (owner.container == null) {
				return false;
			}
			if (owner.container == other) {
				return true;
			}
			return owner.container.hasAncestor(other);
		}

		public bool hasAncestorStateMachine(StateMachine stateMachine) {
			for (var i = 0; i < owner.getPath().Count; ++i) {
				if (owner.getPath()[i] == stateMachine) {
					return true;
				}
			}
			return false;
		}
	}
}
