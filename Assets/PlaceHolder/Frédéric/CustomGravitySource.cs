using UnityEngine;
public enum GravityDirection {
	Up,
	Down,
	Right,
	Left
}
public class CustomGravitySource : GravitySource {

	[SerializeField]
	float gravity = 9.81f;

	Vector3 actualGravity;
	
	[SerializeField]
	GravityDirection gravityDirection = default;

    private void Start() {
		ChangeGravity(gravityDirection);
    }

    public Vector3 ChangeGravity(GravityDirection direction) {
		gravityDirection = direction;
		Vector3 g = Vector3.zero;
        switch (gravityDirection) {
			case GravityDirection.Up:
				g.z = 0;
				g.y = gravity;
				actualGravity = g;
				return actualGravity;
			case GravityDirection.Down:
				g.z = 0;
				g.y = -gravity;
				actualGravity = g;
				return actualGravity;
			case GravityDirection.Right:
				g.z = gravity;
				g.y = 0;
				actualGravity = g;
				return actualGravity;
			case GravityDirection.Left:
				g.z = -gravity;
				g.y = 0;
				actualGravity = g;
				return actualGravity;
			default:
				return actualGravity;
		}
	}
	public override Vector3 GetGravity(Vector3 position) {
		return actualGravity;
    }


	private void Update() {
		if (InputHandler.Controller == null) return;
        if (InputHandler.Controller.dpad.up.wasPressedThisFrame) {
			ChangeGravity(GravityDirection.Up);
        }
		else if (InputHandler.Controller.dpad.down.wasPressedThisFrame) {
			ChangeGravity(GravityDirection.Down);
		}
		else if (InputHandler.Controller.dpad.right.wasPressedThisFrame) {
			ChangeGravity(GravityDirection.Right);
		}
		else if (InputHandler.Controller.dpad.left.wasPressedThisFrame) {
			ChangeGravity(GravityDirection.Left);
		}
	}
}