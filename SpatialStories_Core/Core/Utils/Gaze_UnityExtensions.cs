using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public static class Gaze_UnityExtensions
    {
        public static T GetComponentInChildrenBFS<T>(this GameObject c) where T : Component
        {
            return c.GetComponentsInChildrenBFS<T>()[0];
        }

        public static T[] GetComponentsInChildrenBFS<T>(this GameObject c) where T : Component
        {
            return c.GetComponentsInChildrenBFS<T>(false, true);
        }

        public static T[] GetComponentsInChildrenBFS<T>(this GameObject c, bool includeInactive) where T : Component
        {
            return c.GetComponentsInChildrenBFS<T>(includeInactive, true);
        }

        public static T[] GetComponentsInChildrenBFS<T>(this GameObject c, bool includeInactive, bool includeRoot) where T : Component
        {
            Queue<Transform> foundElements = new Queue<Transform>();
            List<T> resultingList = new List<T>();
            Transform current = c.transform;

            if (includeRoot)
            {
                resultingList.AddRange(current.GetComponents<T>());
            }
            do
            {
                foreach (Transform t in current)
                {
                    if (t.gameObject.activeSelf || includeInactive)
                    {
                        foundElements.Enqueue(t);
                    }
                }
                if (foundElements.Count == 0)
                {
                    break;
                }
                current = foundElements.Dequeue();
                resultingList.AddRange(current.GetComponents<T>());
            } while (foundElements.Count > 0);

            return resultingList.ToArray();
        }
    }
}
