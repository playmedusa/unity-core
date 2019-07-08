using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ScriptableObjectExtensionMethods
{

	public static bool SaveToJson(this ScriptableObject scriptable, string path)
	{
		try
		{
			System.IO.File.WriteAllText(
				string.Format(path + "{0}.data", scriptable.name),
				JsonUtility.ToJson(scriptable)
			);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool LoadFromJSON(this ScriptableObject scriptable, string path)
	{
		path = string.Format(path + "{0}.data", scriptable.name);
		if (System.IO.File.Exists(path))
		{
			string str = System.IO.File.ReadAllText(path);
			JsonUtility.FromJsonOverwrite(
				str,
				scriptable
			);
			return true;
		}
		return false;
	}

	public static string ToJSON(this ScriptableObject scriptable)
	{
		return JsonUtility.ToJson(scriptable);
	}

	public static void FromJSON(this ScriptableObject scriptable, string json)
	{
		JsonUtility.FromJsonOverwrite(
			json,
			scriptable
		);
	}
}