using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool
{
	public GameObject pooledObject;
	public bool willGrow = true;

	public List<GameObject> pooledObjects;

	GameObject prefab;
	Transform parent;

	public ObjectPool(GameObject prefab, bool willGrow, int pooledAmount = 10, Transform parent = null)
	{
		this.prefab = prefab;
		this.willGrow = willGrow;
		this.parent = parent;
		pooledObjects = new List<GameObject>();
		for (int i = 0; i < pooledAmount; i++)
		{
			AddPrefab();
		}
	}

	void AddPrefab()
	{
		GameObject obj = (GameObject)MonoBehaviour.Instantiate(prefab, parent);
		obj.SetActive(false);
		pooledObjects.Add(obj);
	}

	public GameObject GetPooledObject()
	{
		for (int i = 0; i < pooledObjects.Count; i++)
		{
			if (pooledObjects[i] == null)
			{
				GameObject obj = (GameObject)MonoBehaviour.Instantiate(prefab);
				obj.SetActive(false);
				pooledObjects[i] = obj;
				return pooledObjects[i];
			}
			if (!pooledObjects[i].activeInHierarchy)
			{
				return pooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)MonoBehaviour.Instantiate(prefab, parent);
			pooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

}