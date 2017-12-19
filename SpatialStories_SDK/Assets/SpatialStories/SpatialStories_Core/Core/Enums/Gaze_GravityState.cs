namespace Gaze
{
    public enum Gaze_GravityState
    {
        LOCKED,                 // The object state wont change.
        ACTIVE_KINEMATIC,       // The object has gravity and is kinematic
        UNACTIVE_KINEMATIC,     // The oject has not gravity and is kinematic
        ACTIVE_NOT_KINEMATIC,   // The object has gravity and is not kinematic
        UNACTIVE_NOT_KINEMATIC, // The oject has not gravity and is not kinematic
    }
}