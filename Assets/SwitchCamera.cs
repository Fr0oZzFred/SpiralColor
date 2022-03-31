using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour{
    [SerializeField] GameObject camera3D;
    public Vector3 active, notActive;
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>()) {
            transform.position = camera3D.activeSelf ? active : notActive;
            other.GetComponent<Rigidbody>().constraints |= ~RigidbodyConstraints.FreezePositionX;
            camera3D.SetActive(!camera3D.activeSelf);
        }
    }
}
