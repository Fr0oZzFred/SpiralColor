using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateauController : MonoBehaviour {

    public float sensibility;

    private void Update() {
        transform.rotation *= DS4.GetRotation(sensibility * Time.deltaTime);


        if (InputHandler.GetCurrentGamepad().startButton.isPressed) {
            transform.rotation = Quaternion.identity;
        }
    }
}
