using UnityEngine;

public class RotationObjectByWheelButton : MonoBehaviour
{
    [SerializeField]
    WheelButton wheelButton;

    [SerializeField]
    Vector3 rotStart, rotEnd;

    [SerializeField] 
    float limitAngle, powerRotation;

    float value, oldAngle;

    private void Awake() {
        enabled = false;
    }
    private void Update() {
        if (!LevelManager.Instance.CurrentController.GetComponent<CrossPlayerController>()) return;
        Vector2 input = InputHandler.GetLeftStickValues();
        float angle =  Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        float delta = angle - oldAngle;
        oldAngle = angle;
        if (delta == 0f) return;
        if (delta > limitAngle || delta < -limitAngle) return;
        if (delta > 0) {
            value += powerRotation;
        } else {

            value += -powerRotation;
        }
        value = Mathf.Clamp(value, 0f, 1f);
        if(value != 0f && value != 1f) {
            wheelButton.RotateCross(delta);
        }
        this.transform.rotation = Quaternion.Euler(Vector3.Lerp(rotStart, rotEnd, value));
    }
}
