using UnityEngine;
public class ColliderCam : MonoBehaviour{
    public Vector3 pos2D, pos3D;
    private void OnTriggerEnter(Collider other) {
        Controller controller = other.GetComponent<Controller>();
        if (controller) {
            transform.position = SwitchCamera.Instance.newPos(pos2D, pos3D, controller);
        }
    }
}
