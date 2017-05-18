using UnityEngine;

public class Gaze_SnapPosition : MonoBehaviour
{
    public enum HAND { LEFT, RIGHT }
    public HAND ActualHand;

    [HideInInspector]
    public Quaternion Rotation;

    [HideInInspector]
    public Vector3 Position;

    // Use this for initialization
    void Start()
    {
        Rotation = transform.localRotation;
        Position = transform.localPosition;
        Debug.Log(Rotation);
        Debug.Log(Position);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
