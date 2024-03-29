using UnityEngine;

public class SpherePlayerController : Controller {
    #region Fields
    [Tooltip("Camera pour baser le déplacement du joueur")]
    [SerializeField]
    Transform playerInputSpace = default, ball = default;


    [Header("Speed")]
    [Tooltip("Vitesse maximum")]
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;


    [Header("Acceleration")]
    [Tooltip("Acceleration maximum")]
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [Tooltip("Acceleration en l'air")]
    [SerializeField, Range(0f, 100f)]
    float maxAirAcceleration = 1f;


    [Header("Jump")]
    [Tooltip("Hauteur du saut")]
    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [Tooltip("Nombre de saut sans toucher le sol")]
    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;


    [Header("Angles")]
    [Tooltip("Angle maximum pour catégoriser le sol")]
    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    [Tooltip("Angle maximum pour monter des escaliers")]
    [SerializeField, Range(0, 90)]
    float maxStairsAngle = 50f;

    [Tooltip("Vitesse maximum pour calculer l'angle")]
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [Tooltip("Range de détection")]
    [SerializeField, Min(0f)]
    float probeDistance = 1f;


    [Header("Layers")]
    [SerializeField]
    LayerMask probeMask = -1;

    [SerializeField]
    LayerMask stairsMask = -1;


    [Header("Ball Settings")]
    [Tooltip("Simplification : Vitesse de la roue donc elle avance et recule")]
    [SerializeField, Min(0.1f)]
    float ballRadius = 0.5f;

    [Tooltip("Vitesse alignement avec la direction")]
    [SerializeField, Min(0f)]
    float ballAlignSpeed = 180f;

    [Tooltip("Vitesse alignement avec la direction dans l'air")]
    [SerializeField, Min(0f)]
    float ballAirRotation = 0.5f;


    [Header("Controller Color")]
    [SerializeField]
    [ColorUsage(true, true)]
    Color colorOn = default;
    [SerializeField]
    [ColorUsage(true, true)]
    Color colorOff = default;
    [SerializeField]
    [ColorUsage(true, true)]
    Color sphereColor = default;

    [Header("Checkpoints")]
    [SerializeField]
    int[] allowedCheckpointsList;

    [Header("Ath")]
    [SerializeField]
    DynamicATH ath;


    [Header("Decal")]
    [SerializeField]
    MeshRenderer decal;

    Material material;

    Quaternion baseCamDirection;
    
    bool isCurrentlyPlayed = false;

    Rigidbody body, connectedBody, previousConnectedBody;

    Vector3 playerInput;

    Vector3 velocity, connectionVelocity;

    Vector3 connectionWorldPosition, connectionLocalPosition;

    Vector3 upAxis, rightAxis, forwardAxis;

    bool desiredJump;

    Vector3 contactNormal, steepNormal;

    Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

    int groundContactCount, steepContactCount;

    bool OnGround => groundContactCount > 0;

    bool OnSteep => steepContactCount > 0;

    int jumpPhase;

    float minGroundDotProduct, minStairsDotProduct;

    int stepsSinceLastGrounded, stepsSinceLastJump;

    MeshRenderer meshRenderer;

    #endregion

    void PreventSnapToGroundP() {
        stepsSinceLastJump = -1;
    }

