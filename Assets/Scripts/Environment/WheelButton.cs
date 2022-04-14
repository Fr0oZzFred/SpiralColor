using System.Collections;
using UnityEngine;

public class WheelButton : MonoBehaviour
{
    [SerializeField]
    CrossPlayerController cross;
    [SerializeField]
    Vector3 rotationOnButton = new Vector3(0, 0, 0);
    [SerializeField]
    [Min(0.01f)]
    float jumpSpeed = 0.01f;
    [SerializeField]
    float rotationSpeed = 0.1f;
    [SerializeField]
    float rotationSpeedOnButton = 0.5f;
    [SerializeField]
    float height;
    [SerializeField]
    float offset, offsetStart, offsetEnd;
    [SerializeField]
    float rangeForJumping;

    [SerializeField]
    [Min(0.001f)] float resolutionGizmos = 0.01f;

    bool currentCoroutineIsRunning;

    int i = -1;

    Vector3 startpos;
    Quaternion startRot;

    private void Update() {
        if (LevelManager.Instance.CurrentController is CrossPlayerController)
            cross = (CrossPlayerController)LevelManager.Instance.CurrentController;
        else return;

        if (LevelManager.Instance.CurrentController.GetComponent<CrossPlayerController>()) {
            if (i == 0) LevelManager.Instance.CurrentController.RegisterInputs(false);
            Vector3 p = transform.position - cross.transform.position;
            if (p.magnitude < rangeForJumping && cross == LevelManager.Instance.CurrentController) {
                CheckForJump();
            }
        }
    }


    private void CheckForJump() {
        if (InputHandler.Controller.buttonSouth.wasPressedThisFrame) {
            cross.IsOnButton = true;
            LevelManager.Instance.CurrentController.RegisterInputs(false);
            if (!currentCoroutineIsRunning) {
                i = (i + 1) % 2;
                if (i == 0) {
                    startpos = cross.transform.position;
                    startRot = cross.transform.GetChild(1).rotation;
                    StartCoroutine(JumpTrajectory(startpos, transform.position));
                }
                else if (i == 1) {
                    StartCoroutine(JumpTrajectory(transform.position, startpos));
                }
            }
        }
    }

    public void RotateCross(float delta) {
        if (!currentCoroutineIsRunning && i == 0) {
            cross.TurnOnButton(delta * rotationSpeedOnButton);
        }
    }
    IEnumerator JumpTrajectory(Vector3 start, Vector3 end) {
        float interpolation = 0;
        while (interpolation < 1f) {
            currentCoroutineIsRunning = true;
            interpolation = Mathf.Clamp(interpolation, 0f, 1f);


            Quaternion r;
            r = Quaternion.Euler(rotationOnButton);
            if(i == 0)
                cross.transform.GetChild(1).rotation = Quaternion.Lerp(cross.transform.GetChild(1).rotation, r, interpolation * rotationSpeed);
            else if(i ==1)
                cross.transform.GetChild(1).rotation = Quaternion.Lerp(cross.transform.GetChild(1).rotation, startRot, interpolation * rotationSpeed);

            Vector3 res = start + (end - start) * interpolation;

            res.y = (Mathf.Sin(interpolation * Mathf.PI) + 1) / 2;
            res.y = res.y * height - height * 0.5f;
            res.y += offsetStart * (1 - interpolation);
            res.y += offsetEnd * interpolation;
            res.y += offset;
            cross.transform.position = res;
            interpolation += jumpSpeed * Time.deltaTime;
            yield return null;
        }
        if (i == 1) {
            LevelManager.Instance.CurrentController.RegisterInputs(true);
            cross.IsOnButton = false;
        }
        currentCoroutineIsRunning = false;
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeForJumping);
        if (resolutionGizmos <= 0) return;
        if (!cross) return;
        for (float i = 0; i < 1.1f; i += resolutionGizmos) {
            Vector3 pos = JumpTrajectory(cross.transform.position, transform.position, i);
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }
    Vector3 JumpTrajectory(Vector3 start, Vector3 end, float t) {
        t = Mathf.Clamp(t, 0f, 1f);
        Vector3 res = start + (end - start) * t;

        res.y = (Mathf.Sin(t * Mathf.PI) + 1) / 2;
        res.y = res.y * height - height * 0.5f;
        res.y += offsetStart * (1 - t);
        res.y += offsetEnd * t;
        res.y += offset;
        return res;
    }
}
