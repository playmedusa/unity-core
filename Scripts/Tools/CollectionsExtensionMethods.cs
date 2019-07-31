using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CollectionsExtensionMethods
{
	public static Vector3 GetEmptySpacePoint(this Vector3 pivot, float searchRadius, float freeSpaceRadius = 5)
	{
		Vector3 p = Vector3.zero;
		Collider[] hitColliders = null;
		int tries = 0;
		do
		{
			tries++;
			p = pivot + Random.onUnitSphere * searchRadius;
			hitColliders = Physics.OverlapSphere(p, freeSpaceRadius);
		} while (hitColliders.Length > 0 && tries < 100);
		return tries >= 100 ? Vector3.zero : p;
	}

	public static Vector3 GetRandomPointOnMeshSurface(this GameObject target, out Vector3 normal)
	{
		return target.GetRandomPointOnMeshSurface(Vector3.zero, out normal);
	}

	public static Vector3 GetRandomPointOnMeshSurface(this GameObject target, Vector3 offset, out Vector3 normal)
	{
		Bounds bounds = target.GetComponent<Renderer>().bounds;
		Vector3 sph = Random.onUnitSphere;
		Vector3 right = target.transform.right * sph.x * bounds.size.magnitude;
		Vector3 up = target.transform.up * sph.y * bounds.size.magnitude;
		Vector3 from = target.transform.position + right + up + offset;
		Vector3 towardsCenter = target.transform.position - from + offset;
		RaycastHit[] hits = Physics.RaycastAll(from, towardsCenter.normalized, towardsCenter.magnitude);
		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].collider.gameObject == target)
			{
				Debug.DrawLine(from, hits[i].point, Color.magenta, 5);
				normal = hits[i].normal;
				return hits[i].point;
			}
		}
		normal = Vector3.zero;
		return Vector3.zero;
	}

	private static System.Random rng = new System.Random();
	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static T GetRandom<T>(this T[] array)
	{
		if (array.Length == 0)
		{
			Debug.LogError("Empty array!");
			return default(T);
		}
		return (array[rng.Next(0, array.Length)]);
	}

	public static int RouletteWheel<T>(this List<T> slotsList, System.Func<T, float> p)
	{
		if (slotsList.Count == 0) return -1;
		if (slotsList.Count == 1) return 0;

		float[] slots = new float[slotsList.Count];
		for (int i = 0; i < slotsList.Count; i++)
			slots[i] = p(slotsList[i]);
		return slots.RouletteWheel();
	}

	public static int RouletteWheel(this float[] slots)
	{
		if (slots.Length == 0) return -1;

		int slotsCount = slots.Length;
		float sum = 0;

		for (int i = 0; i < slotsCount; i++)
		{
			sum += slots[i];
		}

		float goal = Random.Range(0f, sum);
		float delta = 0;
		int slotIndex = 0;
		while (slotIndex < slotsCount && delta < goal)
		{
			delta += slots[slotIndex];
			slotIndex++;
		}

		return slotIndex > 0 ? slotIndex - 1 : 0;
	}

	public static int GetChildIndex(this Transform transform, GameObject child)
	{
		int index = -1;
		for (int i = 0; i < transform.childCount; i++)
		{
			var t = transform.GetChild(i);
			if (t.gameObject == child)
			{
				index = i;
				break;
			}
		}
		return index;
	}
}