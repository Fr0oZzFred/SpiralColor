using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.Controls;

public class InputTest : MonoBehaviour
{
    public static DualShock4GamepadHID controller;
    private Transform m_transform;
    public float sensibility;
    public Color pink;
    public static XInputController xboxController;
    
    private void Start() {
        rb = GetComponent<Rigidbody>();
        try {
            controller = DS4.GetController();
        } catch {
            try {
                xboxController = (XInputController)Gamepad.current;
            } catch {
                Debug.LogWarning(Gamepad.current.name + " is not supported");
            }

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
                try {
                    controller.SetLightBarColor(Color.green);
                } catch { }
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
            m_transform.rotation *= DS4.GetRotation(sensibility * Time.deltaTime);
        }



        if(controller != null && xboxController == null) {
            try {
                xboxController = (XInputController)Gamepad.current;
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        } else {

            //if (xboxController.startButton.isPressed) {
            //    m_transform.rotation = Quaternion.identity;
            //}

            if (xboxController.triangleButton.isPressed) {
                GetComponent<MeshRenderer>().material.color = Color.green;
            }

            if (xboxController.circleButton.isPressed) {
                GetComponent<MeshRenderer>().material.color = Color.red;
            }

            if (xboxController.buttonSouth.isPressed) {
                GetComponent<MeshRenderer>().material.color = Color.blue;
            }

            if (xboxController.squareButton.isPressed) {
                GetComponent<MeshRenderer>().material.color = pink;
            }
            Quaternion newRot = Quaternion.Euler(
                (m_transform.rotation.x + 1)* xboxController.leftStick.y.ReadValue() * sensibility,
                (m_transform.rotation.y + 1)* -xboxController.leftStick.x.ReadValue() * sensibility,
                m_transform.rotation.z
                );
            m_transform.rotation = newRot;
        }

        #region CameraFixe

        moveDirection = cam.forward * controller.leftStick.y.ReadValue();
        moveDirection += cam.right * controller.leftStick.x.ReadValue();
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection = moveDirection * movementSpeed;

        Vector3 movementVelocity = moveDirection;
        rb.velocity = movementVelocity;

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cam.forward * controller.leftStick.y.ReadValue();
        targetDirection += cam.right * controller.leftStick.x.ReadValue();
        targetDirection.Normalize();
        targetDirection.y = 0;
        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection); 
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
        #endregion


    }
    Rigidbody rb;
    Vector3 moveDirection;
    public Transform cam;
    public float movementSpeed;
    public float rotationSpeed;

    public CameraManager cameraManager;

    private void LateUpdate() {
        cameraManager.FollowTarget();
        cameraManager.RotateCamera();
        cameraManager.HandleCameraCollisions();
    }

    private void OnApplicationQuit() {
        if(controller != null)
        controller.SetLightBarColor(Color.black);
    }
}
