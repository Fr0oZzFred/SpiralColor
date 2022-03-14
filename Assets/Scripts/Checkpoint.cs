using UnityEngine;
using System;
public class Checkpoint : MonoBehaviour, IComparable {
    [SerializeField]
    int progression;

    public int CompareTo(object obj) {
        if (obj == null) return 1;
        Checkpoint otherCheckpoint = obj as Checkpoint;
        if (otherCheckpoint != null) {
            return this.progression.CompareTo(otherCheckpoint.progression);
        } else {
            throw new ArgumentException("Object is not a Checkpoint");
        }
    }

    private void OnTriggerEnter(Collider other) {
        LevelManager.Instance.UpdateCPProgression(progression);
    }
}
