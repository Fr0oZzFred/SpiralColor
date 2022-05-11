using System.Collections;
using UnityEngine;

public class SquareButton : MonoBehaviour
{
    [SerializeField]
    SquarePlayerController square;
    [SerializeField]
    Vector3 rotationOnButton = new Vector3(0, 0, 0);
    [SerializeField]
    [Min(0.01f)]
    float jumpSpeed = 0.01f;
    [SerializeField]
    float rotationSpeed = 0.1f;
    [SerializeField]
    float height;
    [SerializeField]
    float offset, offsetStart, offsetEnd;
    [SerializeField]
    float rangeForJumping;

    [SerializeField]
    [Min(0.001f)] float resolutionGizmos = 0.01f;

    [SerializeField]
    DynamicATH ath;

    bool currentCoroutineIsRunning;

    private void Update() {
        if (LevelManager.Instance.CurrentController is SquarePlayerController)
            square = (SquarePlayerController)LevelManager.Instance.CurrentController;
        else return;
        Vector3 p = transform.position - square.transform.position;
        if (p.magnitude < rangeForJumping && square == LevelManager.Instance.CurrentController) {
            CheckForJump();
            ath.DisplayATH(!square.IsOnButton);
        } else {
            ath.DisplayATH(false);
        }
    }


    private void CheckForJump() {
        if (InputHandler.Controller.buttonWest.wasPressedThisFrame) {
            square.IsOnButton = true;
            LevelManager.Instance.CurrentController.RegisterInputs(false);
            if (!currentCoroutineIsRunning)
                StartCoroutine(JumpTrajectory(square.transform.position, transform.position));
        }
    }

    IEnumerator JumpTrajectory(Vector3 start, Vector3 end) {
        float interpolation = 0;
        while (interpolation < 1f) {
            currentCoroutineIsRunning = true;
            interpolation = Mathf.Clamp(interpolation, 0f, 1f);


            Quaternion r;
            r = Quaternion.Euler(rotationOnButton);
            square.transform.GetChild(0).rotation = Quaternion.Lerp(square.transform.GetChild(0).rotation, r, interpolation * rotationSpeed);

            Vector3 res = start + (end - start) * interpolation;

            res.y = (Mathf.Sin(interpolation * Mathf.PI) + 1) / 2;
            res.y = res.y * height - height * 0.5f;
            res.y += offsetStart * (1 - interpolation);
            res.y += offsetEnd * interpolation;
            res.y += start.y;
            res.y += offset;
            square.transform.position = res;
            interpolation += jumpSpeed * Time.deltaTime;
            yield return null;
        }
        LevelManager.Instance.CurrentController.RegisterInputs(true);
        currentCoroutineIsRunning = square.IsOnButton = false;
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeForJumping);
        if (resolutionGizmos <= 0) return;
        if (!square) return;
        for (float i = 0; i < 1.1f; i += resolutionGizmos) {
            Vector3 pos = JumpTrajectory(square.transform.position, transform.position, i);
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
        res.y += start.y;
        return res;
    }
}
