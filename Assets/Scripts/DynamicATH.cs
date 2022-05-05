using UnityEngine;
using Cinemachine;
public class DynamicATH : MonoBehaviour{
    CinemachineBrain c;
    [SerializeField] GameObject ath;
    [SerializeField] float rangeDetection = 2f;
    PlayerController player;
    private void Start() {
        if(LevelManager.Instance.CurrentController is PlayerController)
            player = LevelManager.Instance.CurrentController as PlayerController;
    }
    void Update(){
        Vector3 p = transform.position - player.transform.position;
        if (p.magnitude < rangeDetection && LevelManager.Instance.CurrentController is PlayerController) {
            ath.SetActive(true);
            ath.transform.LookAt(LevelManager.Instance.CurrentController.GetCam().position, Vector3.up);
        }
        else ath.SetActive(false);
    }
}