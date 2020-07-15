using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class FSMObject<T>
{
	public FSMObject(MonoBehaviour owner)
	{
		this.owner = owner;
	}

	public T currentState
	{
		get;
		protected set;
	}
	protected Dictionary<T, UnityAction> callback = new Dictionary<T, UnityAction>();

	protected Coroutine fsm = null;
	MonoBehaviour owner;

	protected IEnumerator FSMLoop()
	{
		T lastState;
		while (Application.isPlaying)
		{
			lastState = currentState;
			yield return owner.StartCoroutine(currentState.ToString());
			if (callback.TryGetValue(lastState, out var cb))
			{
				cb?.Invoke();
				callback[lastState] = null;
			}
		}
	}

	public virtual void ChangeState(T nextState, UnityAction onChangeStateCallback = null)
	{
		callback[nextState] = onChangeStateCallback;
		currentState = nextState;
		if (fsm == null)
		{
			fsm = owner.StartCoroutine(FSMLoop());
		}
	}

	public void StopFSM()
	{
		if (fsm != null)
			owner.StopCoroutine(fsm);
		fsm = null;
	}

}
