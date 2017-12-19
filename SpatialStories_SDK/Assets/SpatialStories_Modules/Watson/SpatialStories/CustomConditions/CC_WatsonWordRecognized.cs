using UnityEngine;
namespace Gaze
{
    public class CC_WatsonWordRecognized : Gaze_AbstractConditions
    {
        public string WordToRecognize;
        
        private void Start()
        {
            FindObjectOfType<VoiceSpawner>().AddWord(WordToRecognize, Validate);
        }

        public void Validate()
        {
            ValidateCustomCondition(true);
        }
    }
}
