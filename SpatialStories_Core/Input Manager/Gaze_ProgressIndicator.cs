using UnityEngine;
using UnityEngine.UI;

namespace SpatialStories
{
    public class Gaze_ProgressIndicator : MonoBehaviour
    {
        public Image ProgressImage;
        
        public void Show()
        {
            ProgressImage.enabled = true;
        }

        public void Hide()
        {
            ProgressImage.enabled = false;
        }

        public void UpdateProgress(float _progress)
        {
            ProgressImage.fillAmount = _progress;
        }
    }
}