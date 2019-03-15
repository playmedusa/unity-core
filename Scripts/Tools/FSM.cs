using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class FSM<T> : MonoBehaviour
{
	public T currentState
	{
		get;
		protected set;
	}
	protected UnityAction callback = null;

	protected Coroutine fsm = null;

	protected IEnumerator FSMLoop()
	{
		while (Application.isPlaying)
		{
			yield return StartCoroutine(currentState.ToString());
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
			fsm = StartCoroutine(FSMLoop());
		}
	}
}