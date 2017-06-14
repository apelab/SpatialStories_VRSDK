using System.Collections.Generic;
using UnityEngine;


namespace Gaze
{
    [ExecuteInEditMode]
    public class Gaze_InteractiveObjectVisuals : MonoBehaviour
    {
        private List<Renderer> allRenderers = new List<Renderer>();
        public List<Renderer> GetAllRenderers()
        {
            UpdateAllRenderers();
            return allRenderers;
        }

        void OnEnable()
        {
            UpdateAllRenderers();
        }

        void OnTransformChildrenChanged()
        {
            UpdateAllRenderers();
        }

        /// <summary>
        /// Updates the list of all renderers attached to this gameObject or his children.
        /// </summary>
        public void UpdateAllRenderers()
        {
            allRenderers.Clear();
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
                allRenderers.Add(renderer);
        }


        internal void AlterAllVisuals(bool enable)
        {
            ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>();
            foreach (Renderer renderer in allRenderers)
                renderer.enabled = enable;

            foreach (ParticleSystem particle in allParticles)
            {
                if (enable)
                    particle.Play();
                else
                    particle.Stop();
            }
        }

        internal void AlterSelectedVisuals(bool enable, List<int> selectedRenderers)
        {
            foreach (int r in selectedRenderers)
            {
                allRenderers[r].enabled = enable;
                ParticleSystem ps = allRenderers[r].gameObject.GetComponent<ParticleSystem>();
                if (ps)
                {
                    if (enable)
                        ps.Play();
                    else
                        ps.Stop();
                }

            }
        }
    }

}