using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HUBManager : MonoBehaviour
{
    [SerializeField]
    string musicName;

    public int numberOfGems; //Change with GM gems follower
    public GemsPool gemsPool;

    public float secondsBetweenSpawn;

    private void Awake() {
        gemsPool.InitPool(numberOfGems);
        GameManager.Instance.SetState(GameState.InHUB);
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(musicName);
    }
    private IEnumerator Start() {
        for (int i = 0; i < numberOfGems; i++) {
            gemsPool.InstantiatePrefab();
            yield return new WaitForSeconds(secondsBetweenSpawn);
        }
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
