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
                Debug.Log("Passage 3D");
                controller.SetInputSpace(camera3D.transform);
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            } else {
                Debug.Log("Passage 2D");
                controller.SetInputSpace(camera2D.transform);
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | constraint2D;
            }
        }
    }
}