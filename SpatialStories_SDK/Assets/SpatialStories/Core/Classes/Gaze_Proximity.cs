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
        public static List<Gaze_InteractiveObject> HierarchyRigProximities
        {
            get
            {
                if (hierarchyRigProximities == null)
                    hierarchyRigProximities = new List<Gaze_InteractiveObject>();
                UpdateRigProximitiesList();
                return hierarchyRigProximities;
            }
        }
        public bool debug = false;

        private static List<Gaze_InteractiveObject> hierarchyRigProximities;

        private static void UpdateRigProximitiesList()
        {
            hierarchyRigProximities.Clear();
            Gaze_InputManager rigRoot = (Gaze_InputManager)FindObjectOfType(typeof(Gaze_InputManager));
            if (rigRoot)
            {
                Gaze_Proximity[] rigProximities = rigRoot.GetComponentsInChildren<Gaze_Proximity>();
                for (int i = 0; i < rigProximities.Length; i++)
                {
                    Gaze_InteractiveObject io = rigProximities[i].GetComponentInParent<Gaze_InteractiveObject>();
                    if (io && io.transform.parent == rigRoot.transform)
                    {
                        hierarchyRigProximities.Add(io);
                    }
                }
            }
        }
        private bool proximityFlag = false;
        private GameObject otherGameObject;
        private Gaze_ProximityEventArgs proximityEventArgs;
        private int proximityLayerMask;

        [HideInInspector]
        public Gaze_InteractiveObject IOScript;

        void Start()
        {
            // otherwise the event is fired too early
            IOScript = GetComponentInParent<Gaze_InteractiveObject>();
            proximityEventArgs = new Gaze_ProximityEventArgs(IOScript);
            gameObject.layer = LayerMask.NameToLayer(Gaze_HashIDs.LAYER_PROXIMTY);
            StartCoroutine(NotifyAtStart());
        }

        private IEnumerator NotifyAtStart()
        {
            yield return new WaitForEndOfFrame();
            if (proximityFlag)
            {
                proximityEventArgs.Other = otherGameObject.GetComponentInParent<Gaze_InteractiveObject>();
                proximityEventArgs.IsInProximity = true;
                Gaze_EventManager.FireProximityEvent(proximityEventArgs);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Gaze_Proximity>() != null)
            {
                if (debug)
                    Debug.Log("Gaze_Proximity (" + transform.parent.name + ") OnTriggerEnter with " + other.GetComponentInParent<Gaze_InteractiveObject>().name);

                otherGameObject = other.gameObject;

                proximityEventArgs.Other = otherGameObject.GetComponentInParent<Gaze_InteractiveObject>();
                proximityEventArgs.IsInProximity = true;
                Gaze_EventManager.FireProximityEvent(proximityEventArgs);
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

                proximityEventArgs.Other = otherGameObject.GetComponentInParent<Gaze_InteractiveObject>();
                proximityEventArgs.IsInProximity = false;
                Gaze_EventManager.FireProximityEvent(proximityEventArgs);
            }
        }
    }
}