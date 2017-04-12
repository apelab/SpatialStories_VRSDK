using UnityEngine;
using UnityEditor;

/// <summary>
/// Allows Oculus to build apps from the command line.
/// </summary>
partial class OculusBuildApp
{
	static void SetPCTarget()
	{
		if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneWindows);
		}
#if UNITY_5_5_OR_NEWER
		UnityEditorInternal.VR.VREditor.SetVREnabledOnTargetGroup(BuildTargetGroup.Standalone, true);
#elif UNITY_5_4_OR_NEWER
		UnityEditorInternal.VR.VREditor.SetVREnabled(BuildTargetGroup.Standalone, true);
#endif
		PlayerSettings.virtualRealitySupported = true;
		AssetDatabase.SaveAssets();
	}

	static void SetAndroidTarget()
	{
		EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

		if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		}

#if UNITY_5_5_OR_NEWER
		UnityEditorInternal.VR.VREditor.SetVREnabledOnTargetGroup(BuildTargetGroup.Standalone, true);
#elif UNITY_5_4_OR_NEWER
		UnityEditorInternal.VR.VREditor.SetVREnabled(BuildTargetGroup.Android, true);
#endif
		PlayerSettings.virtualRealitySupported = true;
		AssetDatabase.SaveAssets();
	}
}
