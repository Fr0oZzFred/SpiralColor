using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour {
    [SerializeField] private GameObject sphere;
    [SerializeField] private Vector3 baseGravity;
    [SerializeField] private Vector3 maxGravity;
    [SerializeField] private float gravityModifierSpeed;

    private Vector3 modifiedGravity;

    private bool maxGravityReached;


    private void Start() {
        modifiedGravity = baseGravity;
    }

    private void FixedUpdate() {
        if (maxGravityReached)
            Destroy(this);

        modifiedGravity.y = (baseGravity.y * sphere.transform.position.z * gravityModifierSpeed) * Time.fixedDeltaTime;
        if (modifiedGravity.y < maxGravity.y) {
            maxGravityReached = true;
            modifiedGravity.y = maxGravity.y;
        }
        if (modifiedGravity.y > baseGravity.y)
            modifiedGravity.y = baseGravity.y;
        Physics.gravity = modifiedGravity;
    }
}