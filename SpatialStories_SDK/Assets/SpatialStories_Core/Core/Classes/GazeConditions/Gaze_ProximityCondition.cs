using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public class Gaze_ProximityCondition : Gaze_AbstractCondition
    {
        private Collider gazeCollider;

        /// <summary>
        /// Whether this Interactive Object's Proximity is colliding.
        /// </summary>
        public bool isInProximity;

        public Gaze_ProximityCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript)
        {
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        protected override void CustomSetup()
        {
            Gaze_EventManager.OnProximityEvent += OnProximityEvent;
        }

        protected override void CustomDispose()
        {
            Gaze_EventManager.OnProximityEvent -= OnProximityEvent;
        }

        public override void ToEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsValid)
            {
                RenderSatisfiedLabel("Proximities Validated:");
                RenderSatisfiedLabel("True");
            }
            else
            {
                RenderNonSatisfiedLabel("Proximities Validated:");
                RenderNonSatisfiedLabel("False");
            }
            EditorGUILayout.EndHorizontal();

            string objectsInProximity = "";

            foreach (Gaze_ProximityEntry pe in gazeConditionsScript.proximityMap.proximityEntryList)
            {
                foreach (Gaze_InteractiveObject go in pe.CollidingObjects)
                {
                    objectsInProximity = string.Concat(objectsInProximity, ", ", go.name);
                }
            }
            if (objectsInProximity.Length > 0)
            {
                RenderSatisfiedLabel("Objects in proximity:");
                RenderSatisfiedLabel(objectsInProximity.Substring(1, objectsInProximity.Length - 1));

            }
            else
            {
                RenderNonSatisfiedLabel("Objects in proximity:");
                RenderNonSatisfiedLabel("---");
            }
            EditorGUILayout.Space();
        }

        private void OnProximityEvent(Gaze_ProximityEventArgs e)
        {
            if (e.Sender.Equals(gazeConditionsScript.RootIO))
                isInProximity = e.IsInProximity;

            IsValid = HandleProximity(e);
        }

        private void ResetProximitiesCondition()
        {
            Debug.Log("ResetProximitiesCondition()");
            IsValid = false;
            gazeConditionsScript.proximityMap.ResetEveryoneColliding();
        }

        private int collisionsOccuringCount = 0;
        /// <summary>
        /// Checks the proximities conditions validity.
        /// </summary>
        /// <returns><c>true</c>, if proximities was checked, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool HandleProximity(Gaze_ProximityEventArgs e)
        {
            // get colliding objects
            Gaze_InteractiveObject sender = ((Gaze_InteractiveObject)e.Sender).GetComponentInChildren<Gaze_Proximity>().IOScript;
            Gaze_InteractiveObject other = ((Gaze_InteractiveObject)e.Other).GetComponentInChildren<Gaze_Proximity>().IOScript;
            // make sure the collision concerns two objects in the list of proximities (-1 if NOT)

            int otherIndex = IsCollidingObjectsInList(other, sender);
            //				Debug.Log ("otherIndex = " + otherIndex);
            if (otherIndex != -1)
            {

                // OnEnter and tested only if validation is not already true (to avoid multiple collision to toggle the proximitiesValidated flag
                if (e.IsInProximity && !IsValid)
                {
                    // update number of collision in the list occuring
                    collisionsOccuringCount++;
                    gazeConditionsScript.proximityMap.AddCollidingObjectToEntry(gazeConditionsScript.proximityMap.proximityEntryList[otherIndex], sender.GetComponentInChildren<Gaze_Proximity>().IOScript);

                    if (gazeConditionsScript.proximityMap.proximityStateIndex.Equals((int)Gaze_ProximityStates.ENTER))
                    {
                        // get number of valid entries
                        int validatedEntriesCount = gazeConditionsScript.proximityMap.GetValidatedEntriesCount();
                        // OnEnter + RequireAll
                        if (gazeConditionsScript.requireAllProximities)
                        {
                            return validatedEntriesCount == gazeConditionsScript.proximityMap.proximityEntryList.Count;
                        }
                        // OnEnter + NOT RequireAll
                        if (!gazeConditionsScript.requireAllProximities)
                        {
                            return validatedEntriesCount >= 2;
                        }
                    }
                    // OnExit
                }
                else if (!e.IsInProximity)
                {
                    // update number of collision in the list occuring
                    collisionsOccuringCount--;
                    // update everyoneIsColliding tag before removing an element
                    gazeConditionsScript.proximityMap.UpdateEveryoneColliding();
                    // remove colliding object
                    gazeConditionsScript.proximityMap.RemoveCollidingObjectToEntry(gazeConditionsScript.proximityMap.proximityEntryList[otherIndex], sender.GetComponentInChildren<Gaze_Proximity>().IOScript);
                    // if proximity condition is EXIT
                    if (gazeConditionsScript.proximityMap.proximityStateIndex.Equals((int)Gaze_ProximityStates.EXIT))
                    {

                        if (gazeConditionsScript.requireAllProximities)
                        {
                            // every entry was colliding before the exit
                            if (gazeConditionsScript.proximityMap.IsEveryoneColliding)
                                IsValid = true;
                            else
                                IsValid = false;
                            // OnExit + NOT RequireAll
                        }
                        else
                        {
                            gazeConditionsScript.proximityMap.ResetEveryoneColliding();
                            IsValid = true;
                        }
                        // if proximity condition is ENTER
                    }
                    else
                    {
                        // if proximities was validated
                        if (IsValid)
                        {
                            // and if require all
                            if (gazeConditionsScript.requireAllProximities)
                            {
                                return false;
                            }
                            else
                            {
                                // check there's a valid collision left in the list...
                                if (collisionsOccuringCount > 0)
                                    IsValid = true;
                                else
                                    IsValid = false;
                            }
                        }
                    }
                }

            }
            return IsValid;
        }


        /// <summary>
        /// Check if both colliding objects are in the list.
        /// </summary>
        /// <returns>The colliding's (_other) index within list.
        /// If one of the object is not in the list, return -1</returns>
        /// <param name="_other">Other.</param>
        /// <param name="_sender">Sender.</param>
        private int IsCollidingObjectsInList(Gaze_InteractiveObject _other, Gaze_InteractiveObject _sender)
        {
            //			Debug.Log ("other = " + _other.GetComponentInParent<Gaze_InteractiveObject> ().name + " and sender = " + _sender.GetComponentInParent<Gaze_InteractiveObject> ().name);
            int found = 0;
            int otherIndex = -1;
            int tmpIndex = -1;
            for (int i = 0; i < gazeConditionsScript.proximityMap.proximityEntryList.Count; i++)
            {
                //				Debug.Log ("proximityMap.proximityEntryList [i].dependentGameObject = " + proximityMap.proximityEntryList [i].dependentGameObject.GetComponentInParent<Gaze_InteractiveObject> ().name);
                if (gazeConditionsScript.proximityMap.proximityEntryList[i].dependentGameObject.Equals(_other))
                {
                    tmpIndex = i;
                    found++;
                }
                if (gazeConditionsScript.proximityMap.proximityEntryList[i].dependentGameObject.Equals(_sender))
                {
                    found++;
                }
                if (found == 2)
                {
                    otherIndex = tmpIndex;
                    break;
                }
            }
            //			Debug.Log (GetComponentInParent<Gaze_InteractiveObject> ().name + " found = " + found + " with otherIndex = " + otherIndex);
            return otherIndex;
        }

        /// <summary>
        /// Check if both colliding objects are in the list.
        /// </summary>
        /// <returns>The colliding's (_other) index within list.
        /// If one of the object is not in the list, return -1</returns>
        /// <param name="_other">Other.</param>
        /// <param name="_sender">Sender.</param>
        private int IsCollidingObjectsWithinList(GameObject _other, GameObject _sender)
        {
            int found = 0;
            int otherIndex = -1;
            int tmpIndex = -1;
            for (int i = 0; i < gazeConditionsScript.proximityMap.proximityEntryList.Count; i++)
            {
                if (gazeConditionsScript.proximityMap.proximityEntryList[i].dependentGameObject.Equals(_other))
                {
                    tmpIndex = i;
                    found++;
                }
                if (gazeConditionsScript.proximityMap.proximityEntryList[i].dependentGameObject.Equals(_sender))
                {
                    found++;
                }

                if (found == 2)
                {
                    otherIndex = tmpIndex;
                    break;
                }
            }

            return otherIndex;
        }
    }
}
