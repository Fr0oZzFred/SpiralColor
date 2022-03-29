using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Transform jumper;
    public float height;
    public float offset, offsetStart, offsetEnd;
    [Min(0.01f)]
    public float speed = 0.01f, numberOfGizmos = 0.01f;

    public float distance;

    private void Update() {
        Vector3 p = transform.position - jumper.position;
        if(p.magnitude < distance) {
            Debug.Log("you're allowed to jump");
            if (InputHandler.Controller.leftTrigger.wasPressedThisFrame) {
                StartCoroutine(JumpTrajectory(jumper.position, transform.position));
            }
        }
    }
    IEnumerator JumpTrajectory(Vector3 start, Vector3 end) {
        float interpolation = 0;
        while (interpolation < 1f) {
            interpolation = Mathf.Clamp(interpolation, 0f, 1f);
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
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Vector3 startPos = jumper.position;
        startPos.y += offsetStart;
        if (numberOfGizmos <= 0) return;
        for (float i = 0; i < 1.1f; i += numberOfGizmos) {
            Vector3 pos = JumpTrajectory(startPos, transform.position, i);
            Gizmos.DrawSphere(pos, 0.1f);
        }
        Gizmos.DrawWireSphere(transform.position, distance);
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
