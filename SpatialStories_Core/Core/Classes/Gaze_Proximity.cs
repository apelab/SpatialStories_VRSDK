// <copyright file="Gaze_Proximity.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gaze
{
    [ExecuteInEditMode]
    public class Gaze_Proximity : MonoBehaviour
    {
        private static List<Gaze_InteractiveObject> hierarchyRigProximities;
        public static List<Gaze_InteractiveObject> HierarchyRigProximities
        {
            get
            {
                if (hierarchyRigProximities == null)
                    hierarchyRigProximities = new List<Gaze_InteractiveObject>();
                return hierarchyRigProximities;
            }
        }


        public bool debug = false;
        private bool proximityFlag = false;
        private GameObject otherGameObject;


        [HideInInspector]
        public Gaze_InteractiveObject IOScript;

        private void Awake()
        {
            // otherwise the event is fired too early
            IOScript = GetComponentInParent<Gaze_InteractiveObject>();
        }

        void Start()
        {
            StartCoroutine(NotifyAtStart());
        }

        private IEnumerator NotifyAtStart()
        {
            yield return new WaitForEndOfFrame();
            if (proximityFlag)
                Gaze_EventManager.FireProximityEvent(new Gaze_ProximityEventArgs(IOScript, otherGameObject.GetComponentInParent<Gaze_InteractiveObject>(), true));
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Gaze_Proximity>() != null)
            {
                if (debug)
                    Debug.Log("Gaze_Proximity (" + transform.parent.name + ") OnTriggerEnter with " + other.GetComponentInParent<Gaze_InteractiveObject>().name);

                //				proximityFlag = true;
                otherGameObject = other.gameObject;
                Gaze_EventManager.FireProximityEvent(new Gaze_ProximityEventArgs(IOScript, otherGameObject.GetComponentInParent<Gaze_InteractiveObject>(), true));
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Gaze_Proximity>() != null)
            {
                if (debug)
                    Debug.Log("Gaze_Proximity (" + transform.parent.name + ") OnTriggerExit with " + other.GetComponentInParent<Gaze_InteractiveObject>().name);
                proximityFlag = false;
                otherGameObject = other.gameObject;
                Gaze_EventManager.FireProximityEvent(new Gaze_ProximityEventArgs(IOScript, otherGameObject.GetComponentInParent<Gaze_InteractiveObject>(), false));
            }
        }
    }
}