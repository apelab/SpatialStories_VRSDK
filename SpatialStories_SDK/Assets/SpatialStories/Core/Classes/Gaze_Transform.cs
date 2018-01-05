using UnityEngine;
public class Gaze_Transform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Gaze_Transform transform;
    public Gaze_Transform() { }
    public Gaze_Transform(Transform _transform)
    {
        position = _transform.position;
        rotation = _transform.rotation;
        scale = _transform.localScale;
    }
    public Gaze_Transform(Vector3 _postion, Quaternion _rotation, Vector3 _scale)
    {
        position = _postion;
        rotation = _rotation;
        scale = _scale;
    }
}