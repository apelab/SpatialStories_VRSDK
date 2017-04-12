namespace Gaze
{
    public enum Gaze_GravityRequestType
    {
        LOCK,                   // Ask the object to not listen more gravity requests until unlock
        UNLOCK,                 // Ask the object to listen again more gravity requests
        ACTIVATE,               // Ask the object to activate gravity
        DEACTIVATE,             // Ask the object to deactivate gravity
        ACTIVATE_AND_DETACH,    // Set the graviity to true and make the object not kinematic
        DEACTIVATE_AND_ATTACH,  // Set the gravity to false and make the object kinematic
    }
}