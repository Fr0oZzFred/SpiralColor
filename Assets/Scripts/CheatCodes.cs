using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CheatCodes : MonoBehaviour{
    public static CheatCodes Instance { get; private set; }
    public int LevelTarget = 10;
    public List<List<bool>> gemsList { get; private set; }
    void Awake() {
        Instance = this;
        gemsList = new List<List<bool>>();
        for (int i = 0; i < 16; i++) gemsList.Add(new List<bool>());
    }
    void Update() {
        if (Keyboard.current.cKey.wasPressedThisFrame) GameManager.Instance.LoadCheat();
    }
    public List<List<bool>> GemsLevel() {
        for (int level = 0; level < LevelTarget; level++) {
            switch (level) {
                case 1:
                    for (int i = 0; i < 8; i++) gemsList[level].Add(true);
                    break;  
                case 2:
                    for (int i = 0; i < 10; i++) gemsList[level].Add(true);
                    break;
                case 3:
                    for (int i = 0; i < 12; i++) gemsList[level].Add(true);
                    break;
                case 4:
                    for (int i = 0; i < 10; i++) gemsList[level].Add(true);
                    break;
                case 5:
                    for (int i = 0; i < 26; i++) gemsList[level].Add(true);
                    break;
                case 6:
                    for (int i = 0; i < 14; i++) gemsList[level].Add(true);
                    break;
                case 7:
                    for (int i = 0; i < 12; i++) gemsList[level].Add(true);
                    break;
                case 8:
                    for (int i = 0; i < 18; i++) gemsList[level].Add(true);
                    break;
                case 9:
                    for (int i = 0; i < 30; i++) gemsList[level].Add(true);
                    break;
                case 10:
                    for (int i = 0; i < 24; i++) gemsList[level].Add(true);
                    break;
                case 11:
                    for (int i = 0; i < 26; i++) gemsList[level].Add(true);
                    break;
                case 12:
                    for (int i = 0; i < 32; i++) gemsList[level].Add(true);
                    break;
                case 13:
                    for (int i = 0; i < 38; i++) gemsList[level].Add(true);
                    break;
                case 14:
                    for (int i = 0; i < 30; i++) gemsList[level].Add(true);
                    break;
            }
        }
        return gemsList;
    }
}
