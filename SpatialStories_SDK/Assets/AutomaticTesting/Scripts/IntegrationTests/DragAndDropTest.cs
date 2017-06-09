using Gaze;
using UnityEngine;

public class DragAndDropTest : Gaze_AbstractTest
{
    protected bool isTriggerAllowed = false;
    public GameObject CubeToMove;
    public GameObject CubeGhost;

    private enum TEST_PHASE { BEFORE_EXPOSITION, EXPOSING, WAITING_FOR_TRIGGER}
    private TEST_PHASE actualTestPhase = TEST_PHASE.BEFORE_EXPOSITION;

    private void OnEnable()
    {
        Gaze_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;
    }

    private void OnDisable()
    {
        Gaze_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;
    }

    private void OnDragAndDropEvent(Gaze_DragAndDropEventArgs e)
    {
        Debug.Log(e.State);
        PassTest();
        
    }

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.BEFORE_EXPOSITION:
                SkipUpdates();
                actualTestPhase = TEST_PHASE.EXPOSING;
                break;
            case TEST_PHASE.EXPOSING:
                CubeToMove.transform.position = CubeGhost.transform.position;
                CubeToMove.transform.rotation = CubeGhost.transform.rotation;
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                break;
            case TEST_PHASE.WAITING_FOR_TRIGGER:
                break;
        }        
    }
}
