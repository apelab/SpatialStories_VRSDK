using System;
using Gaze;
using GoogleARCore;
using SpatialStories;
using UnityEngine;

public class CA_ARKitMove : Gaze_AbstractBehaviour {

    public GameObject ObjectToMove;
    public Gaze_ArkitPlaceConstraints Constraints;
    public float HeightOffset;
    public float DistanceFromCamera;
    public bool DeactivateAtStart;

    private Anchor m_anchorImage;

    private void Start()
    {
        if (DeactivateAtStart)
            ObjectToMove.SetActive(false);

        SpatialStoriesEventController.Instance.SpatialStoriesEvent += new SpatialStoriesEventHandler(OnSpatialStoriesEvent);
    }

    void OnDestroy()
    {
        SpatialStoriesEventController.Instance.SpatialStoriesEvent -= OnSpatialStoriesEvent;
    }

    private void OnSpatialStoriesEvent(string _nameEvent, object[] _list)
    {
        if (_nameEvent == S_ARCoreCameraRaycaster.EVENT_ARCORECAMERA_RAYCAST_IMAGE_ANCHOR)
        {
            m_anchorImage = (Anchor)_list[0];
        }
    }

    protected override void OnTrigger()
    {
        Vector3 pos = Vector3.zero;
        Anchor anchor = null;
        switch (Constraints)
        {
            case Gaze_ArkitPlaceConstraints.IN_FRONT_OF_CAMERA:
                Vector3 forward = Gaze_CameraRaycaster.RaycasterTransform.forward;
                pos = Gaze_CameraRaycaster.RaycasterTransform.position + forward * DistanceFromCamera;
                SetGlobalPosition(anchor, pos);
                break;
            case Gaze_ArkitPlaceConstraints.OVER_AN_OBJECT:
                pos = Gaze_CameraRaycaster.ClosestHitOverObject.point;
                anchor = ((TrackableHit)Gaze_CameraRaycaster.ClosestHitOverPlaneTrackable).Trackable.CreateAnchor(((TrackableHit)Gaze_CameraRaycaster.ClosestHitOverPlaneTrackable).Pose);
                SetGlobalPosition(anchor, pos);
                break;
            case Gaze_ArkitPlaceConstraints.OVER_A_PLANE:
                pos = Gaze_CameraRaycaster.ClosestHitOverPlane.point;
                anchor = ((TrackableHit)Gaze_CameraRaycaster.ClosestHitOverPlaneTrackable).Trackable.CreateAnchor(((TrackableHit)Gaze_CameraRaycaster.ClosestHitOverPlaneTrackable).Pose);
                SetGlobalPosition(anchor, pos);
                break;
            case Gaze_ArkitPlaceConstraints.OVER_AN_IMAGE:
                pos = m_anchorImage.transform.position;
                anchor = m_anchorImage;
                SetGlobalPosition(anchor, pos);
                break;
            default:
                pos = Gaze_CameraRaycaster.ClosestHit.point;
                SetGlobalPosition(anchor, pos);
                break;
        }

    }

    private void SetGlobalPosition(Anchor _anchor, Vector3 _pos)
    {
        if (_anchor != null)
        {
            ObjectToMove.transform.parent = _anchor.transform;
        }
        ObjectToMove.transform.position = _pos + Vector3.up * HeightOffset;

        if (DeactivateAtStart)
        {
            ObjectToMove.SetActive(true);
        }

        SpatialStoriesEventController.Instance.DispatchBasicSystemEvent(S_ARCoreController.EVENT_ARCORE_CONTROLLER_DISABLE_DETECTION);
    }

    private void SetLocalPosition(Anchor _anchor, Vector3 _pos)
    {
        if (_anchor != null)
        {
            ObjectToMove.transform.parent = _anchor.transform;
        }
        ObjectToMove.transform.localPosition = _pos + Vector3.up * HeightOffset;

        if (DeactivateAtStart)
        {
            ObjectToMove.SetActive(true);
        }
    }

    public override void SetupUsingApi(GameObject _interaction)
    {
        ObjectToMove = (GameObject)creationData[0];
        Constraints = (Gaze_ArkitPlaceConstraints)creationData[1];
        HeightOffset = (float)creationData[2];
        DistanceFromCamera = (float)creationData[3];
        DeactivateAtStart = (bool)creationData[4];
    }
}

/// <summary>
/// Example of how making the API more friendly by creating wrapper classes outside
/// the API definition
/// </summary>
public static partial class APIExtensions
{
    public static CA_ARKitMove CreateArkitMoveAction(this S_InteractionDefinition _def, GameObject _objectToMove,
        Gaze_ArkitPlaceConstraints _constraints, float _heigthOffset, float _distanceFromCamera, bool _dectivateAtStart)
    {
        return _def.CreateAction<CA_ARKitMove>(_objectToMove, _constraints, _heigthOffset, _distanceFromCamera, _dectivateAtStart);
    }
}
