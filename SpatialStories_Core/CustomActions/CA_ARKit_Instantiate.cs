using Gaze;
using SpatialStories;
using System.Collections.Generic;
using UnityEngine;

public class CA_ARKit_Instantiate : Gaze_AbstractBehaviour
{
    public List<GameObject> Prefabs = new List<GameObject>();
    public List<GameObject> Cursors = new List<GameObject>();

    public Gaze_ArkitPlaceConstraints Constraints;
    public float HeightOffset;
    public float DistanceFromCamera;
    public bool ChooseRandom = false;

    [HideInInspector]
    public int choosenIndex = 0;

    private Gaze_Conditions conditions;
    private GameObject prefabToInstantiate;
    private GameObject cursor;

    private void Start()
    {
        conditions = GetComponent<Gaze_Conditions>();
    }

    public GameObject GetNextPrefab()
    {
        GameObject toReturn;

        if (ChooseRandom)
        {
            choosenIndex = Random.Range(0, Prefabs.Count);
            toReturn = Prefabs[choosenIndex];
        }
        else
        {
            if (choosenIndex >= Prefabs.Count)
                choosenIndex = 0;

            toReturn = Prefabs[choosenIndex];
        }

        return toReturn;
    }

    protected override void OnTrigger()
    {
        Vector3 pos = Vector3.zero;

        GameObject goToInstantiate = GameObject.Instantiate(prefabToInstantiate);

        switch (Constraints)
        {
            case Gaze_ArkitPlaceConstraints.IN_FRONT_OF_CAMERA:
                Vector3 forward = Gaze_CameraRaycaster.RaycasterTransform.forward;
                pos = Gaze_CameraRaycaster.RaycasterTransform.position + forward * DistanceFromCamera;
                break;
            case Gaze_ArkitPlaceConstraints.OVER_AN_OBJECT:
                pos = Gaze_CameraRaycaster.ClosestHitOverObject.point;
                break;
            case Gaze_ArkitPlaceConstraints.OVER_A_PLANE:
                pos = Gaze_CameraRaycaster.ClosestHitOverPlane.point;
                break;
            default:
                pos = Gaze_CameraRaycaster.ClosestHit.point;
                break;
        }

        goToInstantiate.transform.position = pos + Vector3.up * HeightOffset;

        if (conditions.reloadMaxRepetitions == conditions.reloadCount)
        {
            if (cursor != null)
                Destroy(cursor);
        }

        ++choosenIndex;
    }

    public override void SetupUsingApi(GameObject _interaction)
    {
        Constraints = (Gaze_ArkitPlaceConstraints)creationData[0];
        HeightOffset = (float)creationData[1];
        DistanceFromCamera = (float)creationData[2];
        ChooseRandom = (bool)creationData[3];

        bool finite = (bool)creationData[4];
        int numObjectsToInstantiate = (int)creationData[5];

        Gaze_Conditions conditions = GetComponent<Gaze_Conditions>();

        if (numObjectsToInstantiate > 1 || !finite)
        {
            conditions.reload = true;
        }

        conditions.reloadModeIndex = finite ? (int)Gaze_ReloadMode.FINITE : (int)Gaze_ReloadMode.INFINITE;
        conditions.reloadMaxRepetitions = finite ? numObjectsToInstantiate - 1 : 0;

        Prefabs = new List<GameObject>((List<GameObject>)creationData[6]);
        Prefabs.Reverse();
        Cursors = new List<GameObject>((List<GameObject>)creationData[7]);
        Cursors.Reverse();
    }

    private void Update()
    {
        if (cursor != null)
        {
            Vector3 pos = Vector3.zero;
            bool needToShowCursor = true;

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
    }

    protected override void OnActive()
    {
        prefabToInstantiate = GetNextPrefab();

        if (Cursors[choosenIndex] != null)
        {
            if (cursor == null)
                cursor = Instantiate(Cursors[choosenIndex]);
        }
    }

    protected override void OnReload()
    {
        if (cursor != null)
            Destroy(cursor);

        prefabToInstantiate = GetNextPrefab();

        if (Cursors[choosenIndex] != null)
        {
            if (cursor == null)
                cursor = Instantiate(Cursors[choosenIndex]);
        }
    }
}

/// <summary>
/// Example of how making the API more friendly by creating wrapper classes outside
/// the API definition
/// </summary>
public static partial class APIExtensions
{
    public static CA_ARKit_Instantiate CreateArkitInstantiateAction(this S_InteractionDefinition _def,
        Gaze_ArkitPlaceConstraints _constraints, float _heigthOffset, float _distanceFromCamera, bool _chooseRandom, bool finite, int numReloads, List<GameObject> _prefabs, List<GameObject> _ghosts)
    {
        CA_ARKit_Instantiate action = _def.CreateAction<CA_ARKit_Instantiate>(_constraints, _heigthOffset,
            _distanceFromCamera, _chooseRandom, finite, numReloads, _prefabs, _ghosts);
        return action;
    }
}
