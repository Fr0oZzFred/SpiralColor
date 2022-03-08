using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public List<BaseController> listGO;
    public int startingIndex = 0;
    int index = 0;

    public OrbitCamera cam;
    private void Start() {
        index = startingIndex;
        for (int i = 0; i < listGO.Count; i++) {
            listGO[i].Playing(index == i);
        }
        cam.SetFocus(listGO[index].transform);
    }
    private void Update() {
        if(listGO.Count > 0) {
            if (InputHandler.Controller.rightShoulder.wasPressedThisFrame) {
                int oldIndex = index;
                index++;
                ChangePlayer(oldIndex);
            } else if (InputHandler.Controller.leftShoulder.wasPressedThisFrame) {
                int oldIndex = index;
                if (index > 0) index--;
                else index = listGO.Count - 1;
                ChangePlayer(oldIndex);
            }
        }
    }

    void ChangePlayer(int oldIndex) {
        index %= listGO.Count;
        listGO[oldIndex].Playing(false);
        listGO[index].Playing(true);
        cam.SetFocus(listGO[index].transform);
    }
}
