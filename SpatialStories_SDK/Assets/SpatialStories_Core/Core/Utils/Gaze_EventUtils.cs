﻿using System;
using UnityEngine;

namespace Gaze
{
    public static class Gaze_EventUtils
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }


        /// <summary>
        /// Checks if 2 generic objects are part of the same game object
        /// </summary>
        public static bool AreUnderSameGameObject(object _object1, object _object2)
        {
            return ConvertIntoGameObject(_object1).Equals(ConvertIntoGameObject(_object2));
        }

        /// <summary>
        /// Casts a generic object into an IO if this is a GameObject or a Monobehaivour.
        /// </summary>
        /// <param name="_obj">A Game Object or Monobehaivouir</param>
        /// <returns></returns>
        public static GameObject ConvertIntoGameObject(object _obj)
        {
            if (_obj is GameObject)
                return (GameObject)_obj;

            if (_obj is MonoBehaviour)
                return ((MonoBehaviour)_obj).gameObject;

            return null;
        }

        /// <summary>
        /// Checks if 2 objects are part of the same Gaze_Interactive object
        /// </summary>
        /// <returns></returns>
        public static bool AreUnderSameIO(object _obj1, object _obj2)
        {
            return GetIOFromObject(_obj1).Equals(GetIOFromObject(_obj2));
        }

        public static bool AreUnderSameIO(GameObject _obj1, object _obj2)
        {
            return GetIOFromGameObject(_obj1).Equals(GetIOFromObject(_obj2));
        }

        public static bool AreUnderSameIO(object _obj1, GameObject _obj2)
        {
            return GetIOFromObject(_obj1).Equals(GetIOFromGameObject(_obj2));
        }

        public static bool AreUnderSameIO(GameObject _obj1, GameObject _obj2)
        {
            return GetIOFromGameObject(_obj1).Equals(GetIOFromGameObject(_obj2));
        }

        public static Gaze_InteractiveObject GetIOFromObject(object _obj)
        {
            return GetIOFromGameObject((GameObject)_obj);
        }

        public static Gaze_InteractiveObject GetIOFromGameObject(GameObject _go)
        {
            return _go.GetComponentInParent<Gaze_InteractiveObject>();
        }
    }
}

