using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixCube : MonoBehaviour
{
    [SerializeField] List<GameObject> cube1 = new List<GameObject>();
    [SerializeField] List<GameObject> cube2 = new List<GameObject>();
    [SerializeField] List<GameObject> cube3 = new List<GameObject>();
    [SerializeField] List<Vector3> pos1 = new List<Vector3>();
    [SerializeField] List<Vector3> pos2 = new List<Vector3>();
    [SerializeField] List<Vector3> pos3 = new List<Vector3>();
    private void Start() {
        Shuffle(cube1);
        Shuffle(cube2);
        Shuffle(cube3);
        for (int i = 0; i < cube1.Count; i++) cube1[i].transform.position = pos1[i];
        for (int i = 0; i < cube2.Count; i++) cube2[i].transform.position = pos2[i];
        for (int i = 0; i < cube3.Count; i++) cube3[i].transform.position = pos3[i];
    }
    void Shuffle(List<GameObject> list) {
        for(int i = 0; i < list.Count; i++) {
            int rnd = Random.Range(0, list.Count);
            GameObject temp = list[rnd];
            list[rnd] = list[i];
            list[i] = temp;
        }
    }
}
