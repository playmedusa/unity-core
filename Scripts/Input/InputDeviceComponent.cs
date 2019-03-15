using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InputMapper;

public class InputDeviceComponent : MonoBehaviour
{

	static InputDeviceComponent _instance;
	public static InputDeviceComponent instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<InputDeviceComponent>();
			if (_instance == null)
				Debug.Log("Create idc");
			return _instance;
		}
	}

	public int playerIndex;
	public bool recordInput;
	[HideInInspector]
	public PersistedInput persistedInput = new PersistedInput();
	public InputSO[] supportedDevices;
	public InputSO currentDevice;
	public StateView pauseTarget;
	public Actuator lastActuator;
	public float lastActuatorStamp = 0;
	public Dictionary<Actuator, float> actions;
	List<InputHandler> listeners;

	public bool this[Actuator action]
	{
		get
		{
			return actions[action] > 0.5f || actions[action] < -0.5f;
		}
	}

	public float raw(Actuator action)
	{
		return actions[action];
	}

	void Awake()
	{
		actions = new Dictionary<Actuator, float>();
		listeners = new List<InputHandler>();

		foreach (Actuator a in System.Enum.GetValues(typeof(Actuator)))
		{
			actions.Add(a, 0f);
		}
		if (currentDevice == null)
			currentDevice = supportedDevices[0];
	}

	public void AddListener(InputHandler ih)
	{
		if (listeners.Contains(ih))
			return;
		listeners.Add(ih);
	}

	public void RemoveListener(InputHandler ih)
	{
		listeners.Remove(ih);
	}

	public void TriggerActuator(Actuator actuator)
	{
		List<InputHandler> localCopy = new List<InputHandler>(listeners.ToArray());
		foreach (var listener in localCopy)// (int i = 0; i < listeners.Count; i++)
		{
			listener.HandleActuator(actuator);
		}
	}

	void OnDestroy()
	{
		if (persistedInput.persistedInputList.Count == 0) return;

		string path = Application.persistentDataPath + string.Format(
			"/P{0}Input_{1}.json", playerIndex, System.DateTime.Now.ToString("s").Replace(":", "-")
		);

		string json = JsonUtility.ToJson(persistedInput);
		System.IO.File.WriteAllText(path, json);
		print("Input saved to: " + path);
	}

}
