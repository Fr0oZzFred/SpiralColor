using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIManager : MonoBehaviour{
    public static UIManager Instance { get; private set; }
    public GameObject pauseMenu;
    void Awake() {
        Instance = this;
    }
    public void QuitPause() {
        pauseMenu.SetActive(false);
        GameManager.Instance.SetState(GameState.play);
    }
}
