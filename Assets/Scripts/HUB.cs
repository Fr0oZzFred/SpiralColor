using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUB : MonoBehaviour
{
    private void Awake() {
        GameManager.Instance.SetState(GameState.InHUB);
    }
}
