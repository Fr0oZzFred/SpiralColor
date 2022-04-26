using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HUBManager : MonoBehaviour {
    [SerializeField] PlayerHandler playerHandler;
    public static HUBManager Instance { get; private set; }
    [SerializeField]
    string musicName;
    public GemsPool gemsPool;

    private void Awake() {
        Instance = this;
        GameManager.Instance.SetState(GameState.InHUB);
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(musicName);
    }

    private void Start() {
        if (GameManager.Instance.GemsCount > gemsPool.gems.Count) {
            playerHandler.CurrentPlayer.RegisterInputs(false);
        }
    }
}
