using Gaze;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[InitializeOnLoad]
[CustomEditor(typeof(CA_ChangeLayer))]
public class CA_ChangeLayerEditor : Gaze_Editor
{
    public CA_ChangeLayer TargetScript;
    public List<string> LayerNames = new List<string>();

    void OnEnable()
    {
        TargetScript = (CA_ChangeLayer)target;

        for (int i = 8; i <= 31; i++)
        {
            var layerN = LayerMask.LayerToName(i);
            if (layerN.Length > 0) 
                LayerNames.Add(layerN);
        }
    }

    public override void Gaze_OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Target Game Object: ");
        TargetScript.TargetGameObject = (GameObject)EditorGUILayout.ObjectField(TargetScript.TargetGameObject, typeof(GameObject), true);
        GUILayout.EndHorizontal();
        
		TargetScript.choosenLayerIndex = Gaze_EditorUtils.Gaze_HintPopup("Avaliable Layers", TargetScript.choosenLayerIndex, LayerNames.ToArray(), "Select a layer", 200);
		if (TargetScript.choosenLayerIndex != -1)
			TargetScript.NewLayer = LayerMask.NameToLayer(LayerNames[TargetScript.choosenLayerIndex]);
    }

}

