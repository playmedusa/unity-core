using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TweeningExtensionMethods
{
	public static float Map(this float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	public static Vector2 LerpUnclampedFrom(this Vector2 targetPosition, Vector2 offset, float t)
	{
		return Vector2.LerpUnclamped(targetPosition + offset, targetPosition, t);
	}
	
	public static Vector3 LerpUnclampedFrom(this Vector3 targetPosition, Vector3 offset, float t)
	{
		return Vector3.LerpUnclamped(targetPosition + offset, targetPosition, t);
	}

	public static Coroutine StartCoroutine(this UnityEngine.GameObject obj, IEnumerator coroutine)
	{
		MonoBehaviour mb = obj.GetComponent<MonoBehaviour>();
		if (mb == null)
			mb = obj.AddComponent<MonoBehaviour>();
		return mb.StartCoroutine(coroutine);
	}

	public static IEnumerator Tween01(this UnityEngine.Object obj, System.Action<float> step, float animationTime, System.Action callback = null)
	{
		float elapsedTime = 0;
		while (elapsedTime < animationTime)
		{
			step(elapsedTime / animationTime);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		step(1);
		if (callback != null)
			callback();
	}

	public static Coroutine DoTween01(this UnityEngine.Component c, System.Action<float> step, float animationTime, System.Action callback = null)
	{
		return c.gameObject.DoTween01(step, animationTime, callback);
	}

	public static Coroutine DoTween01(this UnityEngine.GameObject obj, System.Action<float> step, float animationTime, System.Action callback = null)
	{
		IEnumerator tween = obj.Tween01(step, animationTime, callback);
		return obj.StartCoroutine(tween);
	}
	
	public static IEnumerator DelayedTween01(this UnityEngine.Object obj, System.Action<float> step, float animationTime, float delayTime, System.Action callback = null)
	{
		step(0f);
		yield return new WaitForSeconds(delayTime);
		yield return obj.Tween01( step, animationTime, callback);
	}
	
	public static Coroutine DoDelayedTween01(this UnityEngine.Component c, System.Action<float> step, float animationTime, float delayTime, System.Action callback = null)
	{
		return c.gameObject.DoDelayedTween01(step, animationTime, delayTime, callback);
	}
	
	public static Coroutine DoDelayedTween01(this UnityEngine.GameObject obj, System.Action<float> step, float animationTime, float delayTime, System.Action callback = null)
	{
		IEnumerator tween = obj.DelayedTween01(step, animationTime, delayTime, callback);
		return obj.StartCoroutine(tween);
	}

	public static IEnumerator UnscaledTween01(this UnityEngine.Object obj, System.Action<float> step, float animationTime, System.Action callback = null)
	{
		float elapsedTime = 0;
		while (elapsedTime < animationTime)
		{
			step(elapsedTime / animationTime);
			elapsedTime += Time.unscaledDeltaTime;
			yield return 0;
		}
		step(1);
		if (callback != null)
			callback();
	}

	public static Coroutine DoUnscaledTween01(this UnityEngine.Component c, System.Action<float> step, float animationTime, System.Action callback = null)
	{
		return c.gameObject.DoUnscaledTween01(step, animationTime, callback);
	}

	public static Coroutine DoUnscaledTween01(this UnityEngine.GameObject obj, System.Action<float> step, float animationTime, System.Action callback = null)
	{
		IEnumerator tween = obj.UnscaledTween01(step, animationTime, callback);
		return obj.StartCoroutine(tween);
	}

	public static float CatmullLerp(this float[] values, float t)
	{
		float timePerSegment = 1.0f / (values.Length - 1);
		float timeInSegment = t / timePerSegment;
		timeInSegment = timeInSegment - (int)timeInSegment;

		float p = (float)(values.Length - 1) * t;

		float a = p - 1;
		float b = p;
		float c = p + 1;
		float d = p + 2;

		a = a < 0 ? 0 : a;
		b = b < values.Length ? b : values.Length - 1;
		c = c < values.Length ? c : values.Length - 1;
		d = d < values.Length ? d : values.Length - 1;

		float p0 = values[(int)a];
		float p1 = values[(int)b];
		float p2 = values[(int)c];
		float p3 = values[(int)d];

		return CatmullRom(p0, p1, p2, p3, timeInSegment, 1.0f, 1.0f);
	}

	public static Vector3 CatmullLerp(this Vector3[] points, float t)
	{
		t = Mathf.Clamp01(t);
		float timePerSegment = 1.0f / (points.Length - 1);
		float timeInSegment = t / timePerSegment;
		timeInSegment = timeInSegment - (int)timeInSegment;

		float p = Mathf.Floor((float)(points.Length - 1) * t);

		float a = p - 1;
		float b = p;
		float c = p + 1;
		float d = p + 2;

		a = a < 0 ? 0 : a;
		b = b < points.Length ? b : points.Length - 1;
		c = c < points.Length ? c : points.Length - 1;
		d = d < points.Length ? d : points.Length - 1;

		Vector3 p0 = points[(int)a];
		Vector3 p1 = points[(int)b];
		Vector3 p2 = points[(int)c];
		Vector3 p3 = points[(int)d];

		return CatmullRom(p0, p1, p2, p3, timeInSegment, 1.0f, 1.0f);
	}

	static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next,
								float elapsedTime, float duration, float tension)
	{
		// References used:
		// p.266 GemsV1
		//
		// tension is often set to 0.5 but you can use any reasonable value:
		// http://www.cs.cmu.edu/~462/projects/assn2/assn2/catmullRom.pdf
		//
		// bias and tension controls:
		// http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/interpolation/

		float percentComplete = elapsedTime / duration;
		float percentCompleteSquared = percentComplete * percentComplete;
		float percentCompleteCubed = percentCompleteSquared * percentComplete;

		return (previous * (-0.5f * percentCompleteCubed +
								   percentCompleteSquared -
							0.5f * percentComplete) +
				start * (1.5f * percentCompleteCubed +
						   -2.5f * percentCompleteSquared + 1.0f) +
				end * (-1.5f * percentCompleteCubed +
							2.0f * percentCompleteSquared +
							0.5f * percentComplete) +
				next * (0.5f * percentCompleteCubed -
							0.5f * percentCompleteSquared));
	}

	static float CatmullRom(float previous, float start, float end, float next,
								float elapsedTime, float duration, float tension)
	{
		// References used:
		// p.266 GemsV1
		//
		// tension is often set to 0.5 but you can use any reasonable value:
		// http://www.cs.cmu.edu/~462/projects/assn2/assn2/catmullRom.pdf
		//
		// bias and tension controls:
		// http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/interpolation/

		float percentComplete = elapsedTime / duration;
		float percentCompleteSquared = percentComplete * percentComplete;
		float percentCompleteCubed = percentCompleteSquared * percentComplete;

		return (previous * (-0.5f * percentCompleteCubed +
								   percentCompleteSquared -
							0.5f * percentComplete) +
				start * (1.5f * percentCompleteCubed +
						   -2.5f * percentCompleteSquared + 1.0f) +
				end * (-1.5f * percentCompleteCubed +
							2.0f * percentCompleteSquared +
							0.5f * percentComplete) +
				next * (0.5f * percentCompleteCubed -
							0.5f * percentCompleteSquared));
	}
}