using UnityEngine;

public class PlayMusic : MonoBehaviour {

    [SerializeField]
    string name;
    void Start()
    {
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(name);
    }

}
