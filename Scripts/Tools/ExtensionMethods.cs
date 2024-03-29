using UnityEngine;

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

	public static float Remap(this float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	public static float RemapClamped(this float value, float from1, float to1, float from2, float to2)
	{
		return Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, from2, to2);
	}

	public static Color SetAlpha(this Color c, float alpha)
	{
		c.a = alpha;
		return c;
	}

	public static Texture2D GetScreenshot(this RectTransform rt, Camera camera)
	{
		Vector3[] v = new Vector3[4];
		rt.GetWorldCorners(v);
		/*for (int i = 0; i < v.Length; i++)
		{
			v[i] = camera.WorldToScreenPoint(v[i]);
			v[i].y -= Screen.height;
		}*/
		int startX = (int)v[0].x;
		int startY = (int)v[0].y;
		int width = (int)v[3].x - (int)v[0].x;
		int height = (int)v[1].y - (int)v[0].y;
		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
		tex.Apply();
		return tex;
	}
	
	public static Sprite ToSprite(this Texture2D texture)
	{
		return Sprite.Create(
			texture, 
			new Rect(0, 0, texture.width, texture.height), 
			new Vector2(.5f, .5f)
		);
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
		instance.transform.SetParent(parent);
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
		instance.transform.SetParent(parent);
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
	
	public static Vector2 Rotate(this Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
}