    void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
    }

    void Awake() {
        baseCamDirection = playerInputSpace.rotation;
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        meshRenderer = ball.GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        OnValidate();
    }
    /// <summary>
    /// Check for the playerInput and if is played by the Player Handler
    /// </summary>
    void Update() {
        if (isCurrentlyPlayed) {
            playerInput.x = InputHandler.GetLeftStickValues().x;
            playerInput.z = InputHandler.GetLeftStickValues().y;
            playerInput = Vector3.ClampMagnitude(playerInput, 1f);
        } else playerInput = Vector3.zero;

        if (playerInputSpace) {
            rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
            forwardAxis =
                ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
        } else {
            rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
        }


        if (!isCurrentlyPlayed || InputHandler.Controller == null) {
            UpdateBall();
            return;
        }
        desiredJump |= InputHandler.Controller.buttonEast.wasPressedThisFrame;

        UIManager.Instance.UpdateMidBar(0, InputHandler.Controller.buttonEast.isPressed);
        UpdateBall();
    }
    /// <summary>
    /// For the Ball rotation and material
    /// </summary>
    void UpdateBall() {
        Vector3 rotationPlaneNormal = lastContactNormal;
        float rotationFactor = 1f;
        if (!OnGround) {
            if (OnSteep) {
                rotationPlaneNormal = lastSteepNormal;
            } else {
                rotationFactor = ballAirRotation;
            }
        }

        Vector3 movement =
            (body.velocity - lastConnectionVelocity) * Time.deltaTime;
        movement -=
            rotationPlaneNormal * Vector3.Dot(movement, rotationPlaneNormal);

        float distance = movement.magnitude;

        Quaternion rotation = ball.localRotation;
        if (connectedBody && connectedBody == previousConnectedBody) {
            rotation = Quaternion.Euler(
                connectedBody.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)
            ) * rotation;
            if (distance < 0.001f) {
                ball.localRotation = rotation;
                return;
            }
        } else if (distance < 0.001f) {
            return;
        }

        float angle = distance * rotationFactor * (180f / Mathf.PI) / ballRadius;
        Vector3 rotationAxis =
            Vector3.Cross(rotationPlaneNormal, movement).normalized;
        rotation = Quaternion.Euler(rotationAxis * angle) * rotation;
        if (ballAlignSpeed > 0f) {
            rotation = AlignBallRotation(rotationAxis, rotation, distance);
        }
        ball.localRotation = rotation;
    }

    Quaternion AlignBallRotation(
        Vector3 rotationAxis, Quaternion rotation, float traveledDistance
    ) {
        Vector3 ballAxis = ball.up;
        float dot = Mathf.Clamp(Vector3.Dot(ballAxis, rotationAxis), -1f, 1f);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        float maxAngle = ballAlignSpeed * traveledDistance;

        Quaternion newAlignment =
            Quaternion.FromToRotation(ballAxis, rotationAxis) * rotation;
        if (angle <= maxAngle) {
            return newAlignment;
        } else {
            return Quaternion.SlerpUnclamped(
                rotation, newAlignment, maxAngle / angle
            );
        }
    }

    void FixedUpdate() {
        Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
        UpdateState();

        AdjustVelocity();

        if (desiredJump) {
            desiredJump = false;
            Jump(gravity);
        }

        if (OnGround && velocity.sqrMagnitude < 0.01f) {
            velocity +=
                contactNormal *
                (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
        } else {
            velocity += gravity * Time.deltaTime;
        }
        body.velocity = velocity;
        ClearState();
    }

    void ClearState() {
        lastContactNormal = contactNormal;
        lastSteepNormal = steepNormal;
        lastConnectionVelocity = connectionVelocity;
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
        connectionVelocity = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
    }

    void UpdateState() {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (OnGround || SnapToGround() || CheckSteepContacts()
        ) {
            stepsSinceLastGrounded = 0;
            if (stepsSinceLastJump > 1) {
                jumpPhase = 0;
            }
            if (groundContactCount > 1) {
                contactNormal.Normalize();
            }
        } else {
            contactNormal = upAxis;
        }

        if (connectedBody) {
            if (connectedBody.isKinematic || connectedBody.mass >= body.mass) {
                UpdateConnectionState();
            }
        }
    }

    void UpdateConnectionState() {
        if (connectedBody == previousConnectedBody) {
            Vector3 connectionMovement =
                connectedBody.transform.TransformPoint(connectionLocalPosition) -
                connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
            connectionWorldPosition
        );
    }

    bool SnapToGround() {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed) {
            return false;
        }
        if (!Physics.Raycast(
            body.position, -upAxis, out RaycastHit hit,
            probeDistance, probeMask, QueryTriggerInteraction.Ignore
        )) {
            return false;
        }

        float upDot = Vector3.Dot(upAxis, hit.normal);
        if (upDot < GetMinDot(hit.collider.gameObject.layer)) {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        return true;
    }

    bool CheckSteepContacts() {
        if (steepContactCount > 1) {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
            if (upDot >= minGroundDotProduct) {
                steepContactCount = 0;
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Complex calcul for the velocity
    /// </summary>
    void AdjustVelocity() {
        float acceleration, speed;
        Vector3 xAxis, zAxis;
        acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        speed = maxSpeed;
        xAxis = rightAxis;
        zAxis = forwardAxis;
        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;

        Vector3 adjustment;
        adjustment.x =
            playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z =
            playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = 0f;

        adjustment =
            Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        velocity += xAxis * adjustment.x + zAxis * adjustment.z;
    }

    void Jump(Vector3 gravity) {
        Vector3 jumpDirection;
        if (OnGround) {
            jumpDirection = contactNormal;
        } else if (OnSteep) {
            jumpDirection = steepNormal;
            jumpPhase = 0;
        } else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
            if (jumpPhase == 0) {
                jumpPhase = 1;
            }
            jumpDirection = contactNormal;
        } else {
            return;
        }

        stepsSinceLastJump = 0;
        jumpPhase += 1;
        float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
        jumpDirection = (jumpDirection + upAxis).normalized;
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        if (alignedSpeed > 0f) {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        velocity += jumpDirection * jumpSpeed;
    }

    void OnCollisionEnter(Collision collision) {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision) {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision) {
        int layer = collision.gameObject.layer;
        float minDot = GetMinDot(layer);
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            if (upDot >= minDot) {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
            } else {
                if (upDot > -0.01f) {
                    steepContactCount += 1;
                    steepNormal += normal;
                    if (groundContactCount == 0) {
                        connectedBody = collision.rigidbody;
                    }
                }
            }
        }
    }

    Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal) {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    float GetMinDot(int layer) {
        return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
    }
    #region Controller abstractFunctions
    public override void RegisterInputs(bool b) {
        isCurrentlyPlayed = b;
        decal.material.color = b ? colorOn : Color.black;
        material.color = b ? colorOn : colorOff;
        material.SetColor("_SphereColor", b ? sphereColor : Color.black);
        if (GameManager.Instance.CurrentState != GameState.Pause) {
            playerInputSpace.gameObject.SetActive(b);
            UIManager.Instance.UpdateATH(b ? 0 : -1);
        }
    }
    public override void PreventSnapToGround() {
        PreventSnapToGroundP();
    }
    public override void SetControllerLED() {
        InputHandler.SetControllerLED(colorOn);
    }

    public override void Respawn(Vector3 pos, Quaternion rotation) {
        this.transform.position = pos;
        body.velocity = velocity = Vector3.zero;
        SetCamRotation(rotation);
    }
    public override void SetInputSpace(Transform transform) {
        playerInputSpace = transform;
    }

    public override int GetClosestAllowedCheckpoint(int actualProgression) {
        if (allowedCheckpointsList.Length == 0) {
            Debug.LogWarning("Allowed Checkpoints List is empty.");
            return 0;
        }
        int closestCheckpoint = 0;
        foreach (int index in allowedCheckpointsList) {
            if (index == actualProgression)
                return actualProgression;
            if (closestCheckpoint < index && index < actualProgression)
                closestCheckpoint = index;
        }
        return closestCheckpoint;
    }
    public override Quaternion GetCamRotation() {
        return playerInputSpace.rotation;
    }
    public override void SetCamRotation(Quaternion q) {
        OrbitCamera cam = playerInputSpace.GetComponent<OrbitCamera>();
        if (cam)
            cam.SetRot(q);
    }
    public override Transform GetCam() {
        return playerInputSpace;
    }
    public override void DisplayATH(bool b) {
        ath.DisplayATH(b);
    }
    public override Transform GetShape() {
        return ball;
    }
    #endregion
}
