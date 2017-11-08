using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System;
using System.Linq;

namespace Gaze
{

    public class ListAllConditionsMapsWindow : EditorWindow
    {
        private static List<Gaze_Interaction> interactionsToValidate;

        [MenuItem("SpatialStories/Utils/Scene Inspector")]
        public static void ShowWindow()
        {
            if (interactionsToValidate == null)
                interactionsToValidate = new List<Gaze_Interaction>();

            GetWindow(typeof(ListAllConditionsMapsWindow));
        }

        int tab;
        void OnGUI()
        {
            Gaze_EditorUtils.DrawSectionTitle("SpatialStories SDK Scene Inspector: ");
            tab = GUILayout.Toolbar(tab, new string[] { "Dependencies Activate", "Proximity", "Dependencies Deactivate",  "Custom Conditions", "Interactive Objects", "GameObjects", "Meshes & Materials"});
            GUILayout.BeginHorizontal();
            
            GUILayout.BeginVertical();
            switch (tab)
            {
                case 0:
                    ShowDepententObjectsInspector(true);
                    break;
                case 1:
                    ShowProximityObjects();
                    break;
                case 2:
                    ShowDepententObjectsInspector(false);
                    break;
                case 3:
                    ShowCustomConditionsObjects();
                    break;
                case 4:
                    ShowInteractiveObjects();
                    break;
                case 5:
                    ShowGameObjects();
                    break;
                case 6:
                    ShowAllRenderers();
                    break;
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public Dictionary<Gaze_Conditions, List<GameObject>> DependentObjects = new Dictionary<Gaze_Conditions, List<GameObject>>();
        Vector2 scrollPos = new Vector2();
        bool ShowWithDependenciesDisabled = false;
        private void ShowDepententObjectsInspector(bool _activate)
        {
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Show All Dependency Maps"))
            {
                DependentObjects.Clear();
                Gaze_Conditions[] AllConditions = FindObjectsOfType<Gaze_Conditions>();
                foreach (var condition in AllConditions)
                {
                    
                    DependentObjects.Add(condition, new List<GameObject>());
                    
                    foreach (Gaze_Dependency dependency in (_activate ? condition.ActivateOnDependencyMap.dependencies : condition.DeactivateOnDependencyMap.dependencies))
                    {
                        DependentObjects[condition].Add(dependency.dependentGameObject);
                    }

                    if(DependentObjects[condition].Count > 0)
                    {
                        if (ShowWithDependenciesDisabled && condition.dependent)
                            DependentObjects.Remove(condition);
                        else if (!ShowWithDependenciesDisabled && !condition.dependent)
                            DependentObjects.Remove(condition);
                    }
                    else
                        DependentObjects.Remove(condition);
                }
            }

            ShowWithDependenciesDisabled = GUILayout.Toggle(ShowWithDependenciesDisabled, "Only With Dependecy Disabled");
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            if (DependentObjects != null)
            {
                foreach (Gaze_Conditions conditions in DependentObjects.Keys)
                {
                    var a = EditorGUILayout.ObjectField(conditions, typeof(Gaze_Interaction), true) as Gaze_Interaction;
                    EditorGUI.indentLevel++;
                    foreach (GameObject dependency in DependentObjects[conditions])
                    {
                        var b = EditorGUILayout.ObjectField(dependency, typeof(GameObject), true) as GameObject;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.EndScrollView();
        }


        public Dictionary<Gaze_Conditions, List<GameObject>> ProximityObjects = new Dictionary<Gaze_Conditions, List<GameObject>>();

        Vector2 scrollPos2 = new Vector2();

        bool ShowOnlyWithProximityConditionsDisabled = false;
        bool ShowOnlyIncorrectProximities;
        private void ShowProximityObjects()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show All Proximity Maps"))
            {
                ProximityObjects.Clear();
                Gaze_Conditions[] AllConditions = FindObjectsOfType<Gaze_Conditions>();
                foreach (var condition in AllConditions)
                {
  
                    ProximityObjects.Add(condition, new List<GameObject>());
                    bool incorrect = false;
                    foreach (Gaze_ProximityEntry proximityEntry in condition.proximityMap.proximityEntryList)
                    {
                        try
                        {
                            ProximityObjects[condition].Add(proximityEntry.dependentGameObject.gameObject);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(condition.name + " Contains a destroyed Object!");
                            ProximityObjects[condition].Add(null);
                            incorrect = true;
                        }
                    }

                    foreach (Gaze_ProximityEntryGroup proximityEntryGroup in condition.proximityMap.proximityEntryGroupList)
                    {
                        foreach(Gaze_ProximityEntry entry in proximityEntryGroup.proximityEntries)
                        {
                            try
                            {
                                ProximityObjects[condition].Add(entry.dependentGameObject.gameObject);
                            }
                            catch(Exception ex)
                            {
                                Debug.Log(condition.name + " Contains a destroyed Object!");
                                ProximityObjects[condition].Add(null);
                                incorrect = true;
                            }
                        }
                    }

                    if (!ShowOnlyIncorrectProximities)
                    {
                        if (ProximityObjects[condition].Count > 0)
                        {
                            if (ShowOnlyWithProximityConditionsDisabled && condition.proximityEnabled)
                                ProximityObjects.Remove(condition);
                            else if (!ShowOnlyWithProximityConditionsDisabled && !condition.proximityEnabled)
                                ProximityObjects.Remove(condition);
                        }
                        else
                            ProximityObjects.Remove(condition);

                    }
                    else
                    {
                        if (ProximityObjects[condition].Count != 1 && !incorrect)
                            ProximityObjects.Remove(condition);
                    }
                }
            }

            ShowOnlyIncorrectProximities = GUILayout.Toggle(ShowOnlyIncorrectProximities, "Show Incorrect Proximities");
            if(!ShowOnlyIncorrectProximities)
                ShowOnlyWithProximityConditionsDisabled = GUILayout.Toggle(ShowOnlyWithProximityConditionsDisabled, "Only With Proximity Disabled");

            GUILayout.EndHorizontal();

            scrollPos2 = GUILayout.BeginScrollView(scrollPos2);
            if (DependentObjects != null)
            {
                foreach (Gaze_Conditions conditions in ProximityObjects.Keys)
                {
                    var a = EditorGUILayout.ObjectField(conditions, typeof(Gaze_Interaction), true) as Gaze_Interaction;
                    EditorGUI.indentLevel++;
                    foreach (GameObject proximityEntry in ProximityObjects[conditions])
                    {
                        var b = EditorGUILayout.ObjectField(proximityEntry, typeof(GameObject), true) as GameObject;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.EndScrollView();
        }


        public Dictionary<Gaze_Conditions, List<Gaze_AbstractConditions>> CustomConditions = new Dictionary<Gaze_Conditions, List<Gaze_AbstractConditions>>();
        Vector2 scrollPos3 = new Vector2();
        bool ShowOnlyNotUsed = false;
        int numCustomConditions = 0;
        private void ShowCustomConditionsObjects()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show All Custom Conditions Maps"))
            {
                numCustomConditions = 0;
                CustomConditions.Clear();
                Gaze_Conditions[] AllConditions = FindObjectsOfType<Gaze_Conditions>();
                foreach (var condition in AllConditions)
                {

                    CustomConditions.Add(condition, new List<Gaze_AbstractConditions>());

                    foreach (Gaze_AbstractConditions customCondition in condition.GetComponents<Gaze_AbstractConditions>())
                    {
                        numCustomConditions++;
                        CustomConditions[condition].Add(customCondition);

                        if (!ShowOnlyNotUsed)
                        {
                            if (!condition.customConditionsEnabled)
                            {
                                CustomConditions[condition].Remove(customCondition);
                                numCustomConditions--;
                            }
                        }
                        else
                        {
                            if (condition.customConditionsEnabled)
                            {
                                CustomConditions[condition].Remove(customCondition);
                                numCustomConditions--;
                            }
                        }
                    }

                    if (CustomConditions[condition].Count == 0)
                        CustomConditions.Remove(condition);
                }
            }

            ShowOnlyNotUsed = GUILayout.Toggle(ShowOnlyNotUsed, "Show Only Not used");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Num Custom Conditions: " + numCustomConditions);
            GUILayout.EndHorizontal();

            scrollPos3 = GUILayout.BeginScrollView(scrollPos3);
            if (CustomConditions != null)
            {
                foreach (Gaze_Conditions conditions in CustomConditions.Keys)
                {
                    var a = EditorGUILayout.ObjectField(conditions, typeof(Gaze_Interaction), true) as Gaze_Interaction;
                    EditorGUI.indentLevel++;
                    foreach (Gaze_AbstractConditions conditionEntry in CustomConditions[conditions])
                    {
                        var b = EditorGUILayout.ObjectField(conditionEntry, typeof(Gaze_AbstractConditions), true) as Gaze_AbstractConditions;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.EndScrollView();
        }


        public Dictionary<Gaze_InteractiveObject, List<Gaze_Interaction>> InteractiveObjects = new Dictionary<Gaze_InteractiveObject, List<Gaze_Interaction>>();
        Vector2 scrollPos4 = new Vector2();
        int numInteractiveObjects = 0;
        int numInteractions = 0;
        private void ShowInteractiveObjects()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show All Interactive Objects"))
            {
                numInteractiveObjects = 0;
                numInteractions = 0;
                InteractiveObjects.Clear();
                Gaze_InteractiveObject[] AllInteractiveObjects = FindObjectsOfType<Gaze_InteractiveObject>();
                foreach (var interactiveObject in AllInteractiveObjects)
                {

                    InteractiveObjects.Add(interactiveObject, new List<Gaze_Interaction>());
                    numInteractiveObjects++;
                    foreach (Gaze_Interaction interaction in interactiveObject.GetComponentsInChildren<Gaze_Interaction>())
                    {
                        InteractiveObjects[interactiveObject].Add(interaction);
                        numInteractions++;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Num Interactive Objects: " + numInteractiveObjects);
            GUILayout.Label("Num Interactions: " + numInteractions);
            GUILayout.EndHorizontal();

            scrollPos4 = GUILayout.BeginScrollView(scrollPos4);
            if (InteractiveObjects != null)
            {
                foreach (Gaze_InteractiveObject interactiveObject in InteractiveObjects.Keys)
                {
                    var a = EditorGUILayout.ObjectField(interactiveObject, typeof(Gaze_InteractiveObject), true) as Gaze_InteractiveObject;
                    EditorGUI.indentLevel++;
                    foreach (Gaze_Interaction interactionEntry in InteractiveObjects[interactiveObject])
                    {
                        var b = EditorGUILayout.ObjectField(interactionEntry, typeof(Gaze_Interaction), true) as Gaze_Interaction;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.EndScrollView();
        }

#region GameObjects
        public List<GameObject> GameObjects = new List<GameObject>();
        Vector2 scrollPos5 = new Vector2();
        string searchCriterum = "";
        int foundGameObjects = 0;
        private void ShowGameObjects()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show All Game Objects"))
            {
                GameObjects = new List<GameObject>();
                GameObject[] AllGameObjects = FindObjectsOfType<GameObject>();
                foreach (var go in AllGameObjects)
                    GameObjects.Add(go);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            searchCriterum = GUILayout.TextField(searchCriterum);
            GUILayout.Label("Num Game Objects: " + foundGameObjects);
            GUILayout.EndHorizontal();
            scrollPos5 = GUILayout.BeginScrollView(scrollPos5);
            foundGameObjects = 0;
            if (GameObjects != null)
            {
                foreach (GameObject go in GameObjects)
                {
                    if(searchCriterum.Trim().Length == 0)
                    {
                        var a = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;
                        foundGameObjects++;
                    }
                    else
                    {
                        if (go.name.ToLower().Contains(searchCriterum.ToLower()))
                        {
                            var b = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;
                            foundGameObjects++;
                        }
                            
                    }

                }
            }
            GUILayout.EndScrollView();
        }
#endregion GameObjects
    
        public class RenderInfo
        {
            public Gaze_InteractiveObject IO;
            public List<Material> Materials = new List<Material>();
            public Renderer[] Renderers;

            public List<MeshFilter> MeshFilters = new List<MeshFilter>();

            public int TotalNumberOfTriangles;
            public int TotalNumberOfMaterials;

            public RenderInfo(Gaze_InteractiveObject _io)
            {
                IO = _io;
                Renderers = IO.gameObject.GetComponentsInChildren<Renderer>();
                foreach(Renderer rend in Renderers)
                {
                    foreach(Material mat in rend.materials)
                    {
                        if(!Materials.Contains(mat))
                            Materials.Add(mat);
                    }
                }
                TotalNumberOfMaterials = Materials.Count;
                
                MeshFilters = IO.gameObject.GetComponentsInChildren<MeshFilter>().OrderBy(o=>o.mesh.triangles.Length).ToList();
                MeshFilters.Reverse();

                TotalNumberOfTriangles = 0;
                foreach(MeshFilter filter in MeshFilters)
                {
                    TotalNumberOfTriangles += filter.mesh.triangles.Length;
                }
                TotalNumberOfTrianglesScene += TotalNumberOfTriangles;
                TotalNumberOfMaterialsScene += TotalNumberOfMaterials;
            }
        }

        static int TotalNumberOfMaterialsScene;
        
        static int TotalNumberOfTrianglesScene;
        
        List<RenderInfo> renderInfos;
        Vector2 scrollPos6 = new Vector2();
        private void ShowAllRenderers()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show All Renderers"))
            {
                TotalNumberOfMaterialsScene = 0;
                TotalNumberOfTrianglesScene = 0;
                renderInfos = new List<RenderInfo>();
                Gaze_InteractiveObject[] allIos = FindObjectsOfType<Gaze_InteractiveObject>();
                foreach(Gaze_InteractiveObject io in allIos)
                {
                    renderInfos.Add(new RenderInfo(io));
                }
                renderInfos.Sort(delegate(RenderInfo a, RenderInfo b){
                   int matDiff = a.TotalNumberOfMaterials.CompareTo(b.TotalNumberOfMaterials);
                   if(matDiff != 0) return matDiff;
                   else return a.TotalNumberOfTriangles.CompareTo(b.TotalNumberOfTriangles);
                });
                renderInfos.Reverse();
                
            }

            if(renderInfos != null && renderInfos.Count > 0)
            {
                EditorGUILayout.LabelField("Total Num Materials: ", TotalNumberOfMaterialsScene.ToString());
                EditorGUILayout.LabelField("Total Num NumTriangles", TotalNumberOfTrianglesScene.ToString());
            }

            GUILayout.EndHorizontal();
            scrollPos6 = GUILayout.BeginScrollView(scrollPos6);
            foreach (RenderInfo renderInfo in renderInfos)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Num Materials: ", renderInfo.TotalNumberOfMaterials.ToString());
                EditorGUILayout.LabelField("Num NumTriangles", renderInfo.TotalNumberOfTriangles.ToString());
                var a = EditorGUILayout.ObjectField(renderInfo.IO, typeof(Gaze_InteractiveObject), true) as Gaze_InteractiveObject;
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                foreach (MeshFilter filter in renderInfo.MeshFilters)
                {
                    EditorGUILayout.BeginHorizontal();
                    var b = EditorGUILayout.ObjectField(filter, typeof(MeshFilter), true) as MeshFilter;
                    EditorGUILayout.LabelField("Num Triangles", (filter.mesh.triangles.Length).ToString());
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.EndScrollView();
        }
    }
}

