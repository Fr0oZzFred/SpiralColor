using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerInspector : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var levelManager = (LevelManager)target;
        if (levelManager.HasMissingCheckPoints()) {
            EditorGUILayout.HelpBox("Missing checkpoints!", MessageType.Error);
            if(GUILayout.Button("Remove Missing Checkpoints.")) {
                levelManager.RemoveMissingCheckpoints();
            }
        }
        int item1 = levelManager.HasCheckpointsWithSameProgression().Item1;
        int item2 = levelManager.HasCheckpointsWithSameProgression().Item2;
        if (item1 == item2 -1) {
            EditorGUILayout.HelpBox($"Index {item1} and {item2} have the same Progression !", MessageType.Error);
        }
    }
}