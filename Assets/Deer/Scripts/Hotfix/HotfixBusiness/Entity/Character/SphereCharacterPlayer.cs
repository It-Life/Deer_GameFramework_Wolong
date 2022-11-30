using GameFramework;
using Pathfinding;
using System;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    /// <summary>
    /// ÇòÌå½ÇÉ«
    /// </summary>
    public class SphereCharacterPlayer : EntityLogicBase
    {
        [SerializeField]
        Transform playerInputSpace = default;
        [SerializeField]
        Transform ball = default;

        public LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, waterMask = 0;

        [SerializeField]
        Material normalMaterial = default;
        [SerializeField]
        Material climbingMaterial = default;
        [SerializeField]
        Material swimmingMaterial = default;

        Rigidbody body, connectedBody, previousConnectedBody;

        SphereCharacterPlayerData m_CharacterData;
        public SphereCharacterPlayerData CharacterData { get { return m_CharacterData; } private set { m_CharacterData = value; } }

        Vector3 playerInput;

        Vector3 velocity, connectionVelocity;

        Vector3 connectionWorldPosition, connectionLocalPosition;

        Vector3 upAxis, rightAxis, forwardAxis;

        bool desiredJump, desiresClimbing;

        Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;

        Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

        int groundContactCount, steepContactCount, climbContactCount;

        bool OnGround => groundContactCount > 0;

        bool OnSteep => steepContactCount > 0;

        bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;

        bool InWater => submergence > 0f;

        bool Swimming => submergence >= CharacterData.SwimThreshold;

        float submergence;

        int jumpPhase;

        float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

        int stepsSinceLastGrounded, stepsSinceLastJump;

        MeshRenderer meshRenderer;

        public void PreventSnapToGround()
        {
            stepsSinceLastJump = -1;
        }

        public void RefreshPlayerInput(Vector2 inputV2)
        {
            playerInput.x = inputV2.x;
            playerInput.z = inputV2.y;
        }

        public void PlayerJump()
        {
            desiredJump |= true;
        }

        public void PlayerClimb(bool climbing)
        {
            desiresClimbing = climbing;
        }


        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            CharacterData = (SphereCharacterPlayerData)userData;
            CachedTransform.position = CharacterData.Position;

            ball = CachedTransform.Find("Model").GetChild(0);
            body = CachedTransform.GetComponent<Rigidbody>();
            body.useGravity = false;
            meshRenderer = ball.GetComponent<MeshRenderer>();

            minGroundDotProduct = Mathf.Cos(CharacterData.MaxGroundAngle * Mathf.Deg2Rad);
            minStairsDotProduct = Mathf.Cos(CharacterData.MaxStairsAngle * Mathf.Deg2Rad);
            minClimbDotProduct = Mathf.Cos(CharacterData.MaxClimbAngle * Mathf.Deg2Rad);

            playerInputSpace = Camera.main.transform;
            OrbitCamera orbitCam = playerInputSpace.GetComponent<OrbitCamera>();
            orbitCam.Focus = CachedTransform;
            orbitCam.FocusPoint = CachedTransform.position;
        }

        void Update()
        {
            if (CharacterData == null) return;


            if (playerInput.x == 0 && playerInput.z == 0)
            {
                playerInput.x = Input.GetAxis("Horizontal");
                playerInput.z = Input.GetAxis("Vertical");
            }

            playerInput.y = Swimming ? Input.GetAxis("UpDown") : 0f;

            playerInput = Vector3.ClampMagnitude(playerInput, 1f);

            if (playerInputSpace)
            {
                rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
                forwardAxis = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
            }
            else
            {
                rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
                forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
            }

            if (Swimming)
            {
                desiresClimbing = false;
            }
            else
            {
                desiredJump |= Input.GetButtonDown("Jump");

                //if (!desiresClimbing)
                //    desiresClimbing = Input.GetButton("Climb");
            }

            UpdateBall();
        }

        void UpdateBall()
        {
            Material ballMaterial = normalMaterial;
            Vector3 rotationPlaneNormal = lastContactNormal;
            float rotationFactor = 1f;
            if (Climbing)
            {
                ballMaterial = climbingMaterial;
            }
            else if (Swimming)
            {
                ballMaterial = swimmingMaterial;
                rotationFactor = CharacterData.BallSwimRotation;
            }
            else if (!OnGround)
            {
                if (OnSteep)
                {
                    rotationPlaneNormal = lastSteepNormal;
                }
                else
                {
                    rotationFactor = CharacterData.BallAirRotation;
                }
            }
            //meshRenderer.material = ballMaterial;

            Vector3 movement = (body.velocity - lastConnectionVelocity) * Time.deltaTime;
            movement -= rotationPlaneNormal * Vector3.Dot(movement, rotationPlaneNormal);

            float distance = movement.magnitude;

            Quaternion rotation = ball.localRotation;
            if (connectedBody && connectedBody == previousConnectedBody)
            {
                rotation = Quaternion.Euler(
                    connectedBody.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)
                ) * rotation;
                if (distance < 0.001f)
                {
                    ball.localRotation = rotation;
                    return;
                }
            }
            else if (distance < 0.001f)
            {
                return;
            }

            float angle = distance * rotationFactor * (180f / Mathf.PI) / CharacterData.BallRadius;
            Vector3 rotationAxis =
                Vector3.Cross(rotationPlaneNormal, movement).normalized;
            rotation = Quaternion.Euler(rotationAxis * angle) * rotation;
            if (CharacterData.BallAlignSpeed > 0f)
            {
                rotation = AlignBallRotation(rotationAxis, rotation, distance);
            }
            ball.localRotation = rotation;
        }

        Quaternion AlignBallRotation(Vector3 rotationAxis, Quaternion rotation, float traveledDistance)
        {
            Vector3 ballAxis = ball.up;
            float dot = Mathf.Clamp(Vector3.Dot(ballAxis, rotationAxis), -1f, 1f);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            float maxAngle = CharacterData.BallAlignSpeed * traveledDistance;

            Quaternion newAlignment =
                Quaternion.FromToRotation(ballAxis, rotationAxis) * rotation;
            if (angle <= maxAngle)
            {
                return newAlignment;
            }
            else
            {
                return Quaternion.SlerpUnclamped(
                    rotation, newAlignment, maxAngle / angle
                );
            }
        }

        void FixedUpdate()
        {
            if (CharacterData == null) return;

            Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
            UpdateState();

            if (InWater)
            {
                velocity *= 1f - CharacterData.WaterDrag * submergence * Time.deltaTime;
            }

            AdjustVelocity();

            if (desiredJump)
            {
                desiredJump = false;
                Jump(gravity);
            }

            if (Climbing)
            {
                velocity -= contactNormal * (CharacterData.MaxClimbAcceleration * 0.9f * Time.deltaTime);
            }
            else if (InWater)
            {
                velocity += gravity * ((1f - CharacterData.Buoyancy * submergence) * Time.deltaTime);
            }
            else if (OnGround && velocity.sqrMagnitude < 0.01f)
            {
                velocity += contactNormal * (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
            }
            else if (desiresClimbing && OnGround)
            {
                velocity += (gravity - contactNormal * (CharacterData.MaxClimbAcceleration * 0.9f)) * Time.deltaTime;
            }
            else
            {
                velocity += gravity * Time.deltaTime;
            }
            body.velocity = velocity;
            ClearState();
        }

        void ClearState()
        {
            lastContactNormal = contactNormal;
            lastSteepNormal = steepNormal;
            lastConnectionVelocity = connectionVelocity;
            groundContactCount = steepContactCount = climbContactCount = 0;
            contactNormal = steepNormal = climbNormal = Vector3.zero;
            connectionVelocity = Vector3.zero;
            previousConnectedBody = connectedBody;
            connectedBody = null;
            submergence = 0f;
        }
        void UpdateState()
        {
            stepsSinceLastGrounded += 1;
            stepsSinceLastJump += 1;
            velocity = body.velocity;
            if (
                CheckClimbing() || CheckSwimming() ||
                OnGround || SnapToGround() || CheckSteepContacts()
            )
            {
                stepsSinceLastGrounded = 0;
                if (stepsSinceLastJump > 1)
                {
                    jumpPhase = 0;
                }
                if (groundContactCount > 1)
                {
                    contactNormal.Normalize();
                }
            }
            else
            {
                contactNormal = upAxis;
            }

            if (connectedBody)
            {
                if (connectedBody.isKinematic || connectedBody.mass >= body.mass)
                {
                    UpdateConnectionState();
                }
            }
        }

        void UpdateConnectionState()
        {
            if (connectedBody == previousConnectedBody)
            {
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

        bool CheckClimbing()
        {
            if (Climbing)
            {
                if (climbContactCount > 1)
                {
                    climbNormal.Normalize();
                    float upDot = Vector3.Dot(upAxis, climbNormal);
                    if (upDot >= minGroundDotProduct)
                    {
                        climbNormal = lastClimbNormal;
                    }
                }
                groundContactCount = 1;
                contactNormal = climbNormal;
                return true;
            }
            return false;
        }

        bool CheckSwimming()
        {
            if (Swimming)
            {
                groundContactCount = 0;
                contactNormal = upAxis;
                return true;
            }
            return false;
        }

        bool SnapToGround()
        {
            if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2 || InWater)
            {
                return false;
            }
            float speed = velocity.magnitude;
            if (speed > CharacterData.MaxSnapSpeed)
            {
                return false;
            }
            if (!Physics.Raycast(
                body.position, -upAxis, out RaycastHit hit,
                CharacterData.ProbeDistance, probeMask, QueryTriggerInteraction.Ignore
            ))
            {
                return false;
            }

            float upDot = Vector3.Dot(upAxis, hit.normal);
            if (upDot < GetMinDot(hit.collider.gameObject.layer))
            {
                return false;
            }

            groundContactCount = 1;
            contactNormal = hit.normal;
            float dot = Vector3.Dot(velocity, hit.normal);
            if (dot > 0f)
            {
                velocity = (velocity - hit.normal * dot).normalized * speed;
            }
            connectedBody = hit.rigidbody;
            return true;
        }

        bool CheckSteepContacts()
        {
            if (steepContactCount > 1)
            {
                steepNormal.Normalize();
                float upDot = Vector3.Dot(upAxis, steepNormal);
                if (upDot >= minGroundDotProduct)
                {
                    steepContactCount = 0;
                    groundContactCount = 1;
                    contactNormal = steepNormal;
                    return true;
                }
            }
            return false;
        }

        void AdjustVelocity()
        {
            float acceleration, speed;
            Vector3 xAxis, zAxis;
            if (Climbing)
            {
                acceleration = CharacterData.MaxClimbAcceleration;
                speed = CharacterData.MaxClimbSpeed;
                xAxis = Vector3.Cross(contactNormal, upAxis);
                zAxis = upAxis;
            }
            else if (InWater)
            {
                float swimFactor = Mathf.Min(1f, submergence / CharacterData.SwimThreshold);
                acceleration = Mathf.LerpUnclamped(
                    OnGround ? CharacterData.MaxAcceleration : CharacterData.MaxAirAcceleration,
                    CharacterData.MaxSwimAcceleration, swimFactor
                );
                speed = Mathf.LerpUnclamped(CharacterData.MaxSpeed, CharacterData.MaxSwimSpeed, swimFactor);
                xAxis = rightAxis;
                zAxis = forwardAxis;
            }
            else
            {
                acceleration = OnGround ? CharacterData.MaxAcceleration : CharacterData.MaxAirAcceleration;
                speed = OnGround && desiresClimbing ? CharacterData.MaxClimbSpeed : CharacterData.MaxSpeed;
                xAxis = rightAxis;
                zAxis = forwardAxis;
            }
            xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
            zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

            Vector3 relativeVelocity = velocity - connectionVelocity;

            Vector3 adjustment;
            adjustment.x =
                playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
            adjustment.z =
                playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
            adjustment.y = Swimming ?
                playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

            adjustment =
                Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

            velocity += xAxis * adjustment.x + zAxis * adjustment.z;
            if (Swimming)
            {
                velocity += upAxis * adjustment.y;
            }
        }

        void Jump(Vector3 gravity)
        {
            Vector3 jumpDirection;
            if (OnGround)
            {
                jumpDirection = contactNormal;
            }
            else if (OnSteep)
            {
                jumpDirection = steepNormal;
                jumpPhase = 0;
            }
            else if (CharacterData.MaxAirJumps > 0 && jumpPhase <= CharacterData.MaxAirJumps)
            {
                if (jumpPhase == 0)
                {
                    jumpPhase = 1;
                }
                jumpDirection = contactNormal;
            }
            else
            {
                return;
            }

            stepsSinceLastJump = 0;
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * CharacterData.JumpHeight);
            if (InWater)
            {
                jumpSpeed *= Mathf.Max(0f, 1f - submergence / CharacterData.SwimThreshold);
            }
            jumpDirection = (jumpDirection + upAxis).normalized;
            float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += jumpDirection * jumpSpeed;
        }

        void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void EvaluateCollision(Collision collision)
        {
            if (Swimming)
            {
                return;
            }
            int layer = collision.gameObject.layer;
            float minDot = GetMinDot(layer);
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(upAxis, normal);
                if (upDot >= minDot)
                {
                    groundContactCount += 1;
                    contactNormal += normal;
                    connectedBody = collision.rigidbody;
                }
                else
                {
                    if (upDot > -0.01f)
                    {
                        steepContactCount += 1;
                        steepNormal += normal;
                        if (groundContactCount == 0)
                        {
                            connectedBody = collision.rigidbody;
                        }
                    }
                    if (
                        desiresClimbing && upDot >= minClimbDotProduct &&
                        (climbMask & (1 << layer)) != 0
                    )
                    {
                        climbContactCount += 1;
                        climbNormal += normal;
                        lastClimbNormal = normal;
                        connectedBody = collision.rigidbody;
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if ((waterMask & (1 << other.gameObject.layer)) != 0)
            {
                EvaluateSubmergence(other);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if ((waterMask & (1 << other.gameObject.layer)) != 0)
            {
                EvaluateSubmergence(other);
            }
        }

        void EvaluateSubmergence(Collider collider)
        {
            if (Physics.Raycast(
                body.position + upAxis * CharacterData.SubmergenceOffset,
                -upAxis, out RaycastHit hit, CharacterData.SubmergenceRange + 1f,
                waterMask, QueryTriggerInteraction.Collide
            ))
            {
                submergence = 1f - hit.distance / CharacterData.SubmergenceRange;
            }
            else
            {
                submergence = 1f;
            }
            if (Swimming)
            {
                connectedBody = collider.attachedRigidbody;
            }
        }


        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }


        float GetMinDot(int layer)
        {
            return (stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
        }
    }
}