using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Piece : MonoBehaviour{
    [HideInInspector]public int level;
    public int index;
    void Start() {
        level = int.Parse(LevelManager.Instance.Level.Remove(0, LevelManager.Instance.Level.Length - 1));
    }
    private void Update() {
        if (Keyboard.current.aKey.wasPressedThisFrame && index == 1 ||
            Keyboard.current.zKey.wasPressedThisFrame && index == 2 ||
            Keyboard.current.eKey.wasPressedThisFrame && index == 3) Collect();
    }
    public void Collect() {
        GameManager.Instance.CheckPiece(this);
        gameObject.SetActive(!GameManager.Instance.pieces["Star " + level + "-" + index]);
    }
}