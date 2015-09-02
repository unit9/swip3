using UnityEditor;
using System.Collections.Generic;
using System;

class JenkinsBuildHooks
{
	static string[] SCENES = FindEnabledEditorScenes ();
	static string APP_NAME = "KiaOptimaUnity";
	static string TARGET_DIR = "../../../../../website";

	[MenuItem ("Build/CI/Build Flash Deployment")]
	static void PerformFlashDeploymentBuild ()
	{   
		string target_dir = APP_NAME + ".swf";
		GenericBuild (SCENES, Environment.GetEnvironmentVariable("SWF_EXPORT_PATH") + "/" + APP_NAME, BuildTarget.FlashPlayer, BuildOptions.None);
	}

	private static string[] FindEnabledEditorScenes ()
	{
		List<string> EditorScenes = new List<string> ();
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (!scene.enabled)
				continue;
			EditorScenes.Add (scene.path);
		}
		return EditorScenes.ToArray ();
	}

	static void GenericBuild (string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
	{   
		EditorUserBuildSettings.SwitchActiveBuildTarget (build_target);
		string res = BuildPipeline.BuildPlayer (scenes, target_dir, build_target, build_options);
		if (res.Length > 0) {
			throw new Exception ("BuildPlayer failure: " + res);
		}
	}
}