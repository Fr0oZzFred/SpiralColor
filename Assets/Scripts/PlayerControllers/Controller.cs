using UnityEngine;

public abstract class Controller : MonoBehaviour {
    public abstract void RegisterInputs(bool b);
    public abstract void PreventSnapToGround();
    public abstract void SetControllerLED();
    public abstract void Respawn(Vector3 v3);
    public abstract void SetInputSpace(Transform transform);
    public abstract int GetClosestAllowedCheckpoint(int actualProgression);
    public abstract string GetHelpBoxMessage();
    public abstract Quaternion GetCamRotation();
    public abstract void SetCamRotation(Quaternion q);
    public abstract Transform GetCam();
}
