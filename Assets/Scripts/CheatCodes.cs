using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CheatCodes : MonoBehaviour {
    public static CheatCodes Instance { get; private set; }
    public int LevelTarget = 10;
    public List<List<bool>> gemsList { get; private set; }
    public List<int> GemsTypesIndex { get; private set; }
    int randGem;
    void Awake() {
        Instance = this;
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
        gemsList = new List<List<bool>>();
        GemsTypesIndex = new List<int>();
        for (int i = 0; i < 16; i++) gemsList.Add(new List<bool>());
        for (int level = 0; level < LevelTarget; level++) {
            switch (level) {
                case 1:
                    for (int i = 0; i < 8; i++) {
                        gemsList[level].Add(true);
                        GemsTypesIndex.Add(0);
                    }
                    break;
                case 2:
                    for (int i = 0; i < 10; i++) {
                        gemsList[level].Add(true);
                        GemsTypesIndex.Add(1);
                    }
                    break;
                case 3:
                    for (int i = 0; i < 12; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(0, 2);
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 4:
                    for (int i = 0; i < 10; i++) {
                        gemsList[level].Add(true);
                        GemsTypesIndex.Add(2);
                    }
                    break;
                case 5:
                    for (int i = 0; i < 26; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(0, 2) == 1 ? 2 : 0;
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 6:
                    for (int i = 0; i < 14; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(1, 3);
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 7:
                    for (int i = 0; i < 12; i++) {
                        gemsList[level].Add(true);
                        GemsTypesIndex.Add(3);
                    }
                    break;
                case 8:
                    for (int i = 0; i < 18; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(0, 4) < 2 ? 0 : 3;
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 9:
                    for (int i = 0; i < 30; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(1, 4) < 2 ? 1 : 3;
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 10:
                    for (int i = 0; i < 24; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(2, 4);
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 11:
                    for (int i = 0; i < 26; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(0, 3);
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 12:
                    for (int i = 0; i < 32; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(0, 3);
                        randGem = randGem == 2 ? 3 : randGem;
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 13:
                    for (int i = 0; i < 38; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(1, 4);
                        randGem = randGem == 1 ? 0 : randGem;
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
                case 14:
                    for (int i = 0; i < 30; i++) {
                        gemsList[level].Add(true);
                        randGem = Random.Range(1, 4);
                        GemsTypesIndex.Add(randGem);
                    }
                    break;
            }
        }
        return gemsList;
    }
}
