using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerInspector : Editor {

    const string baseName = "Checkpoint n°";
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var levelManager = (LevelManager)target;
        if (levelManager.HasMissingCheckPoints()) {
            EditorGUILayout.HelpBox("Missing checkpoints!", MessageType.Error);
            if (GUILayout.Button("Remove Missing Checkpoints.")) {
                levelManager.RemoveMissingCheckpoints();
            }
        }
        int item1 = levelManager.HasCheckpointsWithSameProgression().Item1;
        int item2 = levelManager.HasCheckpointsWithSameProgression().Item2;
        if (item1 == item2 - 1) {
            EditorGUILayout.HelpBox($"Index {item1} and {item2} have the same Progression !", MessageType.Error);
        }
        if (GUILayout.Button("Create new Checkpoint")) {
            GameObject newCheckpoint = new GameObject();
            Checkpoint prefab = levelManager.GetCheckpointPrefab();

            newCheckpoint.transform.localPosition = Vector3.zero;
            newCheckpoint.transform.localRotation = Quaternion.identity;
            newCheckpoint.transform.localScale = Vector3.one;

            newCheckpoint.AddComponent<BoxCollider>();
            newCheckpoint.GetComponent<BoxCollider>().isTrigger = prefab.GetComponent<BoxCollider>().isTrigger;

            newCheckpoint.AddComponent<Checkpoint>();
            newCheckpoint.GetComponent<Checkpoint>().SetProgression(
                levelManager.GetLastCheckpoint ? levelManager.GetLastCheckpoint.Progression + 1 : 0);


            GameObject VFX = new GameObject();
            VFX.transform.SetParent(newCheckpoint.transform);

            VFX.transform.localPosition = prefab.transform.GetChild(0).localPosition;
            VFX.transform.localRotation = prefab.transform.GetChild(0).localRotation;
            VFX.transform.localScale = prefab.transform.GetChild(0).localScale;

            VFX.AddComponent<VisualEffect>();
            VFX.GetComponent<VisualEffect>().visualEffectAsset = prefab.transform.GetChild(0).GetComponent<VisualEffect>().visualEffectAsset;

            newCheckpoint.name = baseName + newCheckpoint.GetComponent<Checkpoint>().Progression;
            VFX.name = prefab.transform.GetChild(0).name;
            levelManager.AddCheckpoint(newCheckpoint.GetComponent<Checkpoint>());
        }
    }
}