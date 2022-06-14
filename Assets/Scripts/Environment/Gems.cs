using UnityEngine;
public enum GemsTypes { Circle, Triangle, Square, Cross }
public class Gems : MonoBehaviour {

    [SerializeField] float speed, amplitude;
    [SerializeField] Vector3 rotSpeed;
    [SerializeField] string sound;
    public GemsTypes type;
    Vector3 startPos;

    public int gemIndex;
    private void Awake() {
        startPos = transform.localPosition;
        if (GameManager.Instance.gemsList[LevelManager.Instance.LevelInt].Count < FindObjectsOfType(typeof(Gems)).Length)
            GameManager.Instance.AddGem();
        else if (GameManager.Instance.CheckGem(gemIndex)) gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>())
            Collect();
    }
    public void Collect() {
        if(!GameManager.Instance.gemsList[LevelManager.Instance.LevelInt][gemIndex]) GameManager.Instance.CollectGem(gemIndex, type);
        gameObject.SetActive(false);
        UIManager.Instance.DisplayGems();
        SoundsManager.Instance.Play(sound);
    }
    private void Update() {
        transform.Rotate(rotSpeed * Time.deltaTime);
        transform.localPosition = new Vector3(
            startPos.x,
            startPos.y + Mathf.Sin(Time.time * speed) * amplitude,
            startPos.z
        );
    }
#if UNITY_EDITOR
    private void OnValidate() {
        string meshName = GetComponent<MeshFilter>().sharedMesh.name;
        this.gameObject.name = $"Gem_{meshName} {gemIndex}";
    }
#endif
}