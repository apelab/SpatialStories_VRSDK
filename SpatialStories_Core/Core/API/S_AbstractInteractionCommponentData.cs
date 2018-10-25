using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the data necessary to create conditions, actions or dependencies during the
/// creationg process of an Interactive Object using the API.
/// TODO: This class will dissappear and replaced by S_AbstractInteractionDataMonoBehaivour when all
/// the actions and condions will be monobehaivours
/// </summary>
public abstract class S_AbstractInteractionComponentData {

    protected List<object> creationData = new List<object>();
    public abstract void SetupUsingApi(GameObject _interaction);

    /// <summary>
    /// Needed in order to create conditions and dependencies that will be lazy configured.
    /// </summary>
    public S_AbstractInteractionComponentData() { }

    /// <summary>
    /// Adds some creation data in order to setup the
    /// condition / dependency in the moment of its 
    /// creation
    /// </summary>
    /// <param name="_data">Data required to setup the condition</param>
    public void AddCreationData(params object[] _data)
    {
        creationData.AddRange(_data);
    }

    /// <summary>
    /// Modifies the creation data stored in the specified index
    /// </summary>
    /// <param name="_data"> The new creation data </param>
    /// <param name="index"> Index to replace </param>
    public void ModifyCreationDataByIndex(object _data, int _index)
    {
        if (_index < 0 || _index >= creationData.Count)
        {
            Debug.LogError(string.Format("SpatialStoriesAPI > The specified index {0} is not correct.", _index));
        }
        else
        {
            creationData[_index] = _data;
        }
    }

    /// <summary>
    /// Clears all the creation data of a condition / dependency
    /// </summary>
    public void ClearCreationData()
    {
        creationData.Clear();
    }
}
