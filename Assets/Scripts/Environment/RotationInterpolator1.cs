using UnityEngine;

public class RotationInterpolator1 : MonoBehaviour {
	[SerializeField] float speed;
	float angle;
    private void Awake() {
		angle = 0f;
    }
    private void Update() {
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, -90f, angle), speed * Time.deltaTime);
	}
    public void Right() {
		angle += 90f;
	}
	public void Left() {
		angle -= 90f;
    }
}