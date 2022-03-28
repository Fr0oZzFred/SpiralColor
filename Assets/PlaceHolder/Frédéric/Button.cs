using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Transform transformS;
    public Transform jumper;
    public float height;
    public float offset;
    [Min(0.01f)]
    public float speed = 0.01f;

    public float distance;

    float interpolation = 0;
    private void Update() {
        Vector3 p = transform.position - jumper.position;
        if(p.magnitude < distance) {
            Debug.Log("you're allowed to jump");
        }
            
                /*
        interpolation += speed * Time.deltaTime;
        Vector3 pos = JumpTrajectory(transformS.position, transform.position, interpolation);
        jumper.position = pos;*/
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (float i = 0; i < 1.1f; i += speed) {
            Vector3 pos = JumpTrajectory(transformS.position, transform.position, i);
            Gizmos.DrawSphere(pos, 0.1f);
        }
        Gizmos.DrawWireSphere(transform.position, distance);
    }

    Vector3 JumpTrajectory(Vector3 start, Vector3 end, float t) {
        t = Mathf.Clamp(t, 0f, 1f);
        Vector3 res = start + (end - start) * t;

        res.y = (Mathf.Sin(t * Mathf.PI) + 1) /2;
        res.y = res.y * height - height * 0.5f;
        res.y += offset;
        return res;
    }
}
