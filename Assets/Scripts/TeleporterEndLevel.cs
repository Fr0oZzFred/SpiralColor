using UnityEngine;

public class TeleporterEndLevel : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>()) {
            PlayTestData.Instance.Send();
            LevelManager.Instance.TriggerLevelEnd();
        }
    }
}
