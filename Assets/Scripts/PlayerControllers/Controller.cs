using UnityEngine;

public abstract class Controller : MonoBehaviour {
    public abstract void RegisterInputs(bool b);
    public abstract void PreventSnapToGround();
    public abstract void SetControllerLED();
    public abstract void Respawn(Vector3 v3);
}
