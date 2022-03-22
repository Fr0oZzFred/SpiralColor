using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EndLevel : MonoBehaviour{
    public static EndLevel Instance { get; private set; }
    public int piece = 0;
    public GameObject UI;
    void Awake() {
        Instance = this;
    }
    public void PlusPiece(GameObject bouton) {
        piece++;
        bouton.SetActive(false);
    }
    public void Score() {
        UI.SetActive(true);
        Debug.Log(piece);
    }
    public void ToHub() {
        Debug.Log("you're in the hub");
    }
}
