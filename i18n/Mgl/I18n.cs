﻿using Lib.SimpleJSON;
using System;
using UnityEngine;
using System.Linq;

namespace Mgl
{
	public class I18n : Singleton<I18n>
	{
		private JSONNode translationData = null;

		protected string[] locales = new string[] { "en-US", "fr-FR", "es-ES", "de-DE", "it-IT", "jp-JP", "zh-CN", "es-US", "es-419" };
		public static int localeId
		{
			get;
			private set;
		}

		private string _currentLocale = "en-US";
		private string _localePath = "Locales/";
		private bool _isLoggingMissing = true;

		public delegate void LanguageChanged();
	    public event LanguageChanged OnLanguageChanged;

		override protected void Init()
		{
			InitCurrentLocale();
			SetLocale(_currentLocale);
			DontDestroyOnLoad(instance.gameObject);
		}

		void InitCurrentLocale()
		{
			if (PlayerPrefs.HasKey("Language"))
			{
				_currentLocale = PlayerPrefs.GetString("Language");
				return;
			}

			switch (Application.systemLanguage)
			{
				case SystemLanguage.Catalan: _currentLocale = "es-ES"; break;
				case SystemLanguage.Spanish: _currentLocale = "es-ES"; break;
				case SystemLanguage.English: _currentLocale = "en-US"; break;
				case SystemLanguage.French: _currentLocale = "fr-FR"; break;
				case SystemLanguage.German: _currentLocale = "de-DE"; break;
				case SystemLanguage.Italian: _currentLocale = "it-IT"; break;
				case SystemLanguage.Japanese: _currentLocale = "jp-JP"; break;
				case SystemLanguage.ChineseSimplified: _currentLocale = "zh-CN"; break;
				case SystemLanguage.ChineseTraditional: _currentLocale = "zh-CN"; break;
				case SystemLanguage.Unknown: _currentLocale = "en-ES"; break;
			}

			PlayerPrefs.SetString("Language", _currentLocale);
		}

		void InitConfig()
		{
			if (locales.Contains(_currentLocale))
			{
				string localConfigPath = _localePath + _currentLocale;
				// Read the file as one string.
#if UNITY_EDITOR
				Debug.Log("Loading locale " + localConfigPath);
#endif
				TextAsset configText = Resources.Load(localConfigPath) as TextAsset;
				if (configText != null)
				{
					translationData = JSON.Parse(configText.text);
				}
			}
			else if (_isLoggingMissing)
			{
				Debug.Log("Missing: locale [" + _currentLocale + "] not found in supported list");
			}
		}

		public static string GetLocale()
		{
			return instance._currentLocale;
		}

		public static void SetLocale(string newLocale = null)
		{
			PlayerPrefs.SetString("Language", newLocale);
			instance.Configure(newLocale: newLocale);
			localeId = Array.FindIndex(instance.locales, w => w == instance._currentLocale);
			instance.OnLanguageChanged?.Invoke();
			
		}

		public static void SetPath(string localePath = null)
		{
			instance.Configure(localePath: localePath);
		}

		public void Configure(string localePath = null, string newLocale = null, bool? logMissing = null, string[] locales = null)
		{
			if (localePath != null)
			{
				_localePath = localePath;
			}
			if (newLocale != null)
			{
				_currentLocale = newLocale;
			}
			if (logMissing.HasValue)
			{
				_isLoggingMissing = logMissing.Value;
			}
			if (locales != null)
			{
				this.locales = (string[])locales.Clone();
			}
			InitConfig();
		}

		public static string t(string key, params object[] args)
		{
			return instance.__(key, args);
		}

		public static string T(string key, params object[] args)
		{
			return instance.__(key, args).ToUpper();
		}

		public string __(string key, params object[] args)
		{
			if (key.Length == 0) return "";

			if (translationData == null)
			{
				InitConfig();
			}
			string translation = key;
			if (translationData[key] != null)
			{
				// if this key is a direct string
				if (translationData[key].Count == 0)
				{
					translation = translationData[key];
				}
				else
				{
					translation = FindSingularOrPlural(key, args);
				}
				// check if we have embeddable data
				if (args.Length > 0)
				{
					translation = string.Format(translation, args);
				}
			}
			else if (_isLoggingMissing)
			{
				//Debug.Log("Missing translation for:" + key);
			}
			return translation;
		}

		string FindSingularOrPlural(string key, object[] args)
		{
			JSONClass translationOptions = translationData[key].AsObject;
			string translation = key;
			string singPlurKey;
			// find format to try to use
			switch (GetCountAmount(args))
			{
				case 0:
					singPlurKey = "zero";
					break;
				case 1:
					singPlurKey = "one";
					break;
				default:
					singPlurKey = "other";
					break;
			}
			// try to use this plural/singular key
			if (translationOptions[singPlurKey] != null)
			{
				translation = translationOptions[singPlurKey];
			}
			else if (_isLoggingMissing)
			{
				Debug.Log("Missing singPlurKey:" + singPlurKey + " for:" + key);
			}
			return translation;
		}

		int GetCountAmount(object[] args)
		{
			int argOne = 0;
			// if arguments passed, try to parse first one to use as count
			if (args.Length > 0 && IsNumeric(args[0]))
			{
				argOne = Math.Abs(Convert.ToInt32(args[0]));
				if (argOne == 1 && Math.Abs(Convert.ToDouble(args[0])) != 1)
				{
					// check if arg actually equals one
					argOne = 2;
				}
				else if (argOne == 0 && Math.Abs(Convert.ToDouble(args[0])) != 0)
				{
					// check if arg actually equals one
					argOne = 2;
				}
			}
			return argOne;
		}

		bool IsNumeric(System.Object Expression)
		{
			if (Expression == null || Expression is DateTime)
				return false;

			if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
				return true;

			return false;
		}

	}
}
