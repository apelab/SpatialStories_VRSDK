using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Class to easily model random array. Each element gets a defined probability to be picked.
    /// This probability is the normalization of the density of all the elements in the array
    /// </summary>
    /// <typeparam name="T">The type of object the list will contain</typeparam>
    /// 
    [System.Serializable]
    public class Gaze_RandomList<T>
    {
        [SerializeField]
        private List<T> List;
        [SerializeField]
        private Gaze_dpdf dpdf;

        private bool randomize;
        private int lastIndex;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_randomize">true if the array should be randomized</param>
        public Gaze_RandomList(bool _randomize)
        {
            List = new List<T>();
            randomize = _randomize;
            lastIndex = -1;

            if (randomize)
                dpdf = new Gaze_dpdf();
        }

        /// <summary>
        /// Add an element to the array with a defined density
        /// </summary>
        /// <param name="e">the element to add</param>
        /// <param name="pdf">the corresponding density. Default 1.0f for uniform distribution</param>
        public void Add(T e, float density = 1.0f)
        {
            List.Add(e);
            if (randomize)
                dpdf.Add(density);
        }

        /// <summary>
        /// Adding a range of element with corresponding densities
        /// </summary>
        /// <param name="range">the range of elements to add</param>
        /// <param name="densities">the corresponding densities</param>
        public void AddRange(List<T> range, List<float> densities = null)
        {
            for (int i = 0; i < range.Count; i++)
            {
                if (densities != null && densities.Count > i)
                {
                    Add(range[i], densities[i]);
                }
                else
                {
                    Add(range[i]);
                }
            }
        }

        /// <summary>
        /// Get an element of the array. Either a random one, either the next in the sequence
        /// </summary>
        /// <returns>An element of the array</returns>
        public T GetNext()
        {
            if (randomize)
            {
                return List[dpdf.Sample()];
            }
            else
            {
                lastIndex = (lastIndex + 1) % List.Count;
                return List[lastIndex];
            }
        }

        /// <summary>
        /// Get an element of the array by its index. This won't change the one going sequence
        /// </summary>
        /// <param name="index">the index of the wanted element</param>
        /// <returns></returns>
        public T Get(int index)
        {
            if (index < List.Count)
            {
                return List[index];
            }
            else
            {
                Debug.LogError("Index out Order; return first element of the list instead");
                return List[0];
            }
        }

        /// <summary>
        /// Normalizing the probability distribution
        /// </summary>
        public void Finalize()
        {
            if (randomize)
                dpdf.Normalize();
        }
    }
}
