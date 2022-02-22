using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharedOneTimeAnimations : MonoBehaviour {


	public static IEnumerator ElasticGrowth (Transform t, float randomOffsetMin, float randomOffsetMax, float animationTime) {
		yield return new WaitForSeconds (randomOffsetMin + Random.value * randomOffsetMax);
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			float s = PennerAnimation.BounceEaseOut (elapsedTime, 0, 1, animationTime);
			t.localScale = Vector3.one * s;
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		t.localScale = Vector3.one;
	}

	public static IEnumerator ElasticShrink (Transform t, float randomOffsetMin, float randomOffsetMax, float animationTime) {
		yield return new WaitForSeconds (randomOffsetMin + Random.value * randomOffsetMax);
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			float s = PennerAnimation.BounceEaseOut (elapsedTime, 1, -1, animationTime);
			t.localScale = Vector3.one * s;
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		t.localScale = Vector3.zero;
	}


	public static IEnumerator FallRectTransform (RectTransform rt, float randomOffsetMin, float randomOffsetMax, float animationTime, Vector3 to) {
		float elapsedTime = 0;
		Vector3 from = to + Vector3.up * Screen.height * 2;
		rt.anchoredPosition = from;
		while (elapsedTime < animationTime) {
			float t = PennerAnimation.BounceEaseOut (elapsedTime, 0, 1, animationTime);
			rt.anchoredPosition = Vector3.Lerp(from, to, t);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}

		rt.anchoredPosition = to;
	}

	public static IEnumerator TranslateRectTransform (RectTransform rt, float randomOffsetMin, float randomOffsetMax, float animationTime, Vector3 from, Vector3 to) {
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			float t = PennerAnimation.QuadEaseInOut (elapsedTime, 0, 1, animationTime);
			rt.anchoredPosition = Vector3.Lerp(from, to, t);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}

		rt.anchoredPosition = to;
	}

	public static IEnumerator Translate (Transform tr, float randomOffsetMin, float randomOffsetMax, float animationTime, Vector3 from, Vector3 to) {
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			float t = PennerAnimation.QuadEaseInOut (elapsedTime, 0, 1, animationTime);
			tr.position = Vector3.Lerp(from, to, t);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}

		tr.position = to;
	}

	public static IEnumerator LinearFade (CanvasGroup target, float start, float end, float animationTime) {
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			target.alpha = Mathf.Lerp(start, end, elapsedTime/animationTime);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		target.alpha = end;
	}
	
	public static IEnumerator LinearFade (Image target, float start, float end, float animationTime) {
		float elapsedTime = 0;
		Color c = Color.white;
		while (elapsedTime < animationTime) {
			c.a = Mathf.Lerp(start, end, elapsedTime/animationTime);
			target.color = c;
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		c.a = end;
		target.color = c;
	}

	public static IEnumerator LinearFadeFromTargetColor (Image target, float start, float end, float animationTime) {
		float elapsedTime = 0;
		Color c = target.color;
		while (elapsedTime < animationTime) {
			c.a = Mathf.Lerp(start, end, elapsedTime/animationTime);
			target.color = c;
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		c.a = end;
		target.color = c;
	}

	public static IEnumerator ExpoFade (Image target, float start, float end, float animationTime) {
		float elapsedTime = 0;
		Color c = Color.white;
		while (elapsedTime < animationTime) {
			c.a = PennerAnimation.ExpoEaseIn(
				elapsedTime,
				start,
				end - start,
				animationTime
			);
			target.color = c;
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		c.a = end;
		target.color = c;
	}

	public static IEnumerator CubicFade (Image target, float start, float end, float animationTime) {
		float elapsedTime = 0;
		Color c = Color.white;
		while (elapsedTime < animationTime) {
			c.a = PennerAnimation.CubicEaseIn(
				elapsedTime,
				start,
				end - start,
				animationTime
			);
			target.color = c;
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		c.a = end;
		target.color = c;
	}

	public static IEnumerator BackEaseSlideInY (Transform t, float targetPosition, float animationTime) {
		float elapsedTime = 0;
		float startPosition = t.localPosition.y;
		while (elapsedTime < animationTime) {
			t.localPosition = new Vector2 (
				t.localPosition.x,
				PennerAnimation.BackEaseInOut(elapsedTime, startPosition, targetPosition-startPosition, animationTime)
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		t.localPosition = new Vector2 (
			t.localPosition.x,
			targetPosition
		);
	}

	public static IEnumerator BackEaseSlideInY (RectTransform rt, float targetPosition, float animationTime) {
		float elapsedTime = 0;
		float startPosition = rt.anchoredPosition.y;
		while (elapsedTime < animationTime) {
			rt.anchoredPosition = new Vector2 (
				rt.anchoredPosition.x,
				PennerAnimation.BackEaseInOut(elapsedTime, startPosition, targetPosition-startPosition, animationTime)
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		rt.anchoredPosition = new Vector2 (
			rt.anchoredPosition.x,
			targetPosition
		);
	}

	public static IEnumerator BackEaseSlideInX (RectTransform rt, float targetPosition, float animationTime) {
		float elapsedTime = 0;
		float startPosition = rt.anchoredPosition.x;
		while (elapsedTime < animationTime) {
			rt.anchoredPosition = new Vector2 (
				PennerAnimation.BackEaseInOut(elapsedTime, startPosition, targetPosition-startPosition, animationTime),
				rt.anchoredPosition.y
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		rt.anchoredPosition = new Vector2 (
			targetPosition,
			rt.anchoredPosition.y
		);
	}

	public static IEnumerator ElasticSlideInY (Transform t, float targetPosition, float animationTime) {
		float elapsedTime = 0;
		float startPosition = t.localPosition.y;
		while (elapsedTime < animationTime) {
			t.localPosition = new Vector2 (
				t.localPosition.x,
				PennerAnimation.ElasticEaseOut(elapsedTime, startPosition, targetPosition-startPosition, animationTime)
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		t.localPosition = new Vector2 (
			t.localPosition.x,
			targetPosition
		);
	}

	public static IEnumerator CubicSlideInY (RectTransform rt, float targetPosition, float animationTime) {
		float elapsedTime = 0;
		float startPosition = rt.anchoredPosition.y;
		while (elapsedTime < animationTime) {
			rt.anchoredPosition = new Vector2 (
				rt.anchoredPosition.x,
				PennerAnimation.CubicEaseOut(elapsedTime, startPosition, targetPosition-startPosition, animationTime)
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		rt.anchoredPosition = new Vector2 (
			rt.anchoredPosition.x,
			targetPosition
		);
	}

	public static IEnumerator CubicSlideOutY (RectTransform rt, float targetPosition, float animationTime) {
		float elapsedTime = 0;
		float startPosition = rt.anchoredPosition.y;
		while (elapsedTime < animationTime) {
			rt.anchoredPosition = new Vector2 (
				rt.anchoredPosition.x,
				PennerAnimation.CubicEaseIn(elapsedTime, startPosition, targetPosition-startPosition, animationTime)
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		rt.anchoredPosition = new Vector2 (
			rt.anchoredPosition.x,
			targetPosition
		);
	}

	public static IEnumerator ElasticInScaleFromTo (RectTransform t, float f, float to, float animationTime) {
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			t.localScale = Vector3.one * PennerAnimation.ElasticEaseIn(elapsedTime,
					f,
					to - f,
					animationTime);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		t.localScale = Vector3.one * to;
	}

	public static IEnumerator EaseBackInScaleFromTo (Transform t, float f, float to, float animationTime) {
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			t.localScale = Vector3.one * PennerAnimation.BackEaseIn(elapsedTime,
					f,
					to - f,
					animationTime);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		t.localScale = Vector3.one * to;
	}

	public static IEnumerator EaseExpoInScaleFromTo (Transform t, float f, float to, float animationTime) {
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			t.localScale = Vector3.one * PennerAnimation.ExpoEaseInOut(elapsedTime,
					f,
					to - f,
					animationTime);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		t.localScale = Vector3.one * to;
	}

	public static IEnumerator EaseBackOutScaleFromTo (RectTransform t, float f, float to, float animationTime) {
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			t.localScale = Vector3.one * PennerAnimation.BackEaseOut(elapsedTime,
					f,
					to - f,
					animationTime);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		t.localScale = Vector3.one * to;
	}

	public static IEnumerator EaseOutShake (Transform target, float animationTime, float strength) {
		Vector3 pivotPosition = target.localPosition;
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			Vector3 p = pivotPosition + Random.insideUnitSphere * PennerAnimation.CubicEaseOut(
				elapsedTime,
				0,
				strength,
				animationTime);
			target.localPosition = p;

			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		target.localPosition = pivotPosition;
	}

	public static IEnumerator FadeTextToFullAlpha(Text text, float animationTime)
    {
	    var color = text.color;
	    text.color = new Color(color.r, color.g, color.b, 0);
	    while (text.color.a < 1.0f)
	    {
		    color = text.color;
            text.color = new Color(color.r, color.g, color.b, color.a + (Time.deltaTime / animationTime));
            yield return null;
        }
    }
	
	public static IEnumerator FadeTextToFullAlpha(TMPro.TextMeshProUGUI text, float animationTime)
	{
		var color = text.color;
		text.color = new Color(color.r, color.g, color.b, 0);
		while (text.color.a < 1.0f)
		{
			color = text.color;
			text.color = new Color(color.r, color.g, color.b, color.a + (Time.deltaTime / animationTime));
			yield return null;
		}
	}
 
    public static IEnumerator FadeTextToZeroAlpha(Text text, float animationTime)
    {
	    var color = text.color;
	    text.color = new Color(color.r, color.g, color.b, 1);
	    while (text.color.a > 0.0f)
	    {
		    color = text.color;
		    text.color = new Color(color.r, color.g, color.b, color.a - (Time.deltaTime / animationTime));
            yield return null;
        }
    }
    
    public static IEnumerator FadeTextToZeroAlpha(TMPro.TextMeshProUGUI text, float animationTime)
    {
	    var color = text.color;
	    text.color = new Color(color.r, color.g, color.b, 1);
	    while (text.color.a > 0.0f)
	    {
		    color = text.color;
		    text.color = new Color(color.r, color.g, color.b, color.a - (Time.deltaTime / animationTime));
		    yield return null;
	    }
    }

	public static IEnumerator TranslateSameSpeed(Transform tr, float randomOffsetMin, float randomOffsetMax, float animationTime, Vector3 from, Vector3 to) {
		float elapsedTime = 0;
		while (elapsedTime < animationTime) {
			tr.position = Vector3.Lerp(from, to, elapsedTime/animationTime);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}

		tr.position = to;
	}

}
