﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Character's Health")]
        public float Health;
        public float MaxHealth;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public float sensitivity = 1f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.5f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        public int currencyCounter = 0;
        public int colCurrency = 0;
        public int forCurrency = 0;
        public int facCurrency = 0;
        public int rumCurrency = 0;
        public GameObject WeaponShopUI;


        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDRun;
        private int _animIDShoot;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        public Animator _animator;
        private CharacterController _controller;
        public StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private HealingOverTime healingOverTime;

        [SerializeField]
        private CinemachineVirtualCamera aimCamera;

        private const float _threshold = 0.01f;

        private Vector2 lastMoveDir;

        private bool is_dead = false;

        private bool canReload = false;

        private bool _hasAnimator;

        public bool reload = false;

        public bool melee = false;

        private bool startRound = false;

        private bool falling = false;

        private int nextRound = 1;

        private bool rotateWhenMoving = true;

        private bool training;
        private bool rotated = false;

        private bool canFire = true;
        private float fireTimer = 0f;
        //private float shootingSpread = 15f;

        private Trigger trigger;
        private ArenaTrigger arenaTrigger;

        public WaveSpawner waveSpawner;
        public RumbleSpawner rumbleSpawner = null;
        private TrainingTargetSpawner trainingSpawner = null;

        [SerializeField]
        private Slider healthBar;

        [SerializeField]
        private TextMeshProUGUI healthBarText;

        [SerializeField]
        private Text currencyAmountUI;

        [SerializeField]
        private Text colCurrencyAmountUI;

        [SerializeField]
        private Text facCurrencyAmountUI;

        [SerializeField]
        private Text forCurrencyAmountUI;

        [SerializeField]
        private Text rumCurrencyAmountUI;

        private Vector2 screenCenter;

        [SerializeField]
        private LayerMask aimColliderMask;

        [SerializeField]
        public GameObject blade;

        [SerializeField]
        private GunBehaviour PS;

        [SerializeField]
        private GunBehaviour AR;

        [SerializeField]
        private GunBehaviour SG;

        [SerializeField]
        private GunBehaviour RL;

        private List<GunBehaviour> gunArsenal = new List<GunBehaviour>();

        private int selectedGun = 0;

        [SerializeField]
        private GameObject bloodOverlay;

        private ArenaTrigger onDeathTrigger;
        private bool foundArenaTrigger = false;

        private bool playerGotHit = false;
        private bool heartBeat = false, heartBeatUp = true;
        private float heartBeatRatio = 1.0f;
        private int hitsNumber = 0;
        private int weaponCurrency;
        private float fallingY = -1234.56789f;
        private float teleportedTime = 0f;
        private bool recentlyTeleported = false;
        public bool changingScene = false;

        public Vector3 bladeOffset = new Vector3(0.0114f, -0.045f, 0.0053f);

        private string jumpColliseumBase = "jump_coliseu_";
        private string jumpHubBase = "jump_hub_";
        private string jumpFactoryBase = "jump_factory_";
        private string jumpForestBase = "jump_newworld_";
        private string jumpBase;

        private GameObject[] rumblePlanes;

        GameObject deathScreen;
        Transform arenaPrompt;

        MeshRenderer ARMesh = null;
        MeshRenderer SGMesh = null;
        MeshRenderer RLMesh = null;

        [SerializeField]
        private BackgroundMusicPlayer musicPlayer;

        [SerializeField]
        private GameObject TrainingHUD;

        private Transform WeaponsHUD;

        // Sound Variables

        private FMOD.Studio.EventInstance heartBeatSound;
        private bool walking = false;

        private bool jumping = false;

        private FMOD.Studio.PLAYBACK_STATE pbState;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {

            screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            healingOverTime = GetComponent<HealingOverTime>();
            //walkSound = FMODUnity.RuntimeManager.CreateInstance("event:/Project/General Sounds/Character Related/Footsteps/Grass");
            lastMoveDir = new Vector2(0f, 0f);
            //string[] names = {"Default", "Enemy"};
            //aimColliderMask = LayerMask.GetMask(names);

            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            // starting values
            Health = GetStartingHealth();
            MaxHealth = GetStartingMaxHealth();
            weaponCurrency = GetStartingMoney();
            if (SceneManager.GetActiveScene().name != "Hub")
            {
                UpdateHealthUI();
                updateCurrencyUI();
                AdaptToWeaponLevels();
            }

            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            //AssignAnimationIDs();
            _animator.SetBool("Pistol", true);
            // find the canvas
            GameObject canvas = GameObject.Find("Canvas");
            // get the DeathScreen object from the canvas
            deathScreen = canvas.transform.Find("DeathScreen").gameObject;
            GameObject wavespawn = GameObject.Find("WaveSpawner");
            if (wavespawn != null)
            {
                waveSpawner = wavespawn.GetComponent<WaveSpawner>();
            }
            else
            {
                waveSpawner = null;
                GameObject rumblespawn = GameObject.Find("RumbleSpawner");
                if (rumblespawn != null)
                {
                    rumbleSpawner = rumblespawn.GetComponent<RumbleSpawner>();
                }
            }
            GameObject trainSpawn = GameObject.Find("TrainingSpawner");
            if (trainSpawn != null)
            {
                trainingSpawner = trainSpawn.GetComponent<TrainingTargetSpawner>();
            }
            WeaponsHUD = canvas.transform.Find("ReloadHUD");
            arenaPrompt = canvas.transform.Find("ArenaPrompt");
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            //PS.ApplyUpgrades() // should apply upgrades before adding to the arsenal
            //AR.ApplyUpgrades()
            //SG.ApplyUpgrades()
            //RL.ApplyUpgrades()

            gunArsenal.Add(PS);
            PS.setGunSound("shot_gun2");
            AR.setGunSound("shot_gun1");
            SG.setGunSound("shot_gun3");
            RL.setGunSound("explosion_gun4");
            if (rumbleSpawner != null)
            {
                gunArsenal.Add(AR);
                selectedGun = gunArsenal.Count - 1;
                addGunHUD("AR");
                gunArsenal.Add(SG);
                selectedGun = gunArsenal.Count - 1;
                addGunHUD("SG");
                gunArsenal.Add(RL);
                selectedGun = gunArsenal.Count - 1;
                addGunHUD("RL");
                selectedGun = 0;
            }
            gunArsenal[selectedGun].gameObject.SetActive(true);
            // find all objects with the tag RumblePlane
            rumblePlanes = GameObject.FindGameObjectsWithTag("RumblePlane");
            // select one of the rumble planes to spawn the player at
            if (rumblePlanes != null && rumblePlanes.Length != 0)
            {
                int random = Random.Range(0, rumblePlanes.Length);
                // spawn the player at the selected rumble plane
                transform.position = rumblePlanes[random].gameObject.GetComponent<RumblePlane>().getSpawnPosition();
                rumbleSpawner.setCurrentScene(rumblePlanes[random].name);
            }

            heartBeatSound = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Character Related/heartbeat");
            jumpBase = jumpHubBase;
            switch (SceneManager.GetActiveScene().name)
            {
                case "Colliseum":
                    jumpBase = jumpColliseumBase;
                    break;
                case "Hub":
                    jumpBase = jumpHubBase;
                    break;
                case "Factory":
                    jumpBase = jumpFactoryBase;
                    break;
                case "Forest":
                    jumpBase = jumpForestBase;
                    break;
            }
        }

        private void Update()
        {

            if (SceneManager.GetActiveScene().name == "Colliseum"){
                Cursor.lockState = CursorLockMode.None;
            }
            _hasAnimator = TryGetComponent(out _animator);

            Debug.Log("Before JumpAndGravity!");
            JumpAndGravity();
            Debug.Log("Before GroundedCheck!");
            GroundedCheck();
            Debug.Log("Before Move!");
            Move();
            Debug.Log("Before Aim!");
            Aim();
            Debug.Log("Before Fire!");
            Fire();
            Debug.Log("Before Melee!");
            Melee();
            if (trainingSpawner != null)
            {
                if (!trainingSpawner.active)
                {
                    training = false;
                }

                if (!training && TrainingHUD.activeSelf)
                {
                    if (_input.interact)
                    {
                        trainingSpawner.startTraining();
                        _input.interact = false;
                        training = true;
                        TrainingHUD.SetActive(false);
                    }
                }
            }
            Debug.Log("Before Weaponshop!");
            WeaponShop();
            Debug.Log("Before Interact!");
            Interact();
            Debug.Log("Before SwitchWeapon!");
            SwitchWeapon();
            Debug.Log("Before StartRound!");
            StartRound();
            Debug.Log("Before Reload!");
            Reload();
            Point();
            Click();

            if (gunArsenal[selectedGun].reloading)
            {
                SprintSpeed = 2.0f;
            }


            /*if (_animator.GetCurrentAnimatorStateInfo(0).IsName("ReloadWalking") && _animator.GetCurrentAnimatorClipInfo){
                _animator.SetBool("Reloading", false);
                SprintSpeed = 5.335f;
            }*/

            if (recentlyTeleported && Time.time - teleportedTime > 0.5f)
            {
                for (int i = 0; i < rumblePlanes.Length; i++)
                {
                    rumblePlanes[i].gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }
                recentlyTeleported = false;
            }


            if (!canFire)
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= 0.3f)
                {

                    canFire = true;
                    fireTimer = 0;
                }
            }
            if (playerGotHit)
            {
                Color temp = bloodOverlay.GetComponent<Image>().color;
                temp.a -= 0.005f;
                bloodOverlay.GetComponent<Image>().color = temp;

                if (temp.a < 0)
                {
                    playerGotHit = false;
                    bloodOverlay.SetActive(false);
                    hitsNumber--;
                }
            }

            if (heartBeat)
            {
                if (Health > 20)
                {
                    heartBeat = false;
                    heartBeatSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    heartBeatSound.release();
                    playerGotHit = true;
                    return;
                }

                if (heartBeatRatio >= 1.0f || heartBeatRatio <= 0.0f)
                {
                    heartBeatUp = !heartBeatUp;
                }

                if (heartBeatUp)
                {
                    heartBeatRatio += 0.01f;
                }
                else
                {
                    heartBeatRatio -= 0.01f;
                }

                heartBeatSound.getPlaybackState(out pbState);
                if (pbState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                {
                    heartBeatSound.start();
                    heartBeatSound.release();
                }

                Color temp = bloodOverlay.GetComponent<Image>().color;
                bloodOverlay.GetComponent<Image>().color = temp;
                bloodOverlay.GetComponent<RectTransform>().localScale = new Vector3(1.0f + Health / 100.0f + heartBeatRatio, 1.0f + Health / 100.0f + heartBeatRatio * 3, 1);
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        /*private void AssignAnimationIDs()
        {
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDRun = Animator.StringToHash("Run");
            _animIDShoot = Animator.StringToHash("Shoot");
        }*/

        private void addGunHUD(string weapon)
        {
            string objectToActivate = weapon + (selectedGun + 1).ToString();
            WeaponsHUD.Find(objectToActivate).gameObject.SetActive(true);
            WeaponsHUD.Find("Pos" + (selectedGun + 1).ToString() + "Text").gameObject.SetActive(true);
        }

        private void WeaponShop()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            GameObject UItext = WeaponShopUI.transform.GetChild(0).gameObject;
            if (Physics.Raycast(ray, out hit, 9f, aimColliderMask))
            {
                if (hit.collider.gameObject.name == "AR")
                {
                    ARMesh = hit.collider.gameObject.GetComponent<MeshRenderer>();

                    if (!gunArsenal.Contains(AR))
                    {
                        if (weaponCurrency >= AR.weaponPrice)
                        {
                            UItext.GetComponent<Text>().text = "Press [E] to buy Tango-45 AR (Cost: " + AR.weaponPrice + ")";
                            WeaponShopUI.SetActive(true);
                            ARMesh.material.color = Color.green;
                            if (_input.interact)
                            {
                                gunArsenal.Add(AR);
                                weaponCurrency -= AR.weaponPrice;
                                gunArsenal[selectedGun].gameObject.SetActive(false);
                                selectedGun = gunArsenal.Count - 1;
                                gunArsenal[selectedGun].gameObject.SetActive(true);
                                addGunHUD("AR");
                                //if the current scene is not "Hub", update the currency ui
                                if (SceneManager.GetActiveScene().name != "Hub")
                                {
                                    updateCurrencyUI();
                                }
                                _animator.SetBool("Pistol", false);
                            }
                        }
                        else
                        {
                            UItext.GetComponent<Text>().text = "Not enough currency. (Cost: " + AR.weaponPrice + ")";
                            WeaponShopUI.SetActive(true);
                            ARMesh.material.color = Color.red;
                        }
                    }
                    else
                    {
                        UItext.GetComponent<Text>().text = "Already acquired Tango-45 AR";
                        WeaponShopUI.SetActive(true);
                        ARMesh.material.color = Color.red;
                    }
                }
                else if (hit.collider.gameObject.name == "ARAmmo")
                {

                    if (weaponCurrency >= AR.ammoPrice & gunArsenal.Contains(AR))
                    {
                        UItext.GetComponent<Text>().text = "Press [E] to buy Tango-45 AR Ammo (Cost: " + AR.ammoPrice + ")";
                        WeaponShopUI.SetActive(true);
                        if (_input.interact)
                        {
                            if (AR.BuyAmmo(1))
                            {
                                _input.interact = false;
                                weaponCurrency -= AR.ammoPrice;
                                if (SceneManager.GetActiveScene().name != "Hub")
                                {
                                    updateCurrencyUI();
                                }
                            }
                        }
                    }
                    else
                    {
                        UItext.GetComponent<Text>().text = "Tango-45 AR not acquired";
                        WeaponShopUI.SetActive(true);
                    }
                }
                else if (hit.collider.gameObject.name == "SG")
                {
                    SGMesh = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    if (!gunArsenal.Contains(SG))
                    {
                        if (weaponCurrency >= SG.weaponPrice)
                        {
                            UItext.GetComponent<Text>().text = "Press [E] to buy Baligant XU772 SG (Cost: " + SG.weaponPrice + ")";
                            WeaponShopUI.SetActive(true);
                            SGMesh.material.color = Color.green;
                            if (_input.interact)
                            {
                                gunArsenal.Add(SG);
                                weaponCurrency -= SG.weaponPrice;
                                gunArsenal[selectedGun].gameObject.SetActive(false);
                                selectedGun = gunArsenal.Count - 1;
                                gunArsenal[selectedGun].gameObject.SetActive(true);
                                addGunHUD("SG");
                                if (SceneManager.GetActiveScene().name != "Hub")
                                {
                                    updateCurrencyUI();
                                }
                                _animator.SetBool("Pistol", false);
                            }
                        }
                        else
                        {
                            UItext.GetComponent<Text>().text = "Not enough currency. (Cost: " + SG.weaponPrice + ")";
                            WeaponShopUI.SetActive(true);
                            SGMesh.material.color = Color.red;
                        }
                    }
                    else
                    {
                        UItext.GetComponent<Text>().text = "Already acquired Baligant XU772 SG";
                        WeaponShopUI.SetActive(true);
                        SGMesh.material.color = Color.red;
                    }
                }
                else if (hit.collider.gameObject.name == "SGAmmo")
                {
                    if (weaponCurrency >= SG.ammoPrice & gunArsenal.Contains(SG))
                    {
                        UItext.GetComponent<Text>().text = "Press [E] to buy Baligant XU772 SG Ammo (Cost: " + SG.ammoPrice + ")";
                        WeaponShopUI.SetActive(true);
                        if (_input.interact)
                        {
                            _input.interact = false;
                            if (SG.BuyAmmo(1))
                            {
                                weaponCurrency -= SG.ammoPrice;
                                if (SceneManager.GetActiveScene().name != "Hub")
                                {
                                    updateCurrencyUI();
                                }
                            }
                        }
                    }
                    else
                    {
                        UItext.GetComponent<Text>().text = "Baligant XU772 SG not acquired";
                        WeaponShopUI.SetActive(true);
                    }
                }
                else if (hit.collider.gameObject.name == "RL")
                {
                    RLMesh = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    if (!gunArsenal.Contains(RL))
                    {
                        if (weaponCurrency >= RL.weaponPrice)
                        {
                            UItext.GetComponent<Text>().text = "Press [E] to buy Bazooka M270 RL (Cost: " + RL.weaponPrice + ")";
                            WeaponShopUI.SetActive(true);
                            RLMesh.material.color = Color.green;
                            if (_input.interact)
                            {
                                gunArsenal.Add(RL);
                                weaponCurrency -= AR.weaponPrice;
                                gunArsenal[selectedGun].gameObject.SetActive(false);
                                selectedGun = gunArsenal.Count - 1;
                                gunArsenal[selectedGun].gameObject.SetActive(true);
                                addGunHUD("RL");
                                if (SceneManager.GetActiveScene().name != "Hub")
                                {
                                    updateCurrencyUI();
                                }
                                _animator.SetBool("Pistol", false);
                            }
                        }
                        else
                        {
                            UItext.GetComponent<Text>().text = "Not enough currency. (Cost: " + RL.weaponPrice + ")";
                            WeaponShopUI.SetActive(true);
                            RLMesh.material.color = Color.red;
                        }
                    }
                    else
                    {
                        UItext.GetComponent<Text>().text = "Bazooka M270 RL already acquired";
                        WeaponShopUI.SetActive(true);
                        RLMesh.material.color = Color.red;
                    }
                }
                else if (hit.collider.gameObject.name == "RLAmmo")
                {
                    if (weaponCurrency >= RL.ammoPrice & gunArsenal.Contains(RL))
                    {
                        UItext.GetComponent<Text>().text = "Press [E] to buy Baligant XU772 SG Ammo (Cost: " + SG.ammoPrice + ")";
                        WeaponShopUI.SetActive(true);
                        if (_input.interact)
                        {
                            _input.interact = false;
                            if (RL.BuyAmmo(1))
                            {
                                weaponCurrency -= RL.ammoPrice;
                                if (SceneManager.GetActiveScene().name != "Hub")
                                {
                                    updateCurrencyUI();
                                }
                            }
                        }
                    }
                    else
                    {
                        UItext.GetComponent<Text>().text = "Baligant XU772 SG not acquired";
                        WeaponShopUI.SetActive(true);
                    }
                }
                else
                {
                    if (ARMesh != null)
                    {
                        ARMesh.material.color = AR.GetComponent<MeshRenderer>().material.color;
                    }
                    if (SGMesh != null)
                    {
                        SGMesh.material.color = SG.GetComponent<MeshRenderer>().material.color;
                    }
                    if (RLMesh != null)
                    {
                        RLMesh.material.color = RL.GetComponent<MeshRenderer>().material.color;
                    }
                    WeaponShopUI.SetActive(false);
                }
            }
            foreach (Transform child in WeaponsHUD)
            {
                if (child.gameObject.name.Contains("Icon"))
                {
                    if (child.gameObject.name == "Icon" + gunArsenal[selectedGun].gameObject.name)
                    {
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void GroundedCheck()
        {
            /*
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                //_animator.SetBool(_animIDGrounded, Grounded);
            }*/
            //Debug.Log(_controller.isGrounded);
            Grounded = _controller.isGrounded;
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = SprintSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                // Parar o som
                walking = false;
            }
            else
            {
                lastMoveDir = _input.move;
            }


            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                if (rotateWhenMoving)
                {
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                if (_animationBlend >= 1)
                {
                    //Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Run"));
                    //_animator.SetBool(_animIDRun, true);
                    _animator.SetBool("Running", true);
                }
                else
                {
                    //Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Run"));
                    //_animator.SetBool(_animIDRun, false);
                    _animator.SetBool("Running", false);

                }

                //_animator.SetFloat(_animIDSpeed, _animationBlend);
                //_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void Interact()
        {
            if (_input.interact)
            {
                checkTrigger();
            }
            else
            {
                _input.interact = false;
            }
        }

        private void Melee()
        {
            if (_input.melee && !melee)
            {
                melee = true;
                blade.GetComponent<BladeBehaviour>().setActive();
                //gunArsenal[selectedGun].Reload();
                _animator.SetBool("Melee", true);
                //blade.transform.localPosition += new Vector3(0.0f, -0.15f, 0.0f);
                blade.transform.position += bladeOffset;
                //blade.transform.position = new Vector3(blade.transform.position.x + 0.02f, blade.transform.position.y - 0.057f, blade.transform.position.z + 0.0110f);
                //blade.transform.position += blade.transform.TransformDirection(blade.transform.forward);
            }
            else
            {
                melee = false;
                _input.melee = false;
                //_animator.SetBool("Melee", false);
            }
        }

        private void Reload()
        {
            if (_animator.GetBool("Reloading"))
            {
                return;
            }
            canReload = gunArsenal[selectedGun].reloading || (gunArsenal[selectedGun].currentAmmo == gunArsenal[selectedGun].numBulletsPerMagazine) || (gunArsenal[selectedGun].availableAmmo == 0 && !gunArsenal[selectedGun].is_pistol);
            if (_input.reload && !reload && !canReload)
            {
                reload = true;
                gunArsenal[selectedGun].Reload();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Objects/Guns/reload_gun1", transform.position);
                _animator.SetBool("Reloading", true);
            }
            else
            {
                reload = false;
                _input.reload = false;
            }
        }

        private void StartRound()
        {
            if (_input.startRound && !startRound)
            {
                startRound = true;
                //gunArsenal[selectedGun].FillAmmo();
                if (waveSpawner != null) waveSpawner.setStartRoundFlag(true);
                else
                {
                    if (rumbleSpawner.setStartRoundFlag(true))
                    {
                        //fill ammo of every Gun in the gunArsenal
                        foreach (GunBehaviour gun in gunArsenal)
                        {
                            gun.FillAmmo();
                        }
                    }
                }
                _input.startRound = false;
            }
            else
            {
                startRound = false;
                _input.startRound = false;
            }
        }

        private void Aim()
        {
            //Debug.Log(_input.aim);
            if (_input.aim)
            {
                SprintSpeed = 2.0f;
                sensitivity = 0.5f;
                aimCamera.gameObject.SetActive(true);
                //_animator.SetBool(_animIDRun, false);
                rotateWhenMoving = false;

                Vector3 mouseGlobalPosition = Vector3.zero;

                // Rotate the player to face where he is aiming
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 999f, aimColliderMask))
                {
                    mouseGlobalPosition = hit.point;
                }

                mouseGlobalPosition.y = transform.position.y;
                Vector3 aimDir = (mouseGlobalPosition - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 20f);

                _animator.SetBool("Aiming", true);

            }
            else
            {
                SprintSpeed = 5.335f;
                sensitivity = 1f;
                aimCamera.gameObject.SetActive(false);
                _animator.SetBool("Aiming", false);
                //_animator.SetBool(_animIDShoot, false);
                //_animator.SetBool(_animIDRun, true);
                rotateWhenMoving = true;
            }
        }

        public void AddWeaponCurrency(int ammount)
        {
            weaponCurrency += ammount;
        }

        private void Fire()
        {
            if (_animator.GetBool("Reloading")) return;
            if (_input.fire)
            {
                rotateWhenMoving = false;

                Vector3 mouseGlobalPosition = Vector3.zero;

                Vector2 shootingSpreadVec = new Vector2(0, 0);
                if (!_input.aim)
                {
                    if (!Grounded)
                    {
                        shootingSpreadVec = new Vector2(Random.Range(-gunArsenal[selectedGun].shootingSpread * 2, gunArsenal[selectedGun].shootingSpread * 2), Random.Range(-gunArsenal[selectedGun].shootingSpread * 2, gunArsenal[selectedGun].shootingSpread * 2));
                    }
                    else
                    {
                        shootingSpreadVec = new Vector2(Random.Range(-gunArsenal[selectedGun].shootingSpread, gunArsenal[selectedGun].shootingSpread), Random.Range(-gunArsenal[selectedGun].shootingSpread, gunArsenal[selectedGun].shootingSpread));
                    }
                }

                // Rotate the player to face where he is aiming
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 999f, aimColliderMask))
                {
                    mouseGlobalPosition = hit.point;
                }

                mouseGlobalPosition.y = transform.position.y;
                Vector3 aimDir = (mouseGlobalPosition - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 20f);
                //_animator.SetBool(_animIDShoot, true);
                _animator.SetBool("Shooting", true);
                if (canFire)
                {
                    gunArsenal[selectedGun].Shoot(shootingSpreadVec);
                    canFire = false;
                }
            }
            else
            {
                if (!_animator.GetBool("Aiming")) rotateWhenMoving = true;
                //_animator.SetBool(_animIDShoot, false);
                _animator.SetBool("Shooting", false);
            }
        }

        private void JumpAndGravity()
        {

            if (Grounded)
            {
                if (falling)
                {
                    falling = false;
                    //FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Jump/" + jumpBase + "2", transform.position);
                }
                /*if (_animator.GetCurrentAnimatorStateInfo(0).IsName("PistolJump")){
                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.6f){
                        _verticalVelocity = Gravity;
                    } else {
                        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    }
                }*/
                if (fallingY != -1234.56789f)
                {
                    // Fall
                    float distanceFallen = fallingY - transform.position.y;
                    fallingY = -1234.56789f;
                    //Debug.Log("You fell " + distanceFallen + " units");
                    if (distanceFallen > 4)
                    {
                        int id = Random.Range(1, 4);
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Punch and Shot/Hit/ah_charater_" + id, transform.position);
                        TakeDamage(1 * Mathf.RoundToInt(distanceFallen), "");
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Jump/" + jumpBase + "2", transform.position);
                    }
                }
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    //_animator.SetBool(_animIDJump, false);
                    //_animator.SetTrigger("Jumping");
                    //_animator.ResetTrigger("Jump");
                    if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping"))
                    {
                        _animator.SetBool("Jumping", false);
                    }
                    //Debug.Log("Grounded");
                    _animator.SetBool("Falling", false);
                    //_animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {

                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Jump/" + jumpBase + "1", transform.position);
                    Debug.Log("Jump Sound");

                    if (_hasAnimator)
                    {
                        //Debug.Log("Should Jump");
                        if (_animator.GetBool("Aiming")) _input.jump = false;
                        //_animator.SetBool(_animIDJump, true);
                        _animator.SetBool("Jumping", true);
                        /*if (_animator.GetCurrentAnimatorStateInfo(0).IsName("PistolRun")){
                            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                        } else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Running")){
                            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                        }*/
                        //Debug.Log("After Trigger");
                        /*if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping")){
                            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                            Debug.Log("Vertical");
                        }*/
                        //if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Jump")) _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    }

                    if (!jumping)
                        //FMODUnity.RuntimeManager.PlayOneShot("event:/Project/General Sounds/Character Related/Jump/Jump");
                        jumping = true;

                    // update animator if using character
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
                _input.jump = false;
            }
            else
            {
                falling = true;
                /*if(!_animator.GetBool(_animIDFreeFall)){
                    fallingY = transform.position.y;
                }*/

                if (!_animator.GetBool("Falling"))
                {
                    fallingY = transform.position.y;
                }

                jumping = false;
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        //_animator.SetBool(_animIDFreeFall, true);
                        _animator.SetBool("Falling", true);
                        _animator.SetBool("Jumping", false);
                    }
                }

                //_animator.SetBool("Jumping", false);

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void SwitchWeapon()
        {
            if (_input.weapon1)
            {
                _animator.SetBool("Pistol", true);
                selectedGun = 0;
            }
            else if (_input.weapon2)
            {
                if (gunArsenal.Count >= 2)
                {
                    _animator.SetBool("Pistol", false);
                    selectedGun = 1;
                }
            }
            else if (_input.weapon3)
            {
                if (gunArsenal.Count >= 3)
                {
                    _animator.SetBool("Pistol", false);
                    selectedGun = 2;
                }
            }
            else if (_input.weapon4)
            {
                if (gunArsenal.Count >= 4)
                {
                    _animator.SetBool("Pistol", false);
                    selectedGun = 3;
                }
            }
            if (_input.weaponScroll != 0)
            {
                selectedGun = (selectedGun + gunArsenal.Count + _input.weaponScroll) % gunArsenal.Count;
                if (selectedGun == 0)
                {
                    _animator.SetBool("Pistol", true);
                }
                else
                {
                    _animator.SetBool("Pistol", false);
                }
            }
            for (int i = 0; i < gunArsenal.Count; i++)
            {
                if (i == selectedGun)
                {
                    gunArsenal[selectedGun].gameObject.SetActive(true);
                    gunArsenal[selectedGun].updateReloadUI();
                }
                else
                {
                    gunArsenal[i].gameObject.SetActive(false);
                }
            }
            foreach (Transform child in WeaponsHUD)
            {
                if (child.gameObject.name.Contains("Icon"))
                {
                    if (child.gameObject.name == "Icon" + gunArsenal[selectedGun].gameObject.name)
                    {
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }

        /* UI Actions */
        private void Point()
        {
            // FIXME:
            if (_playerInput.currentActionMap.name != "UI") return;

            // Debug.Log(_input.navigate);
        }

        /* UI Actions */
        private void Click()
        {
            // FIXME:
            if (_playerInput.currentActionMap.name != "UI") return;

            // Debug.Log(_input.click);
            _input.click = false;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnLand(AnimationEvent animationEvent)
        {

        }

        private void checkTrigger()
        {
            _input.interact = false;
            if (arenaTrigger != null)
            {
                if ((nextRound - 1) % 5 == 0 && !startRound && !changingScene)
                {
                    //set the extraction portal active
                    Debug.Log("You're okay to extract");
                    //FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Portal/Enter");
                    changingScene = true;
                    arenaTrigger.performAction();
                }
            }
            if (trigger != null && !changingScene)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Objects/portal", transform.position);
                Debug.Log("Sound Here");
                Debug.Log("Entering Arena");
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Portal/Enter");
                changingScene = true;
                arenaPrompt.gameObject.SetActive(false);
                trigger.performAction();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            trigger = other.GetComponent<Trigger>();
            if (trigger != null)
            {
                if (arenaPrompt != null)
                {
                    arenaPrompt.gameObject.SetActive(true);
                    if (trigger.ArenaName == "Rumble")
                    {
                        foreach (Transform child in arenaPrompt)
                        {
                            if (child.gameObject.name == "Rumble")
                            {
                                arenaPrompt.GetChild(0).GetComponent<Text>().text = "Press [E] to enter Rumble Arena";
                                child.gameObject.SetActive(true);
                            }
                            else if (child.gameObject.name != "Tooltip")
                            {
                                child.gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        arenaPrompt.GetChild(1).gameObject.SetActive(true);
                        arenaPrompt.GetChild(2).gameObject.SetActive(true);
                        if (trigger.ArenaName == "Colliseum")
                        {
                            arenaPrompt.GetChild(0).GetComponent<Text>().text = "Press [E] to enter the Colloseum";
                            foreach (Transform child in arenaPrompt)
                            {
                                if (child.gameObject.name == "Colliseum")
                                {
                                    child.gameObject.SetActive(true);
                                }
                                else if (child.gameObject.name != "Tooltip" && child.gameObject.name != "DroppedCurrency")
                                {
                                    child.gameObject.SetActive(false);
                                }
                            }
                        }
                        else if (trigger.ArenaName == "Forest")
                        {
                            arenaPrompt.GetChild(0).GetComponent<Text>().text = "Press [E] to enter the Snowforest";
                            foreach (Transform child in arenaPrompt)
                            {
                                if (child.gameObject.name == "Forest")
                                {
                                    child.gameObject.SetActive(true);
                                }
                                else if (child.gameObject.name != "Tooltip" && child.gameObject.name != "DroppedCurrency")
                                {
                                    child.gameObject.SetActive(false);
                                }
                            }
                        }
                        else if (trigger.ArenaName == "Factory")
                        {
                            arenaPrompt.GetChild(0).GetComponent<Text>().text = "Press [E] to enter the Factory";
                            foreach (Transform child in arenaPrompt)
                            {
                                if (child.gameObject.name == "Factory")
                                {
                                    child.gameObject.SetActive(true);
                                }
                                else if (child.gameObject.name != "Tooltip" && child.gameObject.name != "DroppedCurrency")
                                {
                                    child.gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
            arenaTrigger = other.GetComponent<ArenaTrigger>();
            if (!foundArenaTrigger)
            {
                foundArenaTrigger = true;
                onDeathTrigger = arenaTrigger;
            }
            if (other.gameObject.tag == "Lava")
            {
                TakeDamage(Health - 1, "");
                TakeDamage(Health, "");
                int id = Random.Range(1, 4);
                FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Punch and Shot/Hit/ah_charater_" + id, transform.position);
            }
            if (other.gameObject.tag == "RumblePlane" && !recentlyTeleported)
            {
                // find the index of the other object in the rumble planes array
                int randomRumblePlane = Random.Range(0, rumblePlanes.Length);
                while (rumblePlanes[randomRumblePlane].name == other.gameObject.name)
                {
                    randomRumblePlane = Random.Range(0, rumblePlanes.Length);
                }
                // teleport the player to the randomRumblePlane
                _controller.enabled = false;
                transform.position = rumblePlanes[randomRumblePlane].gameObject.GetComponent<RumblePlane>().getTeleportPosition();
                rumbleSpawner.setCurrentScene(rumblePlanes[randomRumblePlane].name);
                _controller.enabled = true;
                other.isTrigger = false;
                teleportedTime = Time.time;
                recentlyTeleported = true;
            }
            if (other.gameObject.tag == "TrainingStarter")
            {
                //Debug.Log("Entrei no trigger");
                if (!training)
                {
                    TrainingHUD.SetActive(true);
                    WeaponShopUI.SetActive(false);
                }
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "TrainingStarter")
            {
                if (!training)
                {
                    if (!TrainingHUD.activeSelf)
                    {
                        TrainingHUD.SetActive(true);
                    }
                }
            }
            else
            {
                Trigger auxTrigger = other.GetComponent<Trigger>();
                if (auxTrigger != null)
                {
                    trigger = auxTrigger;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.name == "Blade") return;
            trigger = other.GetComponent<Trigger>();
            if (trigger != null)
            {
                if (arenaPrompt != null)
                {
                    arenaPrompt.GetChild(1).gameObject.SetActive(false);
                    arenaPrompt.GetChild(2).gameObject.SetActive(false);
                    arenaPrompt.gameObject.SetActive(false);
                }
            }
            trigger = null;
            arenaTrigger = null;
            if (other.gameObject.tag == "TrainingStarter")
            {
                if (TrainingHUD.activeSelf)
                {
                    TrainingHUD.SetActive(false);
                }
            }
        }

        void OnParticleCollision()
        {
            TakeDamage(1, "");
        }

        public void TakeDamage(float damage, string enemy)
        {
            Health -= damage;

            if (Health <= 0f)
            {
                Die();
                return;
            }

            healingOverTime.PlayerTookDamage();

            UpdateHealthUI();

            if (Health > 25 && !heartBeat)
            {
                float scale;
                if (hitsNumber >= 5)
                {
                    scale = 0.0f;
                }
                else
                {
                    scale = 2.5f - hitsNumber * 0.5f;
                }


                bloodOverlay.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1);
                Color temp = bloodOverlay.GetComponent<Image>().color;
                if (hitsNumber >= 5)
                {
                    temp.a = 0.75f;
                }
                else
                {
                    temp.a = 0.5f + hitsNumber * 0.05f;
                }

                bloodOverlay.GetComponent<Image>().color = temp;

                bloodOverlay.SetActive(true);
                playerGotHit = true;
                hitsNumber++;
            }
            else
            {
                bloodOverlay.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                Color temp = bloodOverlay.GetComponent<Image>().color;
                temp.a = 0.5f;
                bloodOverlay.GetComponent<Image>().color = temp;

                bloodOverlay.SetActive(true);
                heartBeat = true;
                heartBeatSound.start();
                heartBeatSound.release();
            }
        }

        public void Heal(float amount)
        {
            //Debug.Log("Healing");
            Health += amount;
            if (Health >= MaxHealth)
            {
                Health = MaxHealth;
            }
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            healthBar.value = 100.0f * Mathf.Round((Health / MaxHealth) * 100.0f) * 0.01f;
            healthBarText.text = "HP " + Health.ToString() + "/" + MaxHealth.ToString();
        }

        private void Die()
        {
            musicPlayer.StopMusic();
            if (!is_dead)
            {
                is_dead = true;
                Debug.Log("Player pre dead");
                _input.enabled = false;
                //deathScreen.SetActive(true);
                Debug.Log("Player died");
                //currencyCounter = 0;
                float deathTime = Time.time;
                deathScreen.SetActive(true);
                if (onDeathTrigger == null)
                {
                    onDeathTrigger = GameObject.Find("DeathTrigger").GetComponent<ArenaTrigger>();
                }
                onDeathTrigger.performAction(true);
            }
        }

        private void updateCurrencyUI()
        {
            currencyAmountUI.text = weaponCurrency.ToString();
        }

        private void updateFacCurrency()
        {
            facCurrencyAmountUI.text = facCurrency.ToString();
        }

        private void updateForCurrency()
        {
            forCurrencyAmountUI.text = forCurrency.ToString();
        }

        private void updateRumCurrency()
        {
            rumCurrencyAmountUI.text = rumCurrency.ToString();
        }

        private void updateColCurrency()
        {
            colCurrencyAmountUI.text = colCurrency.ToString();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.tag == "Currency")
            {
                CurrencyBehaviour currency = hit.transform.GetComponent<CurrencyBehaviour>();
                currency.DeSpawn();
                if (hit.gameObject.name.Contains("Colliseum"))
                {
                    colCurrency++;
                    updateColCurrency();
                }
                else if (hit.gameObject.name.Contains("Factory"))
                {
                    facCurrency++;
                    updateFacCurrency();
                }
                else if (hit.gameObject.name.Contains("Forest"))
                {
                    forCurrency++;
                    updateForCurrency();
                }
                else if (hit.gameObject.name.Contains("Rumble"))
                {
                    rumCurrency++;
                    updateRumCurrency();
                }
                FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Objects/pick_coin", transform.position);
            }
            if (hit.gameObject.tag == "Enemy" || hit.transform.tag == "RangedEnemy")
            {
                GetComponent<ImpactReceiver>().AddImpact(new Vector3(lastMoveDir.x, 0, lastMoveDir.y), 0.4f);
            }
        }

        public void SwitchInputToUI()
        {
            _playerInput.SwitchCurrentActionMap("UI");
        }

        public void SwitchInputToPlayer()
        {
            _playerInput.SwitchCurrentActionMap("Player");
        }

        float GetStartingHealth()
        {
            return GetStartingMaxHealth();
        }

        float GetStartingMaxHealth()
        {
            int step = 10;
            int level = PlayerPrefs.GetInt("HealthLevel") == 0 ? 1 : PlayerPrefs.GetInt("HealthLevel");

            return (100.0f + (float)(step * (level - 1)));
        }

        int GetStartingMoney()
        {
            int step = 25;
            int level = PlayerPrefs.GetInt("MoneyLevel") == 0 ? 1 : PlayerPrefs.GetInt("MoneyLevel");

            return (100 + (int)(step * (level - 1)));
        }

        void AdaptToWeaponLevels()
        {
            int levelPS = PlayerPrefs.GetInt("Weapon0Level");
            int levelAR = PlayerPrefs.GetInt("Weapon1Level");
            int levelSG = PlayerPrefs.GetInt("Weapon2Level");
            int levelRL = PlayerPrefs.GetInt("Weapon3Level");

            PS.damage += levelPS * 1.0f;
            AR.damage += levelAR * 1.0f;
            SG.damage += levelSG * 1.0f;
            RL.damage += levelRL * 1.0f;

            PS.range += levelPS * 1.0f;
            AR.range += levelAR * 1.0f;
            SG.range += levelSG * 1.0f;
            RL.range += levelRL * 1.0f;

            PS.fireRateTimer -= levelPS * 0.001f;

            SG.innacuracyDistance -= levelSG * 0.05f;
        }
    }
}
