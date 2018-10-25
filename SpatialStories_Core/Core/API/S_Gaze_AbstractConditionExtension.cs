using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// A partial definition of the Gaze_Abstract condition used to
    /// being able to create conditions and dependencies by using the
    /// SpatialStories API.
    /// </summary>
    public abstract partial class Gaze_AbstractCondition : S_AbstractInteractionComponentData
    {
        // This constructor is needed in order to lazy initialize conditions
        public Gaze_AbstractCondition() { }
    }
}