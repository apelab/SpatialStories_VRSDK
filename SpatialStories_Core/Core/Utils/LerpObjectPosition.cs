using Gaze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpObjectPosition : MonoBehaviour {

    private IEnumerator lerpCoroutine;
    private float moveSpeed = .1f;

    public void SetNewTrip(Vector3 _origin, Vector3 _destination, float _moveSpeed = .1f)
    {

        // store the delta moveSpeed
        moveSpeed = _moveSpeed;

        // if there's already a coroutine moving the object, stop it
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        // set the new coroutine values
        lerpCoroutine = LerpPosition(_origin, _destination);

        // start the coroutine
        StartCoroutine(lerpCoroutine);
    }

    private IEnumerator LerpPosition(Vector3 _origin, Vector3 _destination)
    {
        float deltaThreshold = 0.1f;
        transform.position = _origin;

        // get the IOs
        Gaze_InteractiveObject[] IOs = GetComponentsInChildren<Gaze_InteractiveObject>();

        // deactivate the gravity
        for (int i = 0; i < IOs.Length; i++)
            Gaze_GravityManager.ChangeGravityState(IOs[i], Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);

        //
        float deltaPosition = Vector3.Distance(transform.position, _destination);
        while (deltaPosition > deltaThreshold)
        {
            //Debug.Log("lerping with deltaPosition = "+ deltaPosition);
            Vector3 newPos = Vector3.Lerp(_origin, _destination, Time.time * moveSpeed);
            transform.position = newPos;
            deltaPosition = Vector3.Distance(transform.position, _destination);
            yield return null;
        }

        // set the final destination (removing the threshold distance left)
        transform.position = _destination;

        // re-activate the gravity
        for (int i = 0; i < IOs.Length; i++)
            Gaze_GravityManager.ChangeGravityState(IOs[i], Gaze_GravityRequestType.RETURN_TO_DEFAULT);
    }
}
