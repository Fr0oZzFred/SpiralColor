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
        if (Keyboard.current.numpad1Key.wasPressedThisFrame) LevelTarget = 1;
        else if (Keyboard.current.numpad2Key.wasPressedThisFrame) LevelTarget = 2;
        else if (Keyboard.current.numpad3Key.wasPressedThisFrame) LevelTarget = 3;
        else if (Keyboard.current.numpad4Key.wasPressedThisFrame) LevelTarget = 4;
        else if (Keyboard.current.numpad5Key.wasPressedThisFrame) LevelTarget = 5;
        else if (Keyboard.current.numpad6Key.wasPressedThisFrame) LevelTarget = 6;
        else if (Keyboard.current.numpad7Key.wasPressedThisFrame) LevelTarget = 7;
        else if (Keyboard.current.numpad8Key.wasPressedThisFrame) LevelTarget = 8;
        else if (Keyboard.current.numpad9Key.wasPressedThisFrame) LevelTarget = 9;
        else if (Keyboard.current.numpad0Key.wasPressedThisFrame) LevelTarget = 10;
        else if (Keyboard.current.yKey.wasPressedThisFrame) LevelTarget = 11;
        else if (Keyboard.current.uKey.wasPressedThisFrame) LevelTarget = 12;
        else if (Keyboard.current.iKey.wasPressedThisFrame) LevelTarget = 13;
        else if (Keyboard.current.oKey.wasPressedThisFrame) LevelTarget = 14;
        else if (Keyboard.current.pKey.wasPressedThisFrame) LevelTarget = 15;
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
