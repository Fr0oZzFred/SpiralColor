using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DeadZone : MonoBehaviour {
    [SerializeField] bool gizmos;
    [SerializeField] Color gizmosColor = Color.red;
    private void OnTriggerEnter(Collider other) {
        Controller c = other.GetComponent<Controller>();
        if (c) {
            LevelManager.Instance.Respawn(c);
        }
    }
    private void OnDrawGizmos() {
        if (!gizmos) return;
        if (GetComponents<BoxCollider>() == null) return;
        BoxCollider[] colliders = GetComponents<BoxCollider>();
        Gizmos.color = gizmosColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        foreach (var c in colliders) {
            Gizmos.DrawCube(c.center, c.size);
        }

    }
}
