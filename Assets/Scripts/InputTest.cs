using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;

public class InputTest : MonoBehaviour
{
    private DualShock4GamepadHID controller;
    private Transform m_transform;
    public float sensibility;
    public Color pink;
    SwitchProControllerHID cont;
    
    private void Start() {
        try {
            this.controller = DS4.GetController();
        } catch {
            Debug.LogWarning(Gamepad.current + " is not supported");
        }
        m_transform = this.transform;
    }

    private void Update() {
        if(controller == null) {
            try {
                controller = DS4.GetController();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        } else {
            if (controller.startButton.isPressed) {
                m_transform.rotation = Quaternion.identity;
            }
            if (controller.triangleButton.isPressed) {
                GetComponent<MeshRenderer>().material.color = Color.green;
                controller.SetLightBarColor(Color.green);
            }

            if (controller.circleButton.isPressed) {
                GetComponent<MeshRenderer>().material.color = Color.red;
                controller.SetLightBarColor(Color.red);
            }

            if (controller.buttonSouth.isPressed) {
                GetComponent<MeshRenderer>().material.color = Color.blue;
                controller.SetLightBarColor(Color.blue);
            }

            if (controller.squareButton.isPressed) {
                GetComponent<MeshRenderer>().material.color = pink;
                controller.SetLightBarColor(pink);
            }
        }
        m_transform.rotation *= DS4.GetRotation(sensibility * Time.deltaTime);
    }
    private void OnApplicationQuit() {
        controller.SetLightBarColor(Color.black);
    }
}
