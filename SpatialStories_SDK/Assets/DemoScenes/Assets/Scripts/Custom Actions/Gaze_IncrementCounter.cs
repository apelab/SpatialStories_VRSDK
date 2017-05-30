namespace Gaze
{
    public class Gaze_IncrementCounter : Gaze_AbstractBehaviour
    {

        public UnityEngine.UI.Text[] txt;
        private static int count = 0;

        protected override void OnTrigger()
        {
            count++;
            foreach (UnityEngine.UI.Text t in txt)
            {
                t.text = count.ToString();
            }
        }

        protected override void OnActive() { }

        protected override void OnAfter() { }

        protected override void OnBefore() { }

        protected override void OnReload() { }



    }
}

