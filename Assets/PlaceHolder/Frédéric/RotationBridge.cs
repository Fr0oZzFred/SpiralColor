using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationBridge : MonoBehaviour
{
    [SerializeField]
    Transform rotStart, rotEnd;

    public float value;

    private void Update() {
        if (Keyboard.current.tKey.isPressed) {
            value += 0.005f;
        } 
        value = Mathf.Clamp(value, 0f, 1f);
        this.transform.rotation = Quaternion.Slerp(rotStart.rotation, rotEnd.rotation, value);
    }
}
