using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

public class RotationCylinder : MonoBehaviour
{
    [SerializeField]
    int key;

    void Update() {
        switch (key) {
            case 0:
                if (Keyboard.current.numpad0Key.wasPressedThisFrame) {
                    this.transform.Rotate(0, 22.5f, 0);
                }
                break;
            case 1:
                if (Keyboard.current.numpad1Key.wasPressedThisFrame) {
                    this.transform.Rotate(0, 22.5f, 0);
                }
                break;
            case 2:
                if (Keyboard.current.numpad2Key.wasPressedThisFrame) {
                    this.transform.Rotate(0, 22.5f, 0);
                }
                break;
            case 3:
                if (Keyboard.current.numpad3Key.wasReleasedThisFrame) {
                    this.transform.Rotate(0, 22.5f, 0);
                }
                break;
        }
    }
}
