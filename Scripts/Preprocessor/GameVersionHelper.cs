using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class GameVersion {
	
	public GameVersion (string v, string b) {
		version = v;
		bundle = b;
	}

	public string version;
	public string bundle;
}

public class GameVersionHelper {

	private static GameVersionHelper _instance = null;
	public static GameVersionHelper instance {
		get {
			if (_instance == null) {
				_instance =  new GameVersionHelper();
				_instance.Init();
			}
			return _instance;
		}
	}
	
	static public readonly string fileName = "version.json";
	public GameVersion loadedData;

	public void Init () {
		string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

		#if UNITY_ANDROID && ! UNITY_EDITOR
		WWW reader = new WWW(filePath);
		while (!reader.isDone) { }
		if (string.IsNullOrEmpty(reader.error)) {
			string dataAsJson = reader.text;
			loadedData = JsonUtility.FromJson<GameVersion>(dataAsJson);
		}
		#else
		if(File.Exists(filePath)) {
			string dataAsJson = File.ReadAllText(filePath); 
			loadedData = JsonUtility.FromJson<GameVersion>(dataAsJson);
		}
		#endif

		if (loadedData == null) {
			Debug.LogError("Cannot load game data!");
			loadedData = new GameVersion("0","0");
		}
	}


}