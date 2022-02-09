using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[System.Serializable]
public class PlayerData {
    public int level;
    public PlayerData(Player player) {
        level = player.level;
    }
}
public class Player : MonoBehaviour {
    public int level = 3;
    private void Start() {
        Debug.Log(level);
    }
    void Update() {
        if (Keyboard.current.aKey.wasPressedThisFrame) { level++; Debug.Log(level); } 
        else if (Keyboard.current.zKey.wasPressedThisFrame) { level--; Debug.Log(level); }
        if (Keyboard.current.eKey.wasPressedThisFrame) SavePlayer();
        if (Keyboard.current.rKey.wasPressedThisFrame) LoadPlayer();
    }
    public void SavePlayer() {
        SaveSystem.SavePlayer(this);
        Debug.Log("save");
        Debug.Log(level);
    }
    public void LoadPlayer() {
        PlayerData data = SaveSystem.LoadPlayer();
        level = data.level;
        Debug.Log("load");
        Debug.Log(level);
    }
}
