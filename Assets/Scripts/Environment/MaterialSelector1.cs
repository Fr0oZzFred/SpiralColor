using UnityEngine;

public class MaterialSelector1 : MonoBehaviour {

	[SerializeField] Material[] materials = default;

	[SerializeField] MeshRenderer meshRenderer = default;

	public void Select(int index) {
		if (
			meshRenderer && materials != null &&
			index >= 0 && index < materials.Length
		) {
			Material[] mat = meshRenderer.materials;
			mat[1] = materials[index];
			meshRenderer.materials = mat;
		}
	}
}