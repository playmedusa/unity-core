using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InputMapper;

public class WheelSelector : InputHandler
{

	public Text selectedButtonText;
	[Header("Required references")]
	public Canvas canvas;
	public EventSystem m_EventSystem;
	public GraphicRaycaster m_Raycaster;
	public RectTransform selector;

	PointerEventData m_PointerEventData;
	float radius = 300;

	protected AnimatedButton selectedButton = null;

	Vector3 lastInput = Vector3.zero;

	void Update()
	{
		if (!isInteractive) return;

		Vector3 input = transform.right * idc.raw(Actuator.Roll) +
			transform.up * idc.raw(Actuator.Pitch);
		if (input.magnitude < 0.5f) return;

		//float screenRatio = (float)canvas.worldCamera.pixelHeight / 1080f;
		input = Vector3.Normalize(input) * radius * canvas.transform.localScale.y;// * screenRatio;
		if (input.magnitude > 0.1f)
			selector.localRotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.up, lastInput, -Vector3.forward) + (Random.value - 0.5f) * 0.01f);
		lastInput = Vector3.Lerp(lastInput, input, Time.deltaTime * 20); // Smooth (to release the stick)
		Vector3 canvasPos = transform.position + lastInput;
		//Vector3 screenPos = RectTransformUtility.PixelAdjustPoint(canvasPos, transform, canvas);

#if UNITY_EDITOR
		Debug.DrawLine(transform.position, canvasPos, Color.yellow);
#endif

		m_PointerEventData = new PointerEventData(m_EventSystem);
		m_PointerEventData.position = canvas.worldCamera.WorldToScreenPoint(canvasPos);

		List<RaycastResult> results = new List<RaycastResult>();
		m_Raycaster.Raycast(m_PointerEventData, results);
		foreach (RaycastResult result in results)
		{
			AnimatedButton ab = result.gameObject.GetComponent<AnimatedButton>();
			if (ab != null)
			{
				SelectButton(ab);
			}
		}

		if (StateViewManager.instance.isUsingMouse)
		{
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				AnimatedButton ab = EventSystem.current.currentSelectedGameObject.GetComponent<AnimatedButton>();
				if (ab != null)
				{
					SelectButton(ab);
				}
			}
		}
	}

	virtual protected void SelectButton(AnimatedButton ab)
	{
		SetSelectedGameObject(ab.gameObject);
		selectedButton = ab;
	}

}
