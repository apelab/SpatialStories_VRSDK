 #if UNITY_EDITOR
 using UnityEditor;
 using UnityEditor.Build;
 class Check_CameraPermisions : IPreprocessBuild
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        if(PlayerSettings.iOS.cameraUsageDescription == string.Empty)
        {
            PlayerSettings.iOS.cameraUsageDescription = "Arkit";
        }
    }
}
#endif