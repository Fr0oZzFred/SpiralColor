using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public Transform targetTransform;
    public Transform cameraPivot;
    public Transform camTransform;
    public LayerMask collisionLayers;
    float defaultPos;
    Vector3 cameraFollowVelocity = Vector3.zero;
    Vector3 cameraVectorPos;

    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 2;
    public float cameraCollisionReactionSpeed = 2;
    public float followSpeed = 0.2f;
    public float lookSpeed = 1f;
    public float pivotSpeed = 2f;

    public float lookAngle;

    public float pivotAngle;
    public float minPivotAngle;
    public float maxPivotAngle;
    private void Awake() {
        defaultPos = camTransform.localPosition.z;
    }

    private void Update() {
        if (InputTest.controller.shareButton.isPressed) {
            defaultPos = camTransform.localPosition.z;
        }
    }

    public void FollowTarget() {
        Vector3 targetPos = Vector3.SmoothDamp
            (transform.position, targetTransform.position,ref cameraFollowVelocity, followSpeed);
        transform.position = targetPos;
    }

    public void RotateCamera() {
        Vector3 rotation;
        Quaternion rot;
        if(InputTest.controller != null) {
            lookAngle += InputTest.controller.rightStick.x.ReadValue() * lookSpeed;
            pivotAngle += InputTest.controller.rightStick.y.ReadValue() * pivotSpeed;
        } else {
            lookAngle += InputTest.xboxController.rightStick.x.ReadValue() * lookSpeed;
            pivotAngle += InputTest.xboxController.rightStick.y.ReadValue() * pivotSpeed;
        }
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle,maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        rot = Quaternion.Euler(rotation);
        transform.rotation = rot;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        rot = Quaternion.Euler(rotation);
        cameraPivot.localRotation = rot;
    }

    public void HandleCameraCollisions() {
        float targetPos = defaultPos;
        RaycastHit hit;
        Vector3 direction = camTransform.position - cameraPivot.position;
        direction.Normalize();
        if(Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPos), collisionLayers)) {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPos = targetPos + (distance - cameraCollisionOffset);

        }
        if(Mathf.Abs(targetPos) < minimumCollisionOffset) {
            targetPos = targetPos - minimumCollisionOffset;
        }
        cameraVectorPos.z = Mathf.Lerp(camTransform.localPosition.z, targetPos, cameraCollisionReactionSpeed * Time.deltaTime);
        camTransform.localPosition = cameraVectorPos;

        Vector3 pos = camTransform.localPosition;
        pos.z = Mathf.Clamp(camTransform.localPosition.z, Mathf.NegativeInfinity, -2f);
        camTransform.localPosition = pos;
    }
}
