using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gaze
{
    [Serializable]
    public class Gaze_SceneLoader : Gaze_AbstractBehaviour
    {
        public string targetSceneName;
        public string loadingScreen = "LoadingScreen";
        public int triggerStateIndex;
        public float loadDelay;
        public bool displayLoadingScreen;
        private AsyncOperation loadOperation;
        private Gaze_LevelManager levelManager;

        void Start()
        {
            levelManager = GameObject.FindObjectOfType<Gaze_LevelManager>();
            levelManager.setNextLevelName(targetSceneName);
        }

        private void LoadSceneAsync(int i)
        {
            if (triggerStateIndex == i)
            {
                if (loadDelay > 0f)
                {
                    if (displayLoadingScreen)
                    {
                        loadOperation = SceneManager.LoadSceneAsync(levelManager.getNextLevelName());
                    }
                    else
                    {
                        loadOperation = SceneManager.LoadSceneAsync(targetSceneName);
                    }
                    StartCoroutine(LoadDelayedAsync());
                }
                else
                {
                    if (displayLoadingScreen)
                    {
                        SceneManager.LoadScene(levelManager.getNextLevelName());
                    }
                    else
                    {
                        SceneManager.LoadScene(targetSceneName);
                    }
                }
            }
        }

        private IEnumerator LoadDelayedAsync()
        {
            loadOperation.allowSceneActivation = false;
            yield return new WaitForSeconds(loadDelay);
            loadOperation.allowSceneActivation = true;
        }

        #region implemented abstract members of Gaze_AbstractBehaviour
        protected override void onTrigger()
        {
            LoadSceneAsync(0);
        }

        protected override void onReload()
        {
            LoadSceneAsync(1);
        }

        protected override void onBefore()
        {
            LoadSceneAsync(2);
        }

        protected override void onActive()
        {
            LoadSceneAsync(3);
        }

        protected override void onAfter()
        {
            LoadSceneAsync(4);
        }
        #endregion
    }
}
