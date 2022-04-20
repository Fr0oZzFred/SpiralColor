using UnityEngine;
public class Gems : MonoBehaviour {
    //public Material transparent;
    public int gemIndex;
    private void Start() {
        if (GameManager.Instance.gemmes[LevelManager.Instance.LevelInt].Count < FindObjectsOfType(typeof(Gems)).Length)
            GameManager.Instance.AddGem();
        else if (GameManager.Instance.CheckGem(gemIndex)) gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>())
            Collect();
    }
    public void Collect() {
        if(!GameManager.Instance.gemmes[LevelManager.Instance.LevelInt][gemIndex]) GameManager.Instance.CollectGem(gemIndex);
        gameObject.SetActive(false);
    }
}