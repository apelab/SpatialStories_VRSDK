using SpatialStories;

namespace Gaze
{
    /// <summary>
    /// A partial definition of the Gaze_Abstract behaivour used to
    /// being able to create actions by using the
    /// SpatialStories API.
    /// </summary>
    public abstract partial class Gaze_AbstractBehaviour : S_AbstractInteractionDataMonoBehaviour
    {
        // This constructor is needed in order to lazy initialize actions
        public Gaze_AbstractBehaviour() { }
    }

}
