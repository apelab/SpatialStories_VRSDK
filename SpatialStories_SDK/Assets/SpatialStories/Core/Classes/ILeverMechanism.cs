using UnityEngine;

/// <summary>
/// Interface to describe how a lever should move.
/// </summary>
public interface ILeverMechanism
{
    /// <summary>
    /// Compute the lever position from the new controller position
    /// </summary>
    /// <param name="controllerWorldPosition">The controller position in world space</param>
    /// <returns>Returns an integer either equal to the step number the lever has reached or -1
    /// if it didn't reach any steps</returns>
    int ComputeLeverPosition(Vector3 controllerWorldPosition);
}
