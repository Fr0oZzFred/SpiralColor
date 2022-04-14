using UnityEngine;
using UnityEngine.InputSystem;

public class HUBManager : MonoBehaviour
{
    [SerializeField]
    string musicName;

    public int numberOfGems; //Change with GM gems follower
    public GemsPool gemsPool;

    private void Awake() {
        gemsPool.InitPool(numberOfGems);
        GameManager.Instance.SetState(GameState.InHUB);
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(musicName);
    }

    private void Update() {
        if (Keyboard.current.aKey.wasPressedThisFrame) {
            gemsPool.InstantiatePrefab();
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame) {
            gemsPool.Destroy();
        }
    }
}
