using System.Collections;
using UnityEngine;

public class SharedLoopAnimations : MonoBehaviour {

	public float changeInValue;
	public float animationSpeed;
	[Range(0,1)]
	public float offset;
	public bool randomizeOffset;

	public enum AvailableAnimations {
		Squeeze,
		Rock,
		Hover,
		Sway,
		HoverOnZ
	}
	public AvailableAnimations itemAnimation;

	void OnEnable () {
		StartCoroutine (Animation ());
	}
	
	void OnValidate () {
		if (randomizeOffset) {
			randomizeOffset = false;
			offset = Random.value;
		}
	}

	public IEnumerator Animation () {
		//Debug.Log (name + " " + changeInValue);
		Vector3 pivotLocation = transform.localPosition;
		Vector3 pivotScale = transform.localScale;
		Quaternion pivotRotation = transform.localRotation;
		float rampMod = 1;
		float t = offset;
		while (true)  {
			UpdateTime (ref t, ref rampMod);
			switch (itemAnimation) {
				case AvailableAnimations.Squeeze:
					Squeeze (t, pivotScale);
					break;
				case AvailableAnimations.Rock:
					Rock (t, pivotRotation);
					break;
				case AvailableAnimations.Hover:
					Hover (t, pivotLocation);
					break;
				case AvailableAnimations.Sway:
					Sway (t, pivotLocation);
					break;
				case AvailableAnimations.HoverOnZ:
					HoverOnZ (t, pivotLocation);
					break;
			}
			yield return 0;
		}
	}

	void UpdateTime (ref float t, ref float rampMod) {
		t += Time.deltaTime * rampMod * animationSpeed;
		if (t < 0 || t > 1 ) {
			rampMod *= -1;
			t = Mathf.Clamp01(t);
		}
	}

	void Squeeze (float t, Vector3 pivotScale) {
		float sx = PennerAnimation.CubicEaseInOut (t, pivotScale.x * 1 - changeInValue, pivotScale.x * changeInValue, 1);
		float sy = PennerAnimation.CubicEaseInOut (1-t, pivotScale.y * 1 - changeInValue, pivotScale.y * changeInValue, 1);
		
		transform.localScale = new Vector3 (sx, sy, pivotScale.z);
	}

	void Rock (float t, Quaternion pivotRotation) {
		Vector3 angles = pivotRotation.eulerAngles;
		float z = PennerAnimation.CubicEaseInOut (t, 0, 1, 1);
		angles.z = (z - 0.5f) * changeInValue;

		transform.localRotation = Quaternion.Euler (angles);
	}

	void Hover (float t, Vector3 pivotLocation) {
		float sy = PennerAnimation.QuadEaseInOut (t, pivotLocation.y, changeInValue, 1);
		
		transform.localPosition = new Vector3 (pivotLocation.x, sy, pivotLocation.z);
	}

	void Sway (float t, Vector3 pivotLocation) {
		float sx = PennerAnimation.QuadEaseInOut (t, pivotLocation.x, changeInValue, 1);
		
		transform.localPosition = new Vector3 (sx, pivotLocation.y, pivotLocation.z);
	}
	
	void HoverOnZ (float t, Vector3 pivotLocation) {
		float sz = PennerAnimation.QuadEaseInOut (t, pivotLocation.z, changeInValue, 1);
		
		transform.localPosition = new Vector3 (pivotLocation.x, pivotLocation.z, sz);
	}


}
