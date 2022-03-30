using System;
using System.Collections;
using UnityEngine;

public class Button : MonoBehaviour
{
    enum Shape {
        Cross,
        Square
    }
    [SerializeField]
    Shape shapeConcerned;
    [SerializeField]
    Transform jumper;
    [SerializeField]
    float height;
    [SerializeField]
    float offset, offsetStart, offsetEnd;
    [SerializeField]
    [Min(0.01f)]
    float speed = 0.01f, resolutionGizmos = 0.01f;

    [SerializeField]
    float rangeForJumping;

    private void Update() {
        Vector3 p = transform.position - jumper.position;
        if(p.magnitude < rangeForJumping) {
            CheckForJump();
        }
    }

    private void CheckForJump() {
        if (shapeConcerned == Shape.Cross) {
            if (InputHandler.Controller.buttonSouth.wasPressedThisFrame) {
                LevelManager.Instance.playerHandler.CurrentPlayer.RegisterInputs(false);
                StartCoroutine(JumpTrajectory(jumper.position, transform.position));
            }
        } else if (InputHandler.Controller.buttonWest.wasPressedThisFrame) {
            LevelManager.Instance.playerHandler.CurrentPlayer.RegisterInputs(false);
            StartCoroutine(JumpTrajectory(jumper.position, transform.position));
        }
    }

    IEnumerator JumpTrajectory(Vector3 start, Vector3 end) {
        float interpolation = 0;
        while (interpolation < 1f) {
            interpolation = Mathf.Clamp(interpolation, 0f, 1f);

            if(shapeConcerned == Shape.Cross) {
                Quaternion r;
                r = Quaternion.Euler(0, 0, 270);
                jumper.GetChild(0).rotation = Quaternion.Lerp(jumper.GetChild(0).rotation, r, interpolation * 0.1f);
            }

            Vector3 res = start + (end - start) * interpolation;

            res.y = (Mathf.Sin(interpolation * Mathf.PI) + 1) / 2;
            res.y = res.y * height - height * 0.5f;
            res.y += offsetStart * (1 - interpolation);
            res.y += offsetEnd * interpolation;
            res.y += offset;
            jumper.position = res;
            interpolation += speed * Time.deltaTime;
            yield return null;
        }
        LevelManager.Instance.playerHandler.CurrentPlayer.RegisterInputs(true);
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeForJumping);
        if (resolutionGizmos <= 0) return;
        if (!jumper) return;
        for (float i = 0; i < 1.1f; i += resolutionGizmos) {
            Vector3 pos = JumpTrajectory(jumper.position, transform.position, i);
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
