using UnityEngine;

public class TeleporterEndLevel : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>()) {
            LevelManager.Instance.TriggerLevelEnd();
        }
    }
}
