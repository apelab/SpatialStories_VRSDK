namespace Gaze
{
    /// <summary>
    /// Grab states needed to track the grab state process:
    /// SEARCHING = Player has the trigger down and we are raycasting to seach an object.
    /// ATTRACTING = We have an object to take and we are attracting it to us.
    /// GRABBING = We are going to grab the object with tryatach.
    /// GRABBED = The object is on our hand.
    /// EMPTY = We don't have any objects and the user isn't pressing the button.
    /// BLOCKED = Used to diable the grab logic.
    /// </summary>
    public enum Gaze_GrabManagerState
    {
        SEARCHING,
        ATTRACTING,
        GRABBING,
        GRABBED,
        EMPTY,
        BLOCKED
    }
}
