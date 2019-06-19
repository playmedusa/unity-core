using System.Linq;
using UnityEngine;

/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on editor, null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
/// </summary>
/// <typeparam name="T">Type of the singleton</typeparam>

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
	static T _instance = null;
	public static T instance
	{
		get
		{
			if (!_instance)
				_instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
			if (!_instance)
			{
				var assets = Resources.LoadAll<T>("SingletonScriptables/");
				if (assets.Length > 0)
					_instance = assets[0];
				else
				{
					Debug.LogError("Singleton missingin resources folder");
					CreateInstance<T>();
				}
			}
			return _instance;
		}
	}
}