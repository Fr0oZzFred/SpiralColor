using UnityEngine;

public class PlayerController : Controller {
    #region Fields
    [Tooltip("Camera pour baser le déplacement du joueur")]
    [SerializeField]
    Transform playerInputSpace = default, player = default;


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


    [Header("Materials")]
    [SerializeField]
    Material normalMaterial = default;


    [Header("Player Settings")]
    [Tooltip("Vitesse d'alignement de base")]
    [SerializeField]
    float baseAlignementSpeed = 0.01f;

    [SerializeField]
    float speed, amplitude;

    [Header("Checkpoints")]
    [SerializeField]
    int[] allowedCheckpointsList;

    [Header("Help Box")]
    [SerializeField]
    string helpBoxMessage;

    [Header("Controller Color")]
    [SerializeField]
    Color color = default;


    public bool IsCurrentlyPlayed {
        get {
            return isCurrentlyPlayed;
        }
    }


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
        meshRenderer = player.GetComponent<MeshRenderer>();
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

        
        if (!OnGround)
            desiredJump |= InputHandler.Controller.buttonSouth.wasPressedThisFrame;

        UpdatePlayer();
    }
    /// <summary>
    /// For the Square rotation and material
    /// </summary>
    void UpdatePlayer() {
        Material playerMaterial = normalMaterial;
        Vector3 rotationPlaneNormal = lastContactNormal;
        if (!OnGround) {
            if (OnSteep) {
                rotationPlaneNormal = lastSteepNormal;
            }
        }
        meshRenderer.material = playerMaterial;

        Vector3 movement =
            (body.velocity - lastConnectionVelocity) * Time.deltaTime;
        movement -=
            rotationPlaneNormal * Vector3.Dot(movement, rotationPlaneNormal);
        float distance = movement.magnitude;
        Quaternion rotation = player.localRotation;
        if (connectedBody && connectedBody == previousConnectedBody) {
            rotation = Quaternion.Euler(
                connectedBody.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)
            ) * rotation;
            if (distance < 0.001f) {
                player.localRotation = rotation; 
                Levitate();
                return;
            }
        } else if (distance < 0.001f) {
            Levitate();
            return;
        }

        if (movement != Vector3.zero)
            rotation = Quaternion.LookRotation(movement);

        player.localRotation = Quaternion.Lerp(player.localRotation, rotation, baseAlignementSpeed * Time.deltaTime);
        Levitate();
    }
    void Levitate() {
        Vector3 pos = Vector3.zero;
        pos.y = (Mathf.Sin(Time.time * speed) + 1f) / 2f * amplitude;
        player.localPosition = pos;
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
        playerInputSpace.gameObject.SetActive(b);
        if (GameManager.Instance) {
            if (HUBManager.Instance && HUBManager.Instance.playerInSelection) return;
            else if (GameManager.Instance.CurrentState != GameState.Pause) {
                gameObject.SetActive(b);
            }
        } else {
            gameObject.SetActive(b);
        }
    }
    public override void PreventSnapToGround() {
        PreventSnapToGroundP();
    }

    public override void SetControllerLED() {
        InputHandler.SetControllerLED(color);
    }
    public override void Respawn(Vector3 pos) {
        this.transform.position = pos;
        body.velocity = velocity = Vector3.zero;
        SetCamRotation(baseCamDirection);
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
    public override string GetHelpBoxMessage() {
        return helpBoxMessage;
    }
    public override Quaternion GetCamRotation() {
        return playerInputSpace.rotation;
    }
    public override void SetCamRotation(Quaternion q) {
        OrbitCamera cam = playerInputSpace.GetComponent<OrbitCamera>();
        if(cam)
            cam.SetRot(q);
    }
    public override Transform GetCam() {
        return playerInputSpace;
    }
    #endregion
}
