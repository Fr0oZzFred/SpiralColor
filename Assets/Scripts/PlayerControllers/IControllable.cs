using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable {
    public void IsPlaying(bool b);
    public void PreventSnapToGround();
    public void SetControllerLED();
}
