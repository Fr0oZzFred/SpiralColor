using UnityEngine;

public class RotateObject : MonoBehaviour {
    [SerializeField]
    Vector3 rotation;

    private void Update() {
        this.transform.Rotate(rotation * Time.deltaTime);
    }
}
