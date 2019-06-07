using Gaze;
using UnityEngine;

[AddComponentMenu("SpatialStories/Custom Actions/CA_Activate")]


public class CA_ActivateGameObject : Gaze_AbstractBehaviour
{
    public bool Activate = true;
    public GameObject[] GameObjects;


    protected override void OnTrigger()
    {
        if (GameObjects.Length > 0)
        {
            for (int i = 0; i < GameObjects.Length; ++i)
                GameObjects[i].SetActive(Activate);

        }
    }
}
