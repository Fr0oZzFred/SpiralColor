using UnityEngine;
public class Gemme : MonoBehaviour {
    //public Material transparent;
    public int gemmeIndex;
    private void Start() {
        if (GameManager.Instance.gemmes[LevelManager.Instance.LevelInt].Count < FindObjectsOfType(typeof(Gemme)).Length)
            GameManager.Instance.AddGemme();
        if (GameManager.Instance.CheckGemme(gemmeIndex)) gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>())
            Collect();
    }
    public void Collect() {
        if(!GameManager.Instance.gemmes[LevelManager.Instance.LevelInt][gemmeIndex]) GameManager.Instance.CollectGemme(gemmeIndex);
        gameObject.SetActive(false);
    }
}