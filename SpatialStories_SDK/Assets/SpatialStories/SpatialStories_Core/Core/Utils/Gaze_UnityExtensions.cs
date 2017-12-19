using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gaze
{
    public static class Gaze_UnityExtensions
    {
        static Queue<Transform> c_foundElements;

        public static T GetComponentInChildrenBFS<T>(this GameObject c) where T : Component
        {
            if (c_foundElements == null)
                c_foundElements = new Queue<Transform>();
            else
                c_foundElements.Clear();

            List<T> resultingList = new List<T>();
            Transform current = c.transform;

            resultingList.AddRange(current.GetComponents<T>());
            
            do
            {
                Transform t = null;
                for (int i = 0; i < current.childCount;  i++)
                {
                    t = current.GetChild(i);
                    if (t.gameObject.activeSelf)
                    {
                        c_foundElements.Enqueue(t);
                    }
                }
                if (c_foundElements.Count == 0)
                {
                    break;
                }
                current = c_foundElements.Dequeue();
                T[] components = current.GetComponents<T>();
                if (components.Length > 0)
                    return components[0];
            } while (c_foundElements.Count > 0);

            return null;
        }

        public static List<T> GetComponentsInChildrenBFS<T>(this GameObject c) where T : Component
        {
            return c.GetComponentsInChildrenBFS<T>(false, true);
        }

        public static List<T> GetComponentsInChildrenBFS<T>(this GameObject c, bool includeInactive) where T : Component
        {
            return c.GetComponentsInChildrenBFS<T>(includeInactive, true);
        }


        public static List<T> GetComponentsInChildrenBFS<T>(this GameObject c, bool includeInactive, bool includeRoot) where T : Component
        {
            if (c_foundElements == null)
                c_foundElements = new Queue<Transform>();
            else
                c_foundElements.Clear();
            
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
                        c_foundElements.Enqueue(t);
                    }
                }
                if (c_foundElements.Count == 0)
                {
                    break;
                }
                current = c_foundElements.Dequeue();
                resultingList.AddRange(current.GetComponents<T>());
            } while (c_foundElements.Count > 0);

            return resultingList;
        }
    }

    public delegate double EaseDelegate(double t, double b, double c, double d);

    public abstract class Ease
    {
        protected const double TWO_PI = Math.PI * 2;
        protected const double HALF_PI = Math.PI / 2;
    }
    
    public class Cubic : Ease
    {
        public static double EaseIn(double t, double b, double c, double d)
        {
            return c * (t /= d) * t * t + b;
        }
        public static double EaseOut(double t, double b, double c, double d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }
        public static double EaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }
    }

}
