using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

	protected static T _instance
	{
		get;
		private set;
	}

	public static T instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<T>();
			if (_instance == null)
			{
				GameObject go = new GameObject();
				go.name = typeof(T).ToString();
				_instance = go.AddComponent<T>();
				(_instance as Singleton<T>).Init();
			}
			return _instance;
		}
	}

	virtual protected void Init() { }

}