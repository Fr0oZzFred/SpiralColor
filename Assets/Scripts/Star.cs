using UnityEngine;
public class Star : MonoBehaviour {
    enum Index { First = 1, Second = 2, Third = 3};

    [SerializeField] Index indexLevel;
    public int StarIndex {
        get {
            return (int)indexLevel;
        }
    }
    private void Start() {
        gameObject.SetActive(!GameManager.Instance.CheckStar(this));
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>())
            Collect();
    }
    public void Collect() {
        GameManager.Instance.CollectStar(this);
        gameObject.SetActive(false);
    }
}