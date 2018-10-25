using Gaze;
using SpatialStories;
using UnityEngine;

public class CA_ShowPlaceCursor : Gaze_AbstractBehaviour
{

    public GameObject CursorPrefab;
    private GameObject cursor;
    public Gaze_ArkitPlaceConstraints Constraints;
    public float HeightOffset;
    public float DistanceFromCamera;
    public bool DeactivateAtStart;

    private Gaze_Conditions conditions;
    private bool needToShowCursor = false;

    bool isActive = false;

    protected override void OnTrigger()
    {
        if (cursor == null)
            cursor = Instantiate(CursorPrefab);
        isActive = true;
    }

    // Use this for initialization
    void Start()
    {
        conditions = GetComponent<Gaze_Conditions>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
            return;

        Vector3 pos = Vector3.zero;
        needToShowCursor = true;

        switch (Constraints)
        {
            case Gaze_ArkitPlaceConstraints.IN_FRONT_OF_CAMERA:
                Vector3 forward = Gaze_CameraRaycaster.RaycasterTransform.forward;
                pos = Gaze_CameraRaycaster.RaycasterTransform.position + forward * DistanceFromCamera;
                break;
            case Gaze_ArkitPlaceConstraints.OVER_AN_OBJECT:
                if (!Gaze_CameraRaycaster.DetectedObjectThisFrame)
                    needToShowCursor = false;
                pos = Gaze_CameraRaycaster.ClosestHitOverObject.point;
                break;
            case Gaze_ArkitPlaceConstraints.OVER_A_PLANE:
                if (!Gaze_CameraRaycaster.DetectedPlaneThisFrame)
                    needToShowCursor = false;
                pos = Gaze_CameraRaycaster.ClosestHitOverPlane.point;
                break;
            default:
                pos = Gaze_CameraRaycaster.ClosestHit.point;
                break;
        }

        cursor.transform.position = pos + Vector3.up * HeightOffset;
        cursor.SetActive(needToShowCursor);
    }

    public override void SetupUsingApi(GameObject _interaction)
    {
        CursorPrefab = (GameObject)creationData[0];
        Constraints = (Gaze_ArkitPlaceConstraints)creationData[1];
        HeightOffset = (float)creationData[2];
        DistanceFromCamera = (float)creationData[3];
    }

    public void Deactivate()
    {
        isActive = false;
        cursor.SetActive(false);
    }
}

/// <summary>
/// Example of how making the API more friendly by creating wrapper classes outside
/// the API definition
/// </summary>
public static partial class APIExtensions
{
    public static CA_ShowPlaceCursor CreateArkitPlaceCursorAction(this S_InteractionDefinition _def,
        GameObject _cursorPrefab, Gaze_ArkitPlaceConstraints _constraints, float _heigthOffset, float _distanceFromCamera)
    {
        return _def.CreateAction<CA_ShowPlaceCursor>(_cursorPrefab, _constraints, _heigthOffset, _distanceFromCamera);
    }
}