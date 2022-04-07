using UnityEngine;

public class HUBManager : MonoBehaviour
{
    [SerializeField]
    string musicName;
    private void Awake() {
        GameManager.Instance.SetState(GameState.InHUB);
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(musicName);
    }
}
