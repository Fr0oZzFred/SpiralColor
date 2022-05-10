using UnityEngine;
public class DynamicATH : MonoBehaviour {

    [SerializeField] GameObject ath;
    PlayerController player;

    private void Start() {
        if (LevelManager.Instance) {
            if (LevelManager.Instance.CurrentController is PlayerController)
                player = LevelManager.Instance.CurrentController as PlayerController;
        }

        if (HUBManager.Instance) {
            if (HUBManager.Instance.CurrentController is PlayerController)
                player = HUBManager.Instance.CurrentController as PlayerController;
        }

        ath.SetActive(false);
    }

    void Update() {
        //Il est pas enabled car sinon l'ath n'était pas directement en face
        ath.transform.LookAt(player.GetCam().position, Vector3.up);
    }

    public void DisplayATH(bool b) {
        ath.SetActive(b);
    }
}