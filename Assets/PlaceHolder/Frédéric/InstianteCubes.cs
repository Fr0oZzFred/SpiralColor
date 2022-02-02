using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstianteCubes : MonoBehaviour
{
    public GameObject cubePrefab;
    GameObject[] sampleCube = new GameObject[512];
    public float scale;
    private void Start() {
        for(int i = 0; i < sampleCube.Length; i++) {
            GameObject temp = Instantiate(cubePrefab);
            temp.transform.position = this.transform.position;
            temp.transform.parent = this.transform;
            temp.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);
            temp.transform.position = Vector3.forward * 100;
            sampleCube[i] = temp;
        }
    }

    private void Update() {
        for(int i = 0; i < sampleCube.Length; i++){
            if(sampleCube != null) {
                sampleCube[i].transform.localScale = new Vector3(1, AudioPeer.samples[i] * scale + 1, 1);
            }
        }
    }
}
