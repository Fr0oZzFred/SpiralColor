using UnityEngine;
public delegate Controller GetController();
public class DynamicATH : MonoBehaviour {

    [SerializeField] GameObject ath;

    GetController controller;


    private void Start() {
        if (LevelManager.Instance) {
            controller = LevelManagerCurrentController;
        }

        if (HUBManager.Instance) {
            controller = HUBManagerCurrentController;
        }

        ath.SetActive(false);
    }

    void Update() {
        //Il est pas enabled car sinon l'ath n'était pas directement en face
        ath.transform.LookAt(controller().GetCam().position, Vector3.up);
    }

    public void DisplayATH(bool b) {
        ath.SetActive(b);
    }

    Controller LevelManagerCurrentController() {
        return LevelManager.Instance.CurrentController;
    }
    Controller HUBManagerCurrentController() {
        return HUBManager.Instance.CurrentController;
    }
}