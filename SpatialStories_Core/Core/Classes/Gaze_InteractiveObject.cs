// <copyright file="Gaze_InteractiveObject.cs" company="apelab sàrl">
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
using System;
using UnityEngine;

namespace Gaze
{
    [Serializable]
    [SelectionBase]
    public class Gaze_InteractiveObject : MonoBehaviour
    {
        #region Members

        /// <summary>
        /// If this is active the gameobject won't be detached of the hand
        /// </summary>
        public bool IsSticky { get { return isSticky; } }
        private bool isSticky = false;
        
        /// <summary>
        /// Is this catchable object using gravity
        /// </summary>
        //		public bool hasGravity;

        // TODO test if this works with a FBX that already has a root motion
        public Transform RootMotion;
                
        public Gaze_GravityState ActualGravityState { get { return actualGravityState; } }
        private Gaze_GravityState actualGravityState;

        private Gaze_GravityState initialGravityState;

        public Gaze_Transform InitialTransform { get { return initialTransform; } }

        private Gaze_Transform initialTransform;
        
        #endregion Members

        private void Awake()
        {
            SetActualGravityStateAsDefault();

            initialTransform = new Gaze_Transform(transform);       
           
            if (RootMotion != null)
            {
                transform.SetParent(RootMotion);
            }
        }

        /// <summary>
        /// Notify to everyone that this IO has been destroyed 
        /// </summary>
        private void OnDestroy()
        {
            Gaze_EventManager.FireOnIODestroyed(new Gaze_IODestroyEventArgs(this, this));
        }

        #region GravityManagement
        /// <summary>
        /// Checks the actual gravity state and store it as default
        /// </summary>
        public void SetActualGravityStateAsDefault()
        {
            Rigidbody rigidBody = GetRigitBodyOrError();
            if (rigidBody == null)
                return;

            if (rigidBody.isKinematic)
            {

                initialGravityState = rigidBody.useGravity ? Gaze_GravityState.ACTIVE_KINEMATIC : Gaze_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                initialGravityState = rigidBody.useGravity ? Gaze_GravityState.ACTIVE_NOT_KINEMATIC : Gaze_GravityState.UNACTIVE_NOT_KINEMATIC;
            }

            actualGravityState = initialGravityState;
        }

        /// <summary>
        /// If the gravity of the IO is not locked it will return to its default state.
        /// </summary>
        public void ReturnToInitialGravityState()
        {
            Rigidbody rigidBody = GetRigitBodyOrError();

            if (rigidBody == null)
                return;

            switch (initialGravityState)
            {
                case Gaze_GravityState.ACTIVE_KINEMATIC:
                    rigidBody.useGravity = true;
                    rigidBody.isKinematic = true;
                    break;
                case Gaze_GravityState.ACTIVE_NOT_KINEMATIC:
                    rigidBody.useGravity = true;
                    rigidBody.isKinematic = false;
                    break;
                case Gaze_GravityState.UNACTIVE_KINEMATIC:
                    rigidBody.useGravity = false;
                    rigidBody.isKinematic = true;
                    break;
                case Gaze_GravityState.UNACTIVE_NOT_KINEMATIC:
                    rigidBody.useGravity = false;
                    rigidBody.isKinematic = false;
                    break;
            }
        }

        // Make the IO not listen gravity requests
        public void LockGravity()
        {
            actualGravityState = Gaze_GravityState.LOCKED;
        }

        /// <summary>
        /// Make the IO listen for gravity requests.
        /// </summary>
        public void UnlockGravity()
        {
            Rigidbody rigidBody = GetRigitBodyOrError();
            if (rigidBody == null)
                return;

            if (rigidBody.isKinematic)
            {
                actualGravityState = rigidBody.useGravity ? Gaze_GravityState.ACTIVE_KINEMATIC : Gaze_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                actualGravityState = rigidBody.useGravity ? Gaze_GravityState.ACTIVE_NOT_KINEMATIC : Gaze_GravityState.UNACTIVE_NOT_KINEMATIC;
            }
        }

