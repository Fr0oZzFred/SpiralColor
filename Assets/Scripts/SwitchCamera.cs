using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour {
    [SerializeField] GameObject camera3D, camera2D;
    [SerializeField] Vector3 pos2D, pos3D;
    [SerializeField] RigidbodyConstraints constraint2D;
    private void OnTriggerEnter(Collider other) {
        Controller controller = other.GetComponent<Controller>();
        if (controller) {
            camera3D.SetActive(!camera3D.activeSelf);
            transform.position = camera3D.activeSelf ? pos3D : pos2D;
            if (camera3D.activeSelf) {
                controller.SetInputSpace(camera3D.transform);
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            } else {
                controller.SetInputSpace(camera2D.transform);
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | constraint2D;
            }
        }
    }
}