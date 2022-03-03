using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    #region Fields
    public bool isUsingGyro;
    public float sensibility;
    public GameObject ui;

    public CameraManager cameraManager;

    Rigidbody rb;
    Transform cam;

    [Header("Color")]
    public ColorController[] colorControllerEditor;
    Dictionary<string , Color> colorController;

    [Header("Movement")]
    public float movementSpeed;
    [Header("Rotation")]
    public float rotationSpeed;
    [Header("Jump")]
    public float fallingVelocity;
    public float jumpForce;
    float inAirTimer = 0;
    public float jumpTimerBase = 1;
    public float jumpTimer;
    [Header("Ground")]
    public bool grounded;
    public float rayCastHeightOffSet = 0.5f;
    public LayerMask groundLayer;
    #endregion

    private void Start() {
        cam = cameraManager.camTransform;
        rb = GetComponent<Rigidbody>();
        ResetJumpTimer();
        colorController = new Dictionary<string, Color>();
        foreach(ColorController c in colorControllerEditor) {
            colorController.Add(c.button, c.color);
        }
    }

    private void Update() {
        if (isUsingGyro) transform.rotation *= DS4.GetGyroRotation(sensibility * Time.deltaTime);
        HandleButton();
    }

    private void FixedUpdate() {
        Movement();
        Ground();
        Jump();
    }

    private void LateUpdate() {
        cameraManager.FollowTarget();
        cameraManager.RotateCamera();
        cameraManager.HandleCameraCollisions();
    }

    void Movement() {
        Vector3 moveDirection;

        Vector2 leftStickValues = InputHandler.GetLeftStickValues();

        moveDirection = cam.forward * leftStickValues.y;
        moveDirection += cam.right * leftStickValues.x;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection = moveDirection * movementSpeed;

        Vector3 movementVelocity = moveDirection;
        rb.velocity = movementVelocity;

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cam.forward * leftStickValues.y;
        targetDirection += cam.right * leftStickValues.x;
        targetDirection.Normalize();
        targetDirection.y = 0;


        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;

    }
    void Ground() {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;
        if (!grounded && jumpTimer < 0) {
            inAirTimer += Time.deltaTime;
            rb.AddForce(Vector3.down * fallingVelocity * inAirTimer);
        }
        Debug.DrawRay(rayCastOrigin, Vector3.down, Color.red, 0.2f);
        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, groundLayer)) {
            inAirTimer = 0;
            grounded = true;
        } else {
            grounded = false;
        }
    }
    void Jump() {
        jumpTimer -= Time.deltaTime;
        if (InputHandler.GetCurrentGamepad().rightShoulder.isPressed && grounded && jumpTimer < 0) {
            ResetJumpTimer();
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
    /* Tests
        //Essaie de slop limit qui a enfaite fait de ricoché 
        
         à ajouter sur targetDirection & moveDirection

         if (!InputHandler.GetCurrentGamepad().leftTrigger.isPressed) {
             targetDirection.y = 0;
         } else {
             targetDirection.y = transform.rotation.y;
         } 
          
         

        RaycastHit hit2;
        rayCastOrigin.y = rayCastOrigin.y - 2 *rayCastHeightOffSet;
        if (InputHandler.GetCurrentGamepad().leftTrigger.isPressed) {
            if (Physics.Raycast(rayCastOrigin, transform.forward, out hit2, 2, groundLayer)) {
                //Debug.Log(hit2.normal);
                float res = Vector3.Dot(hit2.normal, -transform.up);
                Debug.Log(res);
                Debug.DrawRay(rayCastOrigin, transform.forward, Color.green,2);
                Quaternion rot = Quaternion.Euler(
                    res * 100,
                    transform.rotation.y,
                    transform.rotation.z);
                transform.rotation = rot;
            } else {
                Debug.DrawRay(rayCastOrigin, transform.forward, Color.red,2);
            }

        }

        //Essaie de rot coordonnées avec le sol
        if (Physics.Raycast(rayCastOrigin, -transform.up, out hit2, 2, groundLayer)){
            //Debug.Log(Vector3.Dot(hit2.normal, -transform.up));
            Debug.Log(hit2.normal);
            Quaternion rot = transform.rotation;
            rot.x += hit2.normal.x;
            transform.rotation = rot;
        }*/
    void HandleButton() {
        var c = InputHandler.GetCurrentGamepad();
        if (c.startButton.isPressed) {
            ui.SetActive(!ui.activeSelf);
        }

        if (c.triangleButton.isPressed) {
            GetComponent<MeshRenderer>().material.color = colorController["triangleButton"];
            InputHandler.SetControllerColor(colorController["triangleButton"]);
        }

        if (c.circleButton.isPressed) {
            GetComponent<MeshRenderer>().material.color = colorController["circleButton"];
            InputHandler.SetControllerColor(colorController["circleButton"]);
        }

        if (c.crossButton.isPressed) {
            GetComponent<MeshRenderer>().material.color = colorController["crossButton"];
            InputHandler.SetControllerColor(colorController["crossButton"]);
        }

        if (c.squareButton.isPressed) {
            GetComponent<MeshRenderer>().material.color = colorController["squareButton"];
            InputHandler.SetControllerColor(colorController["squareButton"]);
        }
    }
    void ResetJumpTimer() {
        jumpTimer = jumpTimerBase;
    }
    private void OnApplicationQuit() {
        InputHandler.SetControllerColor(Color.black);
    }
}
