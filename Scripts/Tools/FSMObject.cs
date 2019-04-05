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
	protected UnityAction callback = null;
	protected Coroutine fsm = null;
	MonoBehaviour owner;

	protected IEnumerator FSMLoop()
	{
		while (Application.isPlaying)
		{
			yield return owner.StartCoroutine(currentState.ToString());
			if (callback != null)
				callback.Invoke();
			callback = null;
		}
	}

	public virtual void ChangeState(T nextState)
	{
		currentState = nextState;
		if (fsm == null)
		{
			fsm = owner.StartCoroutine(FSMLoop());
		}
	}
}