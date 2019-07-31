using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
	public static float GetRandom(this Vector2 minmax)
	{
		return Random.Range(minmax.x, minmax.y);
	}

	public static int GetRandomInt(this Vector2 minmax)
	{
		return Mathf.RoundToInt(Random.Range(minmax.x, minmax.y));
	}

	public static Color SetAlpha(this Color c, float alpha)
	{
		c.a = alpha;
		return c;
	}

	public static Vector3 ToViewportPointProjected(this Vector3 worldPos, Camera camera)
	{
		Vector3 camNormal = camera.transform.forward;
		Vector3 vectorFromCam = worldPos - camera.transform.position;
		float camNormDot = Vector3.Dot(camNormal, vectorFromCam);
		if (camNormDot <= 0)
		{
			// we are behind the camera forward facing plane, project the position in front of the plane
			Vector3 proj = (camNormal * camNormDot * 1.01f);
			worldPos = camera.transform.position + (vectorFromCam - proj);
		}
		Vector3 projectedPosition = camera.WorldToViewportPoint(worldPos);
		projectedPosition.z = camNormDot;

		return projectedPosition;
	}

	public static GameObject InstanceFromPool(this GameObject prefab, int poolSize, Transform parent)
	{
		GameObject instance = FXPool.getFXObject(prefab, poolSize);
		instance.transform.parent = parent;
		instance.SetActive(true);
		return instance;
	}

	public static GameObject InstanceFromPool(this GameObject[] prefabArray, int maxIndex, int poolSize, Transform parent)
	{
		GameObject prefab = prefabArray[
			Random.Range(
				0,
				Mathf.Clamp(maxIndex, 0, prefabArray.Length)
			)];
		GameObject instance = FXPool.getFXObject(prefab, poolSize);
		instance.transform.parent = parent;
		instance.SetActive(true);
		return instance;
	}

	public static float RaiseFX(this FXSO fx, Vector3 position, Quaternion rotation, Transform parent = null, float scale = 1)
	{
		if (fx != null)
			return fx.Raise(position, rotation, parent, scale);
		return 0;
	}

	public static void Reset(this Transform transform)
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	public static float GetCurrentAnimatorClipLength(this Animator animator)
	{
		AnimatorClipInfo[] aClip = animator.GetCurrentAnimatorClipInfo(0);
		if (aClip.Length > 0)
		{
			return aClip[0].clip.length;
		}
		return 0;
	}
}
