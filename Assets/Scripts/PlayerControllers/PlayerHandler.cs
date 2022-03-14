using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {
    public List<GameObject> listGO;
    public int startingIndex = 0;
    int index = 0;

    public OrbitCamera cam;
    private void Start() {
        index = startingIndex > listGO.Count -1 ? 0 : startingIndex;
        for (int i = 0; i < listGO.Count; i++) {
            listGO[i].GetComponent<IControllable>().IsPlaying(index == i);
        }
        cam.SetFocus(listGO[index].transform);
        listGO[index].GetComponent<IControllable>().SetControllerLED();
    }

    private void Update() {
        if (InputHandler.Controller == null) return;
        if (listGO.Count > 0) {
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
    /// <summary>
    /// Change the player who is controlled by the player
    /// </summary>
    /// <param name="oldIndex"></param>
    void ChangePlayer(int oldIndex) {
        index %= listGO.Count;
        listGO[oldIndex].GetComponent<IControllable>().IsPlaying(false);
        listGO[index].GetComponent<IControllable>().IsPlaying(true);
        listGO[index].GetComponent<IControllable>().SetControllerLED();
        cam.SetFocus(listGO[index].transform);
    }

    public IControllable GetCurrentPlayer() {
        return listGO[index].GetComponent<IControllable>();
    }
    private void OnApplicationQuit() {
        InputHandler.SetControllerLED(Color.black);
    }
}
