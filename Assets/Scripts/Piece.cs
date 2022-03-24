using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Piece : MonoBehaviour{
    public enum Index { first, second, third};
    public Index indexLevel;
    public int index {
        get {
            return (int)indexLevel;
        }
    }
    private void Update() {
        if (Keyboard.current.aKey.wasPressedThisFrame && index == 1 ||
            Keyboard.current.zKey.wasPressedThisFrame && index == 2 ||
            Keyboard.current.eKey.wasPressedThisFrame && index == 3) Collect();
    }
    public void Collect() {
        GameManager.Instance.CheckPiece(this);
        gameObject.SetActive(!GameManager.Instance.pieces["Star " + LevelManager.Instance.LevelInt + "-" + index]);
    }
}