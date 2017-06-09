using UnityEngine;
using UnityTest;


public abstract class Gaze_AbstractTest : TestComponent
{
    private int numUpdatesToSkip = 0;
    private float testSucceedTime = -1;
    private static int objectsMovedAway = 0;
    private const int DISTANCE_BETWEEN_OBJECTS_MOVED_AWAY = 100;

    public abstract void Gaze_Update();

    private void Update()
    {
        if (testSucceedTime > 0 && Time.time > testSucceedTime)
            PassTest();

        if (IsUpdateSkipNeeded())
            return;

        Gaze_Update();
    }

    private bool IsUpdateSkipNeeded()
    {
        if (numUpdatesToSkip > 0)
        {
            numUpdatesToSkip--;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Skips some updates in order to allow your system to update some states before
    /// testing something.
    /// </summary>
    /// <param name="_numUpdatesToSkip"> By default it skips 10 updates </param>
    protected void SkipUpdates(int _numUpdatesToSkip=10)
    {
        numUpdatesToSkip = _numUpdatesToSkip;
    }

    protected static void PutObjectAwayAllProximities(GameObject go)
    {
        go.transform.position = new Vector3(float.MaxValue, objectsMovedAway % 2 == 0 ? 0 : objectsMovedAway * DISTANCE_BETWEEN_OBJECTS_MOVED_AWAY, objectsMovedAway % 2 != 0 ? 0 : objectsMovedAway * DISTANCE_BETWEEN_OBJECTS_MOVED_AWAY);
        objectsMovedAway++;
    }

    protected static void PutObjectInProximityWithOther(GameObject objectToMove, GameObject objectToBeInProximity)
    {
        objectToMove.transform.position = objectToBeInProximity.transform.position;
    }

    public static void PassTest()
    {
        IntegrationTest.Pass();
    }

    public static void FailTest(string _message="")
    {
        IntegrationTest.Fail(_message);
    }

    protected static void PutObjectInfrontOfCamera(GameObject _objectToGaze, Camera _customCamera = null)
    {
        Camera camera = _customCamera == null ? Camera.main : _customCamera;
        _objectToGaze.transform.position = camera.transform.position + camera.transform.forward * 2;
    }

    protected void TrySuceedJustBeforeTimeout()
    {
        testSucceedTime = Time.time + (float)GetTimeout() * 0.9f;
    }
}
