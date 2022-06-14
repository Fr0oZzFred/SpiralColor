using UnityEngine;
public class SwitchCamera : MonoBehaviour {
    public static SwitchCamera Instance { get; private set; }
    [SerializeField] GameObject camera2D;
    [SerializeField] RigidbodyConstraints constraint2D;
    GameObject camera3D;
    private void Awake() { Instance = this; }
    public Vector3 newPos(Vector3 pos2D, Vector3 pos3D, Controller controller) {
        if (LevelManager.Instance.CurrentController.GetCam().gameObject != camera2D) camera3D = LevelManager.Instance.CurrentController.GetCam().gameObject;
        camera3D.SetActive(!camera3D.activeSelf);
        camera2D.SetActive(!camera3D.activeSelf);
        if (camera3D.activeSelf) {
            controller.SetInputSpace(camera3D.transform);
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        } else {
            controller.SetInputSpace(camera2D.transform);
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | constraint2D;
        }
        return camera3D.activeSelf ? pos3D : pos2D;
    }
}