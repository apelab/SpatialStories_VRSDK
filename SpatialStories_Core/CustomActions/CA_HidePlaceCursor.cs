using Gaze;
using SpatialStories;
using UnityEngine;

public class CA_HidePlaceCursor : Gaze_AbstractBehaviour {

    public CA_ShowPlaceCursor CursorPlacerAction;

    public override void SetupUsingApi(GameObject _interaction)
    {
        string gazeConditionsGUID = (string)creationData[0];
        CursorPlacerAction = SpatialStoriesAPI.GetObjectOfTypeWithGUID(gazeConditionsGUID).GetComponent<CA_ShowPlaceCursor>();
    }

    protected override void OnTrigger()
    {
        CursorPlacerAction.Deactivate();
    }
}

/// <summary>
/// Example of how making the API more friendly by creating wrapper classes outside
/// the API definition
/// </summary>
public static partial class APIExtensions
{
    public static CA_HidePlaceCursor CreateArkitHidePlaceCursor(this S_InteractionDefinition _def,
        string _showCursorInteractionGUID)
    {
        return _def.CreateAction<CA_HidePlaceCursor>(_showCursorInteractionGUID);
    }
}