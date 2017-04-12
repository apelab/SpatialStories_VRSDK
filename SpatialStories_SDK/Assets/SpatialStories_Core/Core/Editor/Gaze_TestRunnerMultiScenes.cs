using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityTest;
using System;
using System.Collections.Generic;

public class Gaze_TestRunnerMultiScenes : MonoBehaviour {
    [MenuItem("Test Multi-scenes/Run All Tests")]
    static void RunAllTests()
    {
        
        Debug.Log("Call TestRunner in every Test scene here !");

        //  Before start let's disable all the game objects of each scene
        Scene[] scenes = SceneManager.GetAllScenes();
        foreach(Scene scene in scenes)
        {
            DisableAllGameObjectsOfScene(scene);
        }

        foreach (Scene scene in scenes)
        {
            RunTestForSelectedScene(scene);
        }
    }


    public void Update()
    {
    }

    public static void DisableAllGameObjectsOfScene(Scene scene)
    {
        foreach(GameObject go in scene.GetRootGameObjects())
        {
            go.SetActiveRecursively(false);
        }
    }

    public static void RunTestForSelectedScene(Scene scene)
    {
        SceneManager.SetActiveScene(scene);

        foreach (GameObject go in scene.GetRootGameObjects())
        {
            go.SetActiveRecursively(true);
        }

        GameObject[] gameObjectsOfScene = scene.GetRootGameObjects();
        TestRunner testRunner = null;
        try
        {
            GameObject testRunnerGO = gameObjectsOfScene.FirstOrDefault(go => go.name.Equals("TestRunner"));
            testRunner = testRunnerGO.GetComponent<TestRunner>();
        }catch(Exception ex)
        {
            GameObject go = new GameObject();
            go.name = "TestRunner";
            testRunner = go.AddComponent<TestRunner>();
        }


        List<TestComponent> testOfScene = new List<TestComponent>();

        var objectsWithTests = gameObjectsOfScene.Where(go => go.GetComponent<TestComponent>() != null);
        foreach(GameObject go in objectsWithTests)
        {
            TestComponent component = go.GetComponent<TestComponent>();
            testOfScene.Add(go.GetComponent<TestComponent>());
            Debug.Log(component.gameObject.name);

        }

        
        IntegrationTestsRunnerWindow.instance.RunTests(testOfScene.Cast<ITestComponent>().ToList());
        IntegrationTestsRunnerWindow.instance.OnInspectorUpdate();
        //testRunner.currentTest.EnableTest(true);
        //testRunner.Start();
        //DisableAllGameObjectsOfScene(scene);
    }
}