        /// <summary>
        /// Sets ONLY the gravity of a game object to true or false and changes
        /// its state.
        /// </summary>
        /// <param name="_hasGravity"></param>
        public void SetGravity(bool _hasGravity)
        {
            if (IsGravityLocked())
            {
                WarnUnauthorizedGravityChange();
                return;
            }


            Rigidbody rb = GetRigitBodyOrError();

            if (rb == null)
                return;
            rb.useGravity = _hasGravity;

            if (rb.isKinematic)
            {
                actualGravityState = _hasGravity ? Gaze_GravityState.ACTIVE_KINEMATIC : Gaze_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                actualGravityState = _hasGravity ? Gaze_GravityState.ACTIVE_NOT_KINEMATIC : Gaze_GravityState.UNACTIVE_NOT_KINEMATIC;
            }

        }

        /// <summary>
        /// Sets the gravity of a game object and also the kinematic state in order to 
        /// attach it or not.
        /// </summary>
        /// <param name="_hasGravity"></param>
        public void SetGravityAndAttach(bool _hasGravity)
        {
            if (IsGravityLocked())
            {
                WarnUnauthorizedGravityChange();
                return;
            }

            Rigidbody rb = GetRigitBodyOrError();

            if (rb == null)
                return;

            rb.isKinematic = !_hasGravity;
            rb.useGravity = _hasGravity;

            actualGravityState = _hasGravity ? Gaze_GravityState.ACTIVE_NOT_KINEMATIC : Gaze_GravityState.UNACTIVE_NOT_KINEMATIC;

        }

        public Rigidbody GetRigitBodyOrError()
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null && Gaze_GravityManager.SHOW_GRAVITY_WARNINGS)
            {
                Debug.LogWarning(String.Format("The interactive object {0} has not a rigidbody and it should, please add a rigidbody to it.", gameObject.name));
            }

            return rb;
        }

        /// <summary>
        /// This method is called if we detect gravity inconsistencies in order to prevent
        /// developpers to change the gravity directly, 
        /// </summary>
        public void WarnUnauthorizedGravityChange()
        {
            Debug.LogWarning("Gravity chages should be only performed by the GravityManager!");
        }

        public bool IsGravityLocked()
        {
            return actualGravityState == Gaze_GravityState.LOCKED;
        }

        public bool IsAffectedByGravity()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            return rb != null && !rb.isKinematic && rb.useGravity;
        }

        public void AddInteraction()
        {
            GameObject interactionsRoot = GetComponentInChildren<Gaze_Interaction>().transform.parent.gameObject;

            GameObject interactionHide = new GameObject("HideOnDrop");
            interactionHide.transform.parent = interactionsRoot.transform;
            Gaze_Interaction i1 = interactionHide.AddComponent<Gaze_Interaction>();
            i1.AddActions();
            i1.AddConditions();
            Gaze_Conditions c1 = i1.GetComponent<Gaze_Conditions>();
            c1.reload = true;
            c1.reloadModeIndex = (int)Gaze_ReloadMode.INFINITE;
            Gaze_Actions a1 = i1.GetComponent<Gaze_Actions>();
            a1.ActionVisuals = Gaze_Actions.ACTIVABLE_OPTION.DEACTIVATE;

            GameObject interactionShow = new GameObject("ShowOnRemove");
            interactionShow.transform.parent = interactionsRoot.transform;
            Gaze_Interaction i2 = interactionShow.AddComponent<Gaze_Interaction>();
            i2.AddActions();
            i2.AddConditions();
            Gaze_Conditions c2 = i2.GetComponent<Gaze_Conditions>();
            c2.reload = true;
            c2.reloadModeIndex = (int)Gaze_ReloadMode.INFINITE;
            Gaze_Actions a2 = i2.GetComponent<Gaze_Actions>();
            a2.ActionVisuals = Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE;
        }

        public void RemoveInteractions()
        {
            Gaze_Interaction[] interactions = GetComponentsInChildren<Gaze_Interaction>();
            int interactionsCount = interactions.Length;
            for (int i = 0; i < interactionsCount; i++)
            {
                DestroyImmediate(interactions[i]);
            }
        }
        #endregion GravityManagement

        #region ManipulationManagement

        public bool IsPointedWithLeftHand;

        public bool IsPointedWithRightHand;
        
        #endregion ManipulationManagement
    }
}