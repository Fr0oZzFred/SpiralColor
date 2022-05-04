using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicATH : MonoBehaviour{
    [SerializeField] GameObject ath;
    [SerializeField] float rangeDetection = 2f;
    PlayerController player;
    private void Start() {
        if(LevelManager.Instance.CurrentController is PlayerController)
            player = LevelManager.Instance.CurrentController as PlayerController;
    }
    void Update(){
        Vector3 p = transform.position - player.transform.position;
        if (p.magnitude < rangeDetection && LevelManager.Instance.CurrentController is PlayerController) ath.SetActive(true);
        else ath.SetActive(false);
        ath.transform.LookAt(-Camera.main.transform.position, Vector3.up);
    }
}
