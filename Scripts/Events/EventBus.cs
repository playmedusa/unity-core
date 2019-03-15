using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class EventBus
{
	static EventBus instanceInternal = null;
	static EventBus instance
	{
		get
		{
			if (instanceInternal == null)
			{
				instanceInternal = new EventBus();
			}

			return instanceInternal;
		}
	}

	public delegate void EventDelegate<T>(T e) where T : GameEvent;
	private delegate void EventDelegate(GameEvent e);

	private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
	private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();

	static public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
	{
		// Early-out if we've already registered this delegate
		if (instance.delegateLookup.ContainsKey(del))
			return;

		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		EventDelegate internalDelegate = (e) => del((T)e);
		instance.delegateLookup[del] = internalDelegate;

		EventDelegate tempDel;
		if (instance.delegates.TryGetValue(typeof(T), out tempDel))
		{
			instance.delegates[typeof(T)] = tempDel += internalDelegate;
		}
		else
		{
			instance.delegates[typeof(T)] = internalDelegate;
		}
	}


	static public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
	{
		EventDelegate internalDelegate;
		if (instance.delegateLookup.TryGetValue(del, out internalDelegate))
		{
			EventDelegate tempDel;
			if (instance.delegates.TryGetValue(typeof(T), out tempDel))
			{
				tempDel -= internalDelegate;
				if (tempDel == null)
				{
					instance.delegates.Remove(typeof(T));
				}
				else
				{
					instance.delegates[typeof(T)] = tempDel;
				}
			}

			instance.delegateLookup.Remove(del);
		}
	}

	static public void Raise(GameEvent e)
	{
		EventDelegate del;
		if (instance.delegates.TryGetValue(e.GetType(), out del))
		{
			del.Invoke(e);
		}
	}

}