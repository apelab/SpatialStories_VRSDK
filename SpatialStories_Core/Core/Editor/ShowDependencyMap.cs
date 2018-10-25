using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Gaze
{
    public class ShowDependencyMapWindow : EditorWindow
    {
        public class DependencyNode
        {
            public bool IsDepdendent { get { return condition.dependent; } }
            public Gaze_Conditions condition;
            public List<DependencyNode> dependencies;
            public Rect nodeRectangle;
            public List<DependencyNode> dependencyNodes;
            public int depth = 0;
            public Color lineColor;
            bool someIsDependentOfMe = false;
            public static int assignedIds = 0;
            public int id;

            public DependencyNode(Gaze_Conditions _condition, Rect _nodeRect, List<DependencyNode> _dependencyNodes)
            {
                condition = _condition;
                nodeRectangle = _nodeRect;
                dependencyNodes = _dependencyNodes;
                lineColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
                id = assignedIds++;
            }

            public void GetConnections()
            {
                dependencies = new List<DependencyNode>();

                if (!condition.dependent)
                    return;

                foreach (Gaze_Dependency dep in condition.ActivateOnDependencyMap.dependencies)
                {
                    DependencyNode node = dependencyNodes.FirstOrDefault<DependencyNode>(n => n.condition.gameObject == dep.dependentGameObject);
                    if (node != null)
                    {
                        dependencies.Add(node);
                        node.someIsDependentOfMe = true;
                    }
                }

            }

            public void GetDependencyDepth()
            {
                // TODO: Fill this method to find how many dependencies needs to be validated before this one will
                // Like that it will be easy to paint into the graph in columns
                Queue<DependencyNode> dependenciesToVisit = new Queue<DependencyNode>(dependencies);
                List<DependencyNode> alreadyVisitedDependencies = new List<DependencyNode>(dependencies);

                if (dependencies == null || dependencies.Count == 0)
                {
                    depth = someIsDependentOfMe ? 1 : 0;
                    nodeRectangle.x = nodeRectangle.width * depth;
                    return;
                }
                depth = 2;
                nodeRectangle.x = (nodeRectangle.width + 100) * depth;

                List<DependencyNode> newDependenciesToVisit = new List<DependencyNode>();
                while (dependenciesToVisit.Count > 0)
                {
                    DependencyNode node = dependenciesToVisit.Dequeue();
                    if (node.IsDepdendent)
                    {
                        foreach (DependencyNode n in node.dependencies)
                        {
                            if (!alreadyVisitedDependencies.Contains(n) && !newDependenciesToVisit.Contains(n))
                                newDependenciesToVisit.Add(n);
                        }
                    }
                    else
                        return;

                    alreadyVisitedDependencies.Add(node);

                    if (dependenciesToVisit.Count == 0 && newDependenciesToVisit.Count > 0)
                    {
                        depth++;
                        nodeRectangle.x = (nodeRectangle.width + 100) * depth;
                        bool cont = false;
                        foreach (DependencyNode depNode in newDependenciesToVisit)
                        {
                            if (!alreadyVisitedDependencies.Contains(depNode))
                                cont = true;
                        }
                        if (cont)
                            dependenciesToVisit = new Queue<DependencyNode>(newDependenciesToVisit);
                        else
                            return;
                    }
                }
            }
        }
        Dictionary<int, int> positions;
        Gaze_Conditions[] allSceneCondition;
        List<DependencyNode> dependencyNodes;
        string searchTerm = "";
        Vector2 scrollPosition;


        [MenuItem("SpatialStories/Utils/Dependency Map")]
        static void ShowEditor()
        {
            ShowDependencyMapWindow editor = EditorWindow.GetWindow<ShowDependencyMapWindow>();
            editor.Init();
        }


        public void Init()
        {
            Debug.ClearDeveloperConsole();
            scrollPosition = new Vector2(0, 0);
            positions = new Dictionary<int, int>();
            allSceneCondition = UnityEngine.Object.FindObjectsOfType<Gaze_Conditions>();
            dependencyNodes = new List<DependencyNode>();

            int i = 0;
            foreach (Gaze_Conditions condition in allSceneCondition)
            {
                dependencyNodes.Add(new DependencyNode(condition, new Rect(0, 0, 180, 40), dependencyNodes));
                i++;
            }

            foreach (DependencyNode node in dependencyNodes)
                node.GetConnections();

            foreach (DependencyNode node in dependencyNodes)
                node.GetDependencyDepth();

            foreach (DependencyNode node in dependencyNodes)
            {
                if (positions.ContainsKey(node.depth))
                {
                    positions[node.depth]++;
                }
                else
                {
                    positions[node.depth] = 0;
                }
                node.nodeRectangle.y = positions[node.depth] * 60;
            }
        }



        void OnGUI()
        {
            try
            {
                GUI.Label(new Rect(0, 3, 300, 20), "Search Term: ");
                searchTerm = GUI.TextField(new Rect(100, 0, 300, 20), searchTerm);
                if (GUI.Button(new Rect(402, 0, 60, 20), "Clear"))
                {
                    searchTerm = "";
                }


                scrollPosition = GUI.BeginScrollView(new Rect(0, 25, position.width, position.height - 24), scrollPosition, new Rect(0, 0, (positions.Keys.Max() + 1) * 300f, (positions.Values.Max() + 3) * 70f));



                foreach (DependencyNode node in dependencyNodes)
                {
                    if (node.dependencies != null)
                    {
                        foreach (DependencyNode dep in node.dependencies)
                        {
                            if (searchTerm.Trim() == "")
                                DrawNodeCurve(dep, node);
                        }
                    }
                }

                BeginWindows();

                int i = 0;
                foreach (DependencyNode node in dependencyNodes)
                {
                    Color col = GUI.color;
                    if (Application.isPlaying)
                    {
                        GUI.color = node.condition.TriggerCount > node.condition.reloadCount ? Color.green : Color.red;
                    }


                    if (node.condition.name.ToLower().Contains(searchTerm.Trim().ToLower()) || searchTerm.Trim().Length == 0)
                        node.nodeRectangle = GUI.Window(node.id, node.nodeRectangle, DrawNodeWindow, node.condition.gameObject.name);
                }

                EndWindows();
                GUI.EndScrollView();
            }
            catch (Exception ex)
            {
                Init();
                return;
            }
        }

        void DrawNodeWindow(int id)
        {
            if (Application.isPlaying)
            {
                if (GUI.Button(new Rect(35, 16, 100, 20), "Validate"))
                {
                    dependencyNodes.FirstOrDefault(node => node.id == id).condition.Validate();
                }
            }

            GUI.DragWindow();
        }

        void DrawNodeCurve(DependencyNode start, DependencyNode end)
        {
            Vector3 startPos = new Vector3(start.nodeRectangle.x + start.nodeRectangle.width, start.nodeRectangle.y + start.nodeRectangle.height / 2, 0);
            Vector3 endPos = new Vector3(end.nodeRectangle.x, end.nodeRectangle.y + end.nodeRectangle.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++) // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);


            Handles.DrawBezier(startPos, endPos, startTan, endTan, start.lineColor, null, 1);
        }
    }

}
