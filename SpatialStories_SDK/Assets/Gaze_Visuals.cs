using System.Collections.Generic;
using UnityEngine;


namespace Gaze
{
    [ExecuteInEditMode]
    public class Gaze_Visuals : MonoBehaviour
    {
        [HideInInspector]
        public List<int> selectedRenderers = new List<int>();
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
            Debug.Log("Updating");
            allRenderers.Clear();
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
                allRenderers.Add(renderer);


            for (int i = 0; i < selectedRenderers.Count; i++)
            {
                if (selectedRenderers[i] >= allRenderers.Count)
                {
                    selectedRenderers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Adds the first index of allRenderers to selectedRenderers.
        /// </summary>
        public void Add()
        {
            if (allRenderers.Count > 0 && selectedRenderers.Count < allRenderers.Count)
                selectedRenderers.Add(0);
            else
                return;
        }

        /// <summary>
        /// Removes from selectedRenderers the int corresponding to the index of allRenderers.
        /// </summary>
        public void Remove(int r)
        {
            if (selectedRenderers.Contains(r))
                selectedRenderers.Remove(r);
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

        internal void AlterSelectedVisuals(bool enable)
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

