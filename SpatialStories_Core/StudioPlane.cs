using UnityEngine;

public class StudioPlane : MonoBehaviour {

    private void OnEnable()
    {
        StudioPlaneManager.Instance.RegisterPlane(gameObject);
        StudioPlaneManager.Instance.UpdatePlanesVisibility();    
    }

    private void OnDisable()
    {
        StudioPlaneManager.Instance.UnRegisterPlane(gameObject);
    }
}
