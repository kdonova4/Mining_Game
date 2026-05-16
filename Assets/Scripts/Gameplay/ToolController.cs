using Dono.MiningGame.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.HID;
using static UnityEngine.UI.Image;

namespace Dono.MiningGame.Gameplay
{


    [Serializable]
    public struct CrosshairData
    {
        [Tooltip("The image that will be used for this tool's crosshair")]
        public Sprite CrosshairSprite;
        [Tooltip("The size of the crosshair image")]
        public int CrosshairSize;

        [Tooltip("The color of the crosshair image")]
        public Color CrosshairColor;


    }
    [RequireComponent(typeof(AudioSource), typeof(LineRenderer))]
    public class ToolController : MonoBehaviour
    {

        [Tooltip("Max Heat until the gun overheats")]
        public float MaxHeat = 8.0f;

        [Tooltip("Rate the gun cools down and is ready to fire")]
        public float CooldownRate = 2.0f;

        [Tooltip("Beam Range")]
        public float BeamRange = 1f;

        [Tooltip("Mine Rate")]
        public float MineRate = 1f;

        [Tooltip("Delay between Mine Shots")]
        public float DelayBetweenMineShots = 1.0f;

        [Tooltip("Heat Increase Rate")]
        public float HeatIncreaseRate = 1.0f;

        [Tooltip("Tool Camera")]
        public Camera ToolCamera;

        [Tooltip("The Parent Transform")]
        public Transform ParentTransform;

        [Tooltip("The root object for the tool")]
        public GameObject ToolRoot;

        [Tooltip("Tip of Mining Tool")]
        public Transform TipOfTool;

        [Tooltip("The force that will push back the tool while shooting")]
        public float RecoilForce = 100.0f;

        [Tooltip("How fast the weapon goes back to it's original position after the recoil is finished")]
        public float RecoilRestitutionSharpness = 10f;

        [Tooltip("How fast the recoil moves the tool")]
        public float RecoilSharpness = 50f;

        [Tooltip("Max Recoil Distance")]
        public float MaxRecoilDistance = .5f;

        [Header("Weapon Bob")]
        [Tooltip("Frequency at which the weapon will move around in the screen when the player is in movement")]
        public float BobFrequency = 10f;

        [Tooltip("How fast the weapon bob is applied, the bigger value the fastest")]
        public float BobSharpness = 10f;

        [Tooltip("Distance the weapon bobs when not aiming")]
        public float BobAmount = 0.05f;

        [Header("Tool Sway")]
        [Tooltip("The maximum amount the tool can move in any direction")]
        public float MaxSway = 0.5f;

        [Tooltip("The amount to sway when moving the camera")]
        public float SwayAmount = 0.1f;

        [Tooltip("Sway Smoothing")]
        public float SwaySmoothing = 4f;

        [Tooltip("The maximum amount the tool can rotate")]
        public float MaxRotation = 10f;

        [Tooltip("The rotation amount")]
        public float RotationAmount = 1.0f;

        [Tooltip("The rotation speed")]
        public float RotationSpeed = 5.0f;

        [Tooltip("The jump force")]
        public float JumpForce = 1.0f;

        [Tooltip("The Jump Speed")]
        public float JumpDownSpeed = 5.0f;

        [Tooltip("The Jump Return Speed")]
        public float JumpReturnSpeed = 5.0f;

        [Tooltip("The Jump Land Speed")]
        public float JumpLandSpeed = 5.0f;

        [Tooltip("The Jump Land Speed")]
        public float JumpLandForce = 5.0f;

        [Tooltip("Overheat Sound")]
        public AudioClip OverheatSound;

        [Tooltip("Cool down sound")]
        public AudioClip CooldownSound;

        [Tooltip("Shoot sound")]
        public AudioClip ShootSound;

        [Tooltip("Mining Manager")]
        public MiningManager MiningManager;

        [Tooltip("Max items allowed to be held by tool")]
        public int MaxItemsHeld = 10;

        [Tooltip("Where Resources/Items will be held")]
        public GameObject PickupRoot;

        public LineRenderer LineRenderer;

        PlayerCharacterController m_PlayerCharacterController;
        PlayerInputHandler m_InputHandler;
        LineRenderer m_LineRenderer;
        AudioSource m_OneShotAudioSource;
        AudioSource m_ContinuousAudioSource;
        float m_CurrentHeat;
        bool m_IsOverheated;
        bool m_IsShooting;
        float m_MineTimer;
        float m_WeaponBobFactor;
        Vector3 m_BeamDirection;
        Vector3 m_JumpTarget;
        Vector3 m_ToolRecoilLocalPosition;
        Vector3 m_LastCharacterPostition;
        Vector3 m_ToolMainLocalPosition;
        Vector3 m_ToolBobLocalPosition;
        Vector3 m_AccumulatedRecoil;
        Vector3 m_MineTarget;
        Vector3 m_SwayLocalPosition;
        Quaternion m_SwayLocalRotation;
        Quaternion m_ToolLocalRotation;
        Vector3 m_ToolJumpImpulseLocalPosition;
        Vector3 m_ToolLandImpulseLocalPosition;
        Quaternion m_ToolOverheatedLocalRotation;
        bool m_WasShooting;
        bool m_IsReturning;
        bool m_Landing;
        float m_PushForce;
        float m_ChargeTime = 1f;
        float m_MaxCharge = 1f;
        float m_ChargeSpeed = 1f;
        float m_LastReleasedForce;
        bool m_JustPushed = false;

        void Awake()
        {
            m_BeamDirection = ToolCamera.transform.forward;
            m_ToolMainLocalPosition = ParentTransform.localPosition;
            m_ToolLocalRotation = ParentTransform.localRotation;
            m_ToolOverheatedLocalRotation = ParentTransform.localRotation;
            m_MineTimer = MineRate;
            m_IsShooting = false;
            m_IsOverheated = false;
            m_IsReturning = false;
            m_Landing = false;
            m_CurrentHeat = 0f;
            m_LineRenderer = GetComponent<LineRenderer>();
            DebugUtility.HandleErrorIfNullGetComponent<LineRenderer, ToolController>(m_LineRenderer, this, gameObject);

            LineRenderer.positionCount = 2;
            LineRenderer.startWidth = 0.2f;
            LineRenderer.endWidth = 0.2f;




            m_OneShotAudioSource = gameObject.AddComponent<AudioSource>();
            m_OneShotAudioSource.playOnAwake = false;

            m_ContinuousAudioSource = gameObject.AddComponent<AudioSource>();
            m_ContinuousAudioSource.playOnAwake = false;
            m_ContinuousAudioSource.clip = ShootSound;
            m_ContinuousAudioSource.loop = true;
            m_ContinuousAudioSource.outputAudioMixerGroup =
                    AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.ToolChargeLoop);

            m_InputHandler = GetComponentInParent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, ToolController>(m_InputHandler, this, gameObject);

            m_PlayerCharacterController = GetComponentInParent<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, ToolController>(m_PlayerCharacterController, this, gameObject);

        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            m_JustPushed = m_InputHandler.GetFireInputDown() && m_InputHandler.GetAimInputHeld();
            HandlePushCharge();
            HandleTractorBeam();
            if (Physics.Raycast(ToolCamera.transform.position, ToolCamera.transform.forward, out RaycastHit hit, 1000000f))
            {
                m_MineTarget = hit.point;
            }
            HandleInput();
            if (m_IsShooting && !m_WasShooting)
            {
                // reset beam state when starting a new shot
                m_BeamDirection = ToolCamera.transform.forward;
            }
            m_WasShooting = m_IsShooting;
            HandleHeat();
            HandleMiningBeam();

            if (m_IsShooting)
            {
                m_AccumulatedRecoil += Vector3.back * RecoilForce;
                m_AccumulatedRecoil = Vector3.ClampMagnitude(m_AccumulatedRecoil, MaxRecoilDistance);
            }


        }

        void LateUpdate()
        {
            HandleOverheatPose();
            HandleJumpImpulse();
            HandleJumpLandImpulse();
            HandleWeaponSway();
            HandleWeaponBob();
            HandleRecoil();
            HandleBeamRender();
            ParentTransform.localPosition = m_ToolMainLocalPosition + m_SwayLocalPosition + m_ToolBobLocalPosition + m_ToolRecoilLocalPosition
                + m_ToolJumpImpulseLocalPosition + m_ToolLandImpulseLocalPosition;
            ParentTransform.localRotation = m_ToolLocalRotation * m_SwayLocalRotation * m_ToolOverheatedLocalRotation;

            //Debug.Log(ParentTransform.localPosition);
            //.Log(m_ToolRecoilLocalPosition);
        }

        void FixedUpdate()
        {

        }

        void HandleInput()
        {
            bool wantsToShoot = m_InputHandler.GetFireInputHeld() && !m_InputHandler.GetAimInputHeld();


            if (wantsToShoot && !m_IsOverheated)
            {
                if (!m_IsShooting)
                {
                    m_IsShooting = true;

                    m_ContinuousAudioSource.Play();
                }
            }
            else
            {
                if (m_IsShooting)
                {
                    m_IsShooting = false;

                    m_ContinuousAudioSource.Stop();
                }
            }
        }

        void HandleHeat()
        {
            if (m_IsShooting)
            {
                m_CurrentHeat += HeatIncreaseRate * Time.deltaTime;

                if (m_CurrentHeat >= MaxHeat)
                {
                    m_CurrentHeat = MaxHeat;
                    m_IsOverheated = true;
                    m_IsShooting = false;

                    m_ContinuousAudioSource.Stop();
                    m_OneShotAudioSource.PlayOneShot(OverheatSound);
                    m_OneShotAudioSource.PlayOneShot(CooldownSound);
                }

            }
            else
            {
                if (m_IsOverheated)
                {
                    m_CurrentHeat -= CooldownRate * Time.deltaTime;

                    if (m_CurrentHeat <= 0f)
                    {
                        m_CurrentHeat = 0f;
                        m_IsOverheated = false;

                        m_OneShotAudioSource.Stop();
                    }
                }
                else
                {
                    m_CurrentHeat -= CooldownRate * Time.deltaTime;
                    m_CurrentHeat = Mathf.Clamp(m_CurrentHeat, 0, MaxHeat);
                }
            }
        }

        void HandleMiningBeam()
        {
            if (m_IsShooting && !m_IsOverheated)
            {
                m_MineTimer -= Time.deltaTime;

                if (m_MineTimer <= 0f)
                {
                    m_MineTimer = MineRate;

                    if (Physics.Raycast(ToolCamera.transform.position, ToolCamera.transform.forward, out RaycastHit hit, BeamRange))
                    {
                        //Debug.DrawLine(ToolCamera.transform.position, hit.point, Color.red, BeamRange);
                        //Debug.Log("Hit: " + hit.collider.name);

                        m_MineTarget = hit.point;
                        ResourceSpawner resourceSpawner = hit.collider.GetComponent<ResourceSpawner>();
                        if (resourceSpawner != null)
                        {

                            int amountMined = MiningManager.MineResult();
                            resourceSpawner.SpawnOre(amountMined);
                            //Debug.Log("MINE");
                        }
                        else
                        {
                            //Debug.Log("DID NOT HIT");
                        }
                    }


                }


            }
            else
            {
                m_MineTimer = MineRate;
            }
        }

        void HandleRecoil()
        {

            if (m_ToolRecoilLocalPosition.z >= m_AccumulatedRecoil.z * 0.99f)
            {
                m_ToolRecoilLocalPosition = Vector3.Lerp(m_ToolRecoilLocalPosition, m_AccumulatedRecoil, RecoilSharpness * Time.deltaTime);
            }
            else
            {
                m_ToolRecoilLocalPosition = Vector3.Lerp(m_ToolRecoilLocalPosition, Vector3.zero, RecoilRestitutionSharpness * Time.deltaTime);
                m_AccumulatedRecoil = m_ToolRecoilLocalPosition;
            }
        }



