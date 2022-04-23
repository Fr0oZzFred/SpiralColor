using UnityEngine;
public class Gems : MonoBehaviour {

    [SerializeField] float speed, amplitude;
    [SerializeField] Vector3 rotSpeed;

    Vector3 startPos;

    public int gemIndex;
    private void Start() {
        startPos = transform.position;
        if (GameManager.Instance.gemsList[LevelManager.Instance.LevelInt].Count < FindObjectsOfType(typeof(Gems)).Length)
            GameManager.Instance.AddGem();
        else if (GameManager.Instance.CheckGem(gemIndex)) gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>())
            Collect();
    }
    public void Collect() {
        if(!GameManager.Instance.gemsList[LevelManager.Instance.LevelInt][gemIndex]) GameManager.Instance.CollectGem(gemIndex);
        gameObject.SetActive(false);
    }
    private void Update() {
        transform.Rotate(rotSpeed * Time.deltaTime);
        transform.position = new Vector3(
            startPos.x,
            startPos.y + Mathf.Sin(Time.time * speed) * amplitude,
            startPos.z
        );
    }
}