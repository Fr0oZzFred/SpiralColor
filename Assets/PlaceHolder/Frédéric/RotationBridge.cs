using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationBridge : MonoBehaviour
{
    [SerializeField]
    Transform rotStart, rotEnd;

    public float value;

    public float[] tabInput;

    bool reverse;
    int index = 0;
    private void Update() {
        if (Keyboard.current.tKey.isPressed) {
            value += 0.005f;
        }
        if (reverse) {
            if (InputHandler.GetLeftStickValues().x + InputHandler.GetRightStickValues().y < tabInput[index]) {
                index++;
                if (index % 10 == 0) reverse = !reverse;
                value += 0.005f;
            }

        }
        else if(InputHandler.GetLeftStickValues().x + InputHandler.GetRightStickValues().y > tabInput[index]) {
            index++;
            if (index % 10 == 0) reverse = !reverse;
            value += 0.005f;
        }
        index %= tabInput.Length;
        value = Mathf.Clamp(value, 0f, 1f);
        this.transform.rotation = Quaternion.Slerp(rotStart.rotation, rotEnd.rotation, value);
    }
}
