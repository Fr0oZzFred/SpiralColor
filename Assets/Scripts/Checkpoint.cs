using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.VFX;
public enum CamRotation {
    forward,
    backward,
    right,
    left
}
public class Checkpoint : MonoBehaviour, IComparable {

    const string baseName = "Checkpoint n°";

    [SerializeField] List<Quaternion> camRotations;

    [SerializeField] CamRotation camRotationEnum;

    [SerializeField] VisualEffect visualEffect;
    [SerializeField] float AccelerateValue;
    [SerializeField] float cylinderPowerValue;
    [SerializeField] float time;

    [SerializeField]
    int progression;
    public int Progression {
        get {
            return progression;
        }
    }
    void Update() {
        if(visualEffect.GetFloat("Global Speed") > 1) visualEffect.SetFloat("Global Speed", Mathf.Lerp(visualEffect.GetFloat("Global Speed"), 1, time));
        if(visualEffect.GetFloat("Cylinder Power Multiplier") > 1) visualEffect.SetFloat("Cylinder Power Multiplier", Mathf.Lerp(visualEffect.GetFloat("Cylinder Power Multiplier"), 1, time));
    }

    public Quaternion CamRotation {
        get {
            return camRotations[(int)camRotationEnum];
        }
    }

    void OnValidate() {
        this.gameObject.name = baseName + progression;
    }
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
        if (LevelManager.Instance.PlayerProgressionIsGreaterThan(progression)) return;
        LevelManager.Instance.UpdateCPProgression(progression);
        visualEffect.SetFloat("Global Speed", AccelerateValue);
        visualEffect.SetFloat("Cylinder Power Multiplier", cylinderPowerValue);
        if (visualEffect.gameObject.activeInHierarchy) {
            SoundsManager.Instance.Play("CheckpointSound");
        }
    }

    public void SetProgression(int prog) {
        progression = prog;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        var c = GetComponent<Collider>();
        var b = c as BoxCollider;
        if(b != null) {
            Gizmos.matrix = Matrix4x4.TRS(
                transform.position, transform.rotation, transform.lossyScale
            );
            Gizmos.DrawCube(b.center, b.size);
            return;
        }
    }
}
