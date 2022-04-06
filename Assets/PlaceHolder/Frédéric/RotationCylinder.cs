using UnityEngine;

public class RotationCylinder : MonoBehaviour {
    [SerializeField]
    WheelButton wheelButton;

    [SerializeField]
    float limitAngle, powerRotation;

    float oldAngle;

    private void Awake() {
        enabled = false;
    }

    void Update() {
        if (!LevelManager.Instance.CurrentController.GetComponent<CrossPlayerController>()) return;
        Vector2 input = InputHandler.GetLeftStickValues();
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        float delta = angle - oldAngle;
        oldAngle = angle;
        if (delta == 0f) return;
        if (delta > limitAngle || delta < -limitAngle) return;
        if (delta > 0) {
            transform.Rotate(0, powerRotation, 0);
        } else {
            transform.Rotate(0, -powerRotation, 0);
        }
        wheelButton.RotateCross(delta);
    }
}
