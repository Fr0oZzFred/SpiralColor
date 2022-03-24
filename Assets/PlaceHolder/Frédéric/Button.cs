using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Transform transformS;
    public float height;

    public bool smooth;
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (float i = 0; i < 1.1f; i += 0.01f) {
            Vector3 pos = JumpTrajectory(transformS.position, transform.position, height, i);
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }

    Vector3 JumpTrajectory(Vector3 start, Vector3 end, float y, float t) {
        t = Mathf.Clamp(t, 0f, 1f);
        Vector3 res = start + (end - start) * t;

        res.y = (Mathf.Sin(t * Mathf.PI) + 1) /2;
        res.y = res .y * height - height * 0.5f;
        return res;
    }
}
