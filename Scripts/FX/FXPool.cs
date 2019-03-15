using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPool : MonoBehaviour
{

	private static FXPool _instance;
	public static FXPool instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<FXPool>();
			if (_instance == null)
			{
				GameObject go = new GameObject();
				go.name = "FXPool";
				_instance = go.AddComponent<FXPool>();
			}
			return _instance;
		}
	}

	Dictionary<GameObject, ObjectPool> fxPool;

	void Awake()
	{
		fxPool = new Dictionary<GameObject, ObjectPool>();
	}

	static public GameObject getFXObject(GameObject fx, int poolSize)
	{
		GameObject go;
		if (instance.fxPool.ContainsKey(fx))
		{
			go = instance.fxPool[fx].GetPooledObject();
		}
		else
		{
			instance.fxPool.Add(fx, new ObjectPool(fx, true, poolSize, instance.transform));
			go = instance.fxPool[fx].GetPooledObject();
		}

		return go;
	}
}
