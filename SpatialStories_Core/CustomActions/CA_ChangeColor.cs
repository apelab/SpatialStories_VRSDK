using Gaze;
using UnityEngine;

public class CA_ChangeColor : Gaze_AbstractBehaviour
{
    private MeshRenderer m_MeshRedenrer;

    private void Start()
    {
        m_MeshRedenrer = gazable.RootIO.GetComponentInChildren<MeshRenderer>();
    }

    protected override void OnTrigger()
    {
        m_MeshRedenrer.material.color = Random.ColorHSV();
    }

    public override void SetupUsingApi(GameObject _interaction)
    {
    }
}