        void HandleBeamRender()
        {
            if (!m_IsShooting)
            {

                LineRenderer.enabled = false;
                return;
            }

            LineRenderer.enabled = true;

            Vector3 start = TipOfTool.position;

            float beamResponsiveness = 15f;

            Vector3 cameraDir = ToolCamera.transform.forward;

            // optional: lock aim per shot (prevents strafe jitter entirely)
            m_BeamDirection = Vector3.Slerp(
                m_BeamDirection,
                cameraDir,
                beamResponsiveness * Time.deltaTime
            );

            Vector3 rayOrigin = ToolCamera.transform.position;

            Vector3 end;

            if (Physics.Raycast(rayOrigin, m_BeamDirection, out RaycastHit hit, BeamRange))
            {
                end = hit.point;
            }
            else
            {
                end = rayOrigin + m_BeamDirection * BeamRange;
            }


            LineRenderer.SetPosition(0, start);
            LineRenderer.SetPosition(1, end);
        }


        void HandleWeaponBob()
        {
            if (Time.deltaTime > 0f)
            {
                Vector3 PlayerCharacterVelocity =
                    (m_PlayerCharacterController.transform.position - m_LastCharacterPostition) / Time.deltaTime;

                // calculate a smoothed weapon bob amount based on how close to our max gounded movement velocity we are
                float characterMovementFactor = 0f;
                if (m_PlayerCharacterController.IsGrounded)
                {
                    characterMovementFactor =
                        Mathf.Clamp01(PlayerCharacterVelocity.magnitude /
                        (m_PlayerCharacterController.MaxSpeedOnGround *
                        m_PlayerCharacterController.SprintSpeedModifier));
                }

                m_WeaponBobFactor =
                    Mathf.Lerp(m_WeaponBobFactor, characterMovementFactor, BobSharpness * Time.deltaTime);

                // Calculatye vertical and horizontal weapon bob values based on a sine function
                float bobAmount = BobAmount;
                float frequency = BobFrequency;
                float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * m_WeaponBobFactor;
                float vBobValue = Mathf.Sin((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * m_WeaponBobFactor;

                // apply weapon bob
                m_ToolBobLocalPosition.x = hBobValue;
                m_ToolBobLocalPosition.y = Math.Abs(vBobValue);

                m_LastCharacterPostition = m_PlayerCharacterController.transform.position;
            }
        }

        void HandleWeaponSway()
        {
            float mouseX = m_InputHandler.GetLookInputsHorizontal();
            float mouseY = m_InputHandler.GetLookInputsVertical();

            float moveX = -mouseX * SwayAmount;
            float moveY = mouseY * SwayAmount;

            float rotateX = -mouseY * RotationAmount;
            float rotateY = -mouseX * RotationAmount;



            moveX = Mathf.Clamp(moveX, -MaxSway, MaxSway);
            moveY = Mathf.Clamp(moveY, -MaxSway, MaxSway);
            rotateX = Mathf.Clamp(rotateX, -MaxRotation, MaxRotation);
            rotateY = Mathf.Clamp(rotateY, -MaxRotation, MaxRotation);

            Vector3 targetSwayPos = new Vector3(moveX, moveY, 0);
            Vector3 targetSwayRot = new Vector3(rotateX, rotateY, 0);


            Quaternion targetRotation = Quaternion.Euler(targetSwayRot);

            m_SwayLocalRotation = Quaternion.Lerp(m_SwayLocalRotation, targetRotation, RotationSpeed * Time.deltaTime);
            m_SwayLocalPosition = Vector3.Lerp(m_SwayLocalPosition, targetSwayPos, SwaySmoothing * Time.deltaTime);
        }

        void HandleOverheatPose()
        {
            if (m_IsOverheated)
            {
                Vector3 targetRotation = new Vector3(0, -10, -10);

                Quaternion targetQaut = Quaternion.Euler(targetRotation);

                m_ToolOverheatedLocalRotation = Quaternion.Lerp(m_ToolOverheatedLocalRotation, targetQaut, RotationSpeed * Time.deltaTime);
            }
            else
            {
                m_ToolOverheatedLocalRotation = Quaternion.Lerp(m_ToolOverheatedLocalRotation, Quaternion.Euler(Vector3.zero), RotationSpeed * Time.deltaTime);
            }
        }

        bool m_IsFalling;

        void HandleJumpImpulse()
        {

            if (!m_PlayerCharacterController.IsGrounded && !m_IsFalling)
            {
                m_Landing = true;
            }

            // 2. ONLY update the target when you jump
            if ((m_InputHandler.GetJumpInputDown() && !m_PlayerCharacterController.IsGrounded && m_PlayerCharacterController.HasJumpedThisFrame))
            {
                m_IsFalling = true;
                m_Landing = true;
                m_IsReturning = false;
                m_JumpTarget = new Vector3(0, -JumpForce, 0);
            }

            // 3. Lerp toward the persistent target
            m_ToolJumpImpulseLocalPosition = Vector3.Lerp(
                m_ToolJumpImpulseLocalPosition,
                m_JumpTarget,
                (m_IsReturning ? JumpReturnSpeed : JumpDownSpeed) * Time.deltaTime
            );

            // 4. Once we reach the bottom, set the target back to zero so it starts coming back up
            if (Vector3.Distance(m_ToolJumpImpulseLocalPosition, m_JumpTarget) < 0.01f)
            {
                m_IsReturning = true;
                m_JumpTarget = Vector3.zero;
            }
        }


        void HandleJumpLandImpulse()
        {
            // 2. ONLY update the target when you jump
            if (m_PlayerCharacterController.IsGrounded && m_Landing)
            {
                m_IsFalling = false;
                m_Landing = false;
                m_JumpTarget = new Vector3(0, JumpLandForce, 0);
            }

            // 3. Lerp toward the persistent target
            m_ToolLandImpulseLocalPosition = Vector3.Lerp(
                m_ToolLandImpulseLocalPosition,
                m_JumpTarget,
                JumpLandSpeed * Time.deltaTime
            );

            // 4. Once we reach the bottom, set the target back to zero so it starts coming back up
            if (Vector3.Distance(m_ToolLandImpulseLocalPosition, m_JumpTarget) < 0.01f)
            {
                m_JumpTarget = Vector3.zero;
            }
        }

        Dictionary<Rigidbody, ConfigurableJoint> heldBodies = new Dictionary<Rigidbody, ConfigurableJoint>();
        float m_GrabCooldownTime = 0.0f;
        void HandleTractorBeam()
        {

            bool IsAiming = m_InputHandler.GetAimInputHeld();

            if (m_GrabCooldownTime > 0)
            {
                m_GrabCooldownTime -= Time.deltaTime;
            }

            if (m_JustPushed && heldBodies.Count > 0)
            {
                ReleaseAndPush();
                m_GrabCooldownTime = 0.5f;
                return;
            }

            if (IsAiming && m_GrabCooldownTime <= 0 && !m_IsOverheated)
            {

                Collider[] hitColliders = Physics.OverlapCapsule(ToolCamera.transform.position, ToolCamera.transform.position + (ToolCamera.transform.forward * 1.5f), .5f);

                foreach (var hitCollider in hitColliders)
                {
                    Rigidbody rb = hitCollider.GetComponent<Rigidbody>();

                    if (rb != null && heldBodies.GetValueOrDefault(rb) == null && heldBodies.Count < MaxItemsHeld)
                    {
                        hitCollider.gameObject.layer = LayerMask.NameToLayer("Held");
                        rb.useGravity = false;

                        ConfigurableJoint newJoint = CreateJoint(rb);
                        heldBodies.Add(rb, newJoint);

                    }
                }
            }
            else if (!IsAiming && heldBodies.Count > 0)
            {
                ReleaseAndDrop();
            }
        }

        void ReleaseAndDrop()
        {
            foreach (KeyValuePair<Rigidbody, ConfigurableJoint> pair in heldBodies)
            {
                Rigidbody rb = pair.Key;
                if (pair.Value != null)
                {
                    pair.Value.connectedBody = null;
                    Destroy(pair.Value);
                }
                if (rb != null)
                {
                    rb.gameObject.layer = LayerMask.NameToLayer("Resource");
                    rb.useGravity = true;
                    rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 10f);
                }

            }
            heldBodies.Clear();
        }


        // Separate these into methods to keep your code readable!
        void ReleaseAndPush()
        {

            foreach (KeyValuePair<Rigidbody, ConfigurableJoint> pair in heldBodies)
            {

                Rigidbody rb = pair.Key;
                if (pair.Value != null)
                {
                    pair.Value.connectedBody = null;
                    Destroy(pair.Value);
                }

                if (rb != null)
                {
                    rb.gameObject.layer = LayerMask.NameToLayer("Resource");
                    rb.useGravity = true;
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(TipOfTool.transform.forward * (m_LastReleasedForce * 10f), ForceMode.VelocityChange);
                }

            }
            heldBodies.Clear();
        }

        ConfigurableJoint CreateJoint(Rigidbody rb)
        {
            // Configure and create joint
            ConfigurableJoint joint = PickupRoot.AddComponent<ConfigurableJoint>();

            joint.connectedBody = rb;

            joint.autoConfigureConnectedAnchor = false;


            Vector3 connectedAnchor = new Vector3(UnityEngine.Random.Range(-.5f, .5f), UnityEngine.Random.Range(-.5f, .5f), UnityEngine.Random.Range(-.5f, .5f));
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = Vector3.zero;


            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;

            var linearLimitSpring = joint.linearLimitSpring;
            linearLimitSpring.spring = 0f;
            linearLimitSpring.damper = 5f;
            joint.linearLimitSpring = linearLimitSpring;

            var linearLimit = joint.linearLimit;
            linearLimit.limit = .1f;
            linearLimit.bounciness = 0;
            linearLimit.contactDistance = 0.01f;
            joint.linearLimit = linearLimit;



            var xDrive = joint.xDrive;
            xDrive.positionSpring = UnityEngine.Random.Range(300f, 350f);
            xDrive.positionDamper = UnityEngine.Random.Range(40f, 150f);
            xDrive.useAcceleration = true;
            var yDrive = joint.yDrive;
            yDrive.positionSpring = UnityEngine.Random.Range(300f, 450f);
            yDrive.positionDamper = UnityEngine.Random.Range(40f, 150f);
            yDrive.useAcceleration = true;
            var zDrive = joint.zDrive;
            zDrive.positionSpring = UnityEngine.Random.Range(300f, 450f);
            zDrive.positionDamper = UnityEngine.Random.Range(5f, 8f);
            zDrive.useAcceleration = true;

            joint.xDrive = xDrive;
            joint.yDrive = yDrive;
            joint.zDrive = zDrive;

            joint.rotationDriveMode = RotationDriveMode.Slerp;



            joint.projectionMode = JointProjectionMode.PositionAndRotation;

            return joint;
        }


        void HandlePushCharge()
        {
            if (m_JustPushed)
            {

                m_LastReleasedForce = 1f;
            }
        }

        private void OnDrawGizmos()
        {
            if (ToolCamera == null) return;
            Gizmos.color = Color.blue;
            Vector3 p1 = ToolCamera.transform.position;
            Vector3 p2 = ToolCamera.transform.position + (ToolCamera.transform.forward * 1.5f);
            float radius = .5f;

            Gizmos.DrawWireSphere(p1, radius);
            Gizmos.DrawWireSphere(p2, radius);

            Vector3 up = ToolCamera.transform.up * radius;
            Vector3 right = ToolCamera.transform.right * radius;

            Gizmos.DrawLine(p1 + up, p2 + up);
            Gizmos.DrawLine(p1 - up, p2 - up);
            Gizmos.DrawLine(p1 + right, p2 + right);
            Gizmos.DrawLine(p1 - right, p2 - right);

        }

    }
}

