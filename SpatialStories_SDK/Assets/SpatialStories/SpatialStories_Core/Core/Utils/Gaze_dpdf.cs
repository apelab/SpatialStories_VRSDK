using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{

    /// <summary>
    /// This class can be used to transform uniformly distributed samples to a stored discrete probability distribution
    /// </summary>
    /// 
    [SerializeField]
    public class Gaze_dpdf
    {
        [SerializeField]
        private List<float> cdf;
        private float sum, normalization;
        private bool normalized;

        public Gaze_dpdf()
        {
            cdf = new List<float>();
        }

        /// <summary>
        /// Add an entry with the specified discrete probability
        /// </summary>
        /// <param name="_pdfValue">discrete probability to add</param>
        public void Add(float _pdfValue)
        {
            if (cdf.Count == 0) cdf.Add(_pdfValue);
            else cdf.Add(cdf[cdf.Count - 1] + _pdfValue);
        }

        /// <summary>
        /// Access an entry by its index
        /// </summary>
        /// <param name="_index">index of the entry</param>
        /// <returns>The entry at index</returns>
        public float Get(int _index)
        {
            if (_index < cdf.Count)
                return cdf[_index];
            else
                throw new System.ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Assumes that normalize() has previously been called
        /// </summary>
        /// <returns>The original (unormalized) sum of all PDF entries</returns>
        public float GetSum()
        {
            return sum;
        }

        /// <summary>
        /// Normalize the distribution
        /// </summary>
        public void Normalize()
        {
            if (cdf.Count > 0)
            {
                sum = cdf[cdf.Count - 1];
                if (sum > 0)
                {
                    normalization = 1.0f / sum;
                    for (int i = 0; i < cdf.Count; i++)
                    {
                        cdf[i] *= normalization;
                    }
                    cdf[cdf.Count - 1] = 1.0f;
                    normalized = true;
                }
                else
                {
                    normalization = 0f;
                }

                int k = 0;
                foreach (var a in cdf)
                {
                    k++;
                }
            }
        }

        /// <summary>
        /// Transform an uniformly distributed sample to the stored distribution
        /// </summary>
        /// <param name="sampleValue"> An uniformly distributed sampl on [0,1]</param>
        /// <returns></returns>
        public int Sample(float sampleValue)
        {
            if (!normalized)
            {
                Debug.LogWarning("The Probability Distribution has not been normalized yet: doing it now");
                Normalize();
            }

            int index = cdf.FindIndex(x => x >= sampleValue);
            return Mathf.Min(index, cdf.Count - 1);
        }

        public int Sample()
        {
            return Sample(Random.value);
        }
    }
}