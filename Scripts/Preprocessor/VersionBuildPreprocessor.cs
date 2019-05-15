#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public class VersionBuildPreprocessor : IPreprocessBuildWithReport
{

	private string versionPath = "/StreamingAssets/" + GameVersionHelper.fileName;

	public int callbackOrder { get { return 0; } }
	public void OnPreprocessBuild(BuildReport report)
	{
		Debug.LogFormat("Building  version {0} at path {1}", report.summary.platform, report.summary.outputPath);
		string bundle;
		switch (report.summary.platform)
		{
			case BuildTarget.iOS:
				bundle = PlayerSettings.iOS.buildNumber;
				break;
			case BuildTarget.Android:
				bundle = PlayerSettings.Android.bundleVersionCode.ToString();
				break;
			default:
				bundle = "";
				break;
		}

		GameVersion gameVersion = new GameVersion(PlayerSettings.bundleVersion, bundle);
		string dataAsJson = JsonUtility.ToJson(gameVersion);

		string filePath = Application.dataPath + versionPath;
		File.WriteAllText(filePath, dataAsJson);
	}

}
#endif
