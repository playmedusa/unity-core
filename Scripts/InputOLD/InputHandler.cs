using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using InputMapper;

public class InputHandler : MonoBehaviour
{
	public bool isInteractive
	{
		get;
		private set;
	}
	protected InputDeviceComponent idc
	{
		get
		{
			return InputDeviceComponent.instance;
		}
	}

	protected void SetInteractive(bool status)
	{
		isInteractive = status;
		if (status)
		{
			InputDeviceComponent.instance.AddListener(this);
		}
		else
		{
			InputDeviceComponent.instance.RemoveListener(this);
		}
	}

	virtual public void HandleActuator(Actuator actuator)
	{
		Debug.LogWarning("Handle actuators here");
	}

	protected void SetSelectedGameObject(GameObject target)
	{
		EventSystem.current.SetSelectedGameObject(target);
	}

	protected void SimulateUIClick(GameObject target)
	{
		ExecuteEvents.Execute<IPointerClickHandler>(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
	}

}