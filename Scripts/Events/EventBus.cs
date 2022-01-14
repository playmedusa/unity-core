using System;
using System.Collections.Generic;

public class EventBus
{
	static EventBus _instanceInternal;
	static EventBus Instance
	{
		get
		{
			if (_instanceInternal == null)
			{
				_instanceInternal = new EventBus();
			}

			return _instanceInternal;
		}
	}

	public delegate void EventDelegate<T>(T e) where T : GameEvent;
	private delegate void EventDelegate(GameEvent e);

	private Dictionary<Type, EventDelegate> delegates = new Dictionary<Type, EventDelegate>();
	private Dictionary<Delegate, EventDelegate> delegateLookup = new Dictionary<Delegate, EventDelegate>();

	static public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
	{
		// Early-out if we've already registered this delegate
		if (Instance.delegateLookup.ContainsKey(del))
			return;

		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		EventDelegate internalDelegate = (e) => del((T)e);
		Instance.delegateLookup[del] = internalDelegate;

		EventDelegate tempDel;
		if (Instance.delegates.TryGetValue(typeof(T), out tempDel))
		{
			Instance.delegates[typeof(T)] = tempDel += internalDelegate;
		}
		else
		{
			Instance.delegates[typeof(T)] = internalDelegate;
		}
	}


	static public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
	{
		EventDelegate internalDelegate;
		if (Instance.delegateLookup.TryGetValue(del, out internalDelegate))
		{
			EventDelegate tempDel;
			if (Instance.delegates.TryGetValue(typeof(T), out tempDel))
			{
				tempDel -= internalDelegate;
				if (tempDel == null)
				{
					Instance.delegates.Remove(typeof(T));
				}
				else
				{
					Instance.delegates[typeof(T)] = tempDel;
				}
			}

			Instance.delegateLookup.Remove(del);
		}
	}

	static public void Raise(GameEvent e)
	{
		EventDelegate del;
		if (Instance.delegates.TryGetValue(e.GetType(), out del))
		{
			del.Invoke(e);
		}
	}

}