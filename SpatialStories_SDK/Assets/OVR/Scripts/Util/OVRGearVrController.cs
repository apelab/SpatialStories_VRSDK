using UnityEngine;
using System.Collections;

public class OVRGearVrController : MonoBehaviour
{
    public GameObject m_model;
    private bool m_prevControllerConnected = false;
    private bool m_prevControllerConnectedCached = false;

	void Start()
    {

	}

    void Update()
    {
        bool controllerConnected = OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote) || OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote);

        if ((controllerConnected != m_prevControllerConnected) || !m_prevControllerConnectedCached)
        {
            m_model.SetActive(controllerConnected);
            m_prevControllerConnected = controllerConnected;
            m_prevControllerConnectedCached = true;
        }

        if (!controllerConnected)
        {
            return;
        }

        transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.GetActiveController());
        transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.GetActiveController());
    }
}
