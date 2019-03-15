using UnityEngine;
using System;
using System.Collections.Generic;

namespace InputMapper
{

	//https://en.wikipedia.org/wiki/Attitude_control
	public enum Actuator
	{
		ForwardAxis,
		HorizontalAxis,
		WeaponSelector,

		Jaw,
		Roll,
		Pitch,

		Channel1,
		Channel2,

		HorizontalDash,
		Boost,
		Use,
		UTurn,

		ClosestTarget,
		CicleNextTarget,
		CiclePreviousTarget,

		Start
	}

	public enum InputType
	{
		None,
		KeyCode,
		Axis,
		Button
	}

	public enum InputModifier
	{
		Single,
		Double,
		Hold
	}

	public enum InputDevice
	{
		Keyboard,
		JoyPad
	}

	[Serializable]
	public class InputKey
	{
		float holdThreshold = 0.35f;

		public KeyCode keyCode;
		public string axisOrButton;
		public InputType type;
		[SerializeField]
		float _value;
		public float value
		{
			get { return _value; }
			set
			{
				if (value == 0)
				{
					if (lastStampUp < currentStampDown)
						lastStampUp = Time.time;
				}
				else if (Mathf.Abs(value) == 1)
				{
					if (currentStampDown <= lastStampUp)
					{
						lastStampDown = currentStampDown;
						currentStampDown = Time.time;
					}
				}
				_value = value;
			}
		}
		public bool invertValue;
		public InputModifier modifier;
		[NonSerialized]
		float currentStampDown = 0;
		[NonSerialized]
		float lastStampDown = 0;
		[NonSerialized]
		float lastStampUp = 0;
		[NonSerialized]
		bool holding;
		public bool isConsecutiveDown
		{
			get
			{
				if (currentStampDown <= lastStampUp)
					return false;

				bool status = currentStampDown - lastStampDown < holdThreshold;
				if (status)
					currentStampDown = lastStampUp = 0;
				return status;
			}
		}
		public bool isHold
		{
			get
			{
				if (Mathf.Abs(value) == 1)
				{
					if (!holding && Time.time - currentStampDown > holdThreshold)
					{
						holding = true;
						return true;
					}
				}
				else if (holding)
				{
					if (lastStampUp == Time.time)
					{
						holding = false;
						return true;
					}
				}
				return false;
			}
		}
	}

	[Serializable]
	public class Bind
	{
		public Actuator actuator;
		public InputKey inputKey;
		public InputKey altKey;
	}

	[Serializable]
	public class InputLog
	{
		public Actuator actuator;
		public InputKey inputKey;
		public float value;
		public int frame;
		public float deltaTime;
	}

	[Serializable]
	public class PersistedInput
	{
		public PersistedInput()
		{
			persistedInputList = new List<InputLog>();
		}
		public List<InputLog> persistedInputList;
	}

	[CreateAssetMenu(menuName = "ScriptableObjects/Input")]
	public class InputSO : ScriptableObject
	{
		public string ControllerName;
		public InputDevice inputDevice;
		public Bind[] binds;

#if UNITY_EDITOR
		public bool saveToJSON;
		public bool loadFromJSON;

		public void OnValidate()
		{
			if (saveToJSON)
			{
				saveToJSON = false;
				System.IO.File.WriteAllText(
					UnityEditor.AssetDatabase.GetAssetPath(this).Replace(".asset", ".json"),
					JsonUtility.ToJson(this)
				);
			}
			if (loadFromJSON)
			{
				loadFromJSON = false;
				string path = UnityEditor.AssetDatabase.GetAssetPath(this).Replace(".asset", ".json");
				if (System.IO.File.Exists(path))
				{
					string s = System.IO.File.ReadAllText(path);
					JsonUtility.FromJsonOverwrite(s, this);
				}
				else
				{
					Debug.LogWarning("File not found: " + path);
				}
			}
		}
#endif

	}
}
