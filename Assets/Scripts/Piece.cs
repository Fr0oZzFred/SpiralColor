using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Piece : MonoBehaviour{
    [HideInInspector]public int level;
    public int index;
    private void Start() {
        level = int.Parse(LevelManager.Instance.Level);
    }
    private void OnMouseDown() {
        GameManager.Instance.CheckPiece(this);
        gameObject.SetActive(!GameManager.Instance.pieces["Star " + level + "-" + index]);
    }
}