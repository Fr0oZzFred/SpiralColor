using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    private Gamepad controller = null;

    void Start() {
        this.controller = InputHandler.Controller;
    }

    void Update() {
        this.controller = InputHandler.Controller;
        if (controller == null) {
            try {
                this.controller = InputHandler.Controller;
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        } else {
            Debug.Log("Connected");
        }
    }
}
