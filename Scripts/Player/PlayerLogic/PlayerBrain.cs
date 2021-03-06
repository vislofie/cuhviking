using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    Crouching, Walking, Running
};
public class PlayerBrain : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerMesh;

    // stamina punishes for all types of attacks
    public const int S_PUNISH_LONG_HIT_FINISHED = -15;
    public const int S_PUNISH_LONG_HIT_N_FINISHED = -20;
    public const int S_PUNISH_BLOCK = -15;
    public const int S_PUNISH_QUICK_HIT = -50;

    private PlayerChars _chars;
    private EntityEventHandler _eventHandler;
    private PlayerMovement _movementController;
    private PlayerAnimation _animationController;
    private EntityCombat _combatController;
    private PlayerUI _UI;
    private PlayerFOV _FOV;
    private PlayerInventory _inventory;

    private Rigidbody _rigidbody;
    private Collider _playerCollider;

    private Ladder _currentLadder;

    [Tooltip("An object that carries all weapons inside")]
    [SerializeField]
    private Transform _weaponHolder;
    private List<WeaponHitManager> _weaponManagers = new List<WeaponHitManager>();
    private int _activeWeaponIndex; // an index of a weapon that player carries at the moment; -1 if player doesnt carry anything

    
    public MovementState PlayerMovementState { get { return _playerMovementState; } }
    [Header("Movement")]
    [SerializeField]
    private MovementState _playerMovementState;
    [SerializeField]
    private float _runStaminaPunish = 2.0f;

    private float _afterActionDelayTime; // Time it takes to restore health or stamina regen after some action.

    private bool _crouching;
    private bool _walking;

    private bool _isAllowedToMove;
    private bool _isAllowedToAttack;

    private bool _longHitLastFrame;
    private bool _longHitReady; // tells whether LongHitStart has ended and animation should proceed

    private IEnumerator _delay; // coroutine that allows to create delays
    private bool _delayEnded; // boolean variable responsible for telling whether the delay has finished or nah

    [Header("Inventory")]
    [SerializeField]
    private LayerMask _collectablesLayerMask;



    #region BASIC-FUNCTIONS SECTION
    private void Awake()
    {
        _chars = GetComponent<PlayerChars>();
        _eventHandler = GetComponent<EntityEventHandler>();
        _movementController = GetComponent<PlayerMovement>();
        _animationController = GetComponent<PlayerAnimation>();
        _combatController = GetComponent<EntityCombat>();
        _UI = GetComponent<PlayerUI>();
        _FOV = GetComponent<PlayerFOV>();
        _inventory = GetComponent<PlayerInventory>();

        _rigidbody = GetComponent<Rigidbody>();
        _playerCollider = GetComponent<Collider>();

        _isAllowedToMove = true;
        _isAllowedToAttack = true;

        _longHitLastFrame = false;
        _longHitReady = false;

        FindAllWeaponManagers();
        _activeWeaponIndex = 0;

        DisableActiveHitManager("Awake");

        _afterActionDelayTime = _chars.AfterActionDelayTime;

        _delayEnded = true;
        _delay = null;
    }

    private void Start()
    {
        _chars.SetMaxHealth(100);
        _chars.ChangeHealth(100);
        _chars.SetMaxStamina(100);
        _chars.ChangeStamina(100);

        _chars.EnableHealthRegen();
        _chars.EnableStaminaRegen();

        _inventory.PassFoodDelegateToContextMenu(OnEatFood);
        _inventory.PassLiquidDelegateToContextMenu(OnDrinkLiquid);

        Walk();
    }

    private void Update()
    {
        if (_isAllowedToMove)
        {
            if (_isAllowedToAttack && !_inventory.MouseInInventory)
                AnimationHitDecide();
            AnimationMovementDecide();
            
            if (Input.GetKeyDown(KeyCode.H)) // loud noise
            {
                _eventHandler.CallHearers();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            _inventory.SwitchInventorySlots();
            return;
        }

        if (_currentLadder != null)
        {
            if (Input.GetKey(KeyCode.Space) && _delayEnded)
            {
                _currentLadder.ClimbUp();
                _delay = Delay(0.5f);
                StartCoroutine(_delay);
            }
            else if (Input.GetKey(KeyCode.M) && _delayEnded)
            {
                _currentLadder.ClimbDown();
                _delay = Delay(0.5f);
                StartCoroutine(_delay);
            }

            return;
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity, _collectablesLayerMask))
            {
                Collectable pickedItem = null;

                if (hit.collider.TryGetComponent(out pickedItem))
                {
                    if (pickedItem.IconActivated)
                    {
                        _inventory.AddItem(pickedItem.ItemID, pickedItem.Amount);
                        pickedItem.DestroyItself();

                        return;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isAllowedToMove)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            _movementController.UpdatePosition(horizontal, vertical);
            _movementController.UpdateRotation();
        }

        _FOV.SetAimDirection(_movementController.MousePosition - transform.position);
        _FOV.SetOrigin(transform.position);
    }
    #endregion

    #region WEAPON-COMBAT SECTION
    /// <summary>
    /// Searches for weapon managers. I mean, name says it all
    /// </summary>
    private void FindAllWeaponManagers()
    {
        _weaponManagers.Clear();
        for (int i = 0; i < _weaponHolder.childCount; i++)
            _weaponManagers.Add(_weaponHolder.GetChild(i).GetChild(0).GetComponent<WeaponHitManager>());
    }

    /// <summary>
    /// ReceiveDamage. Gets damage and senderEntityCombat from EntityCombat script and works with it further
    /// </summary>
    /// <param name="damage">amount of damage</param>
    /// <param name="senderEntityCombat">EntityCombat of an entity who sent the damage</param>
    public void ReceiveDamage(int damage, EntityCombat senderEntityCombat)
    {
        _chars.ChangeHealth(-damage);
    }

    /// <summary>
    /// SendDamageToEnemy. Gets Enemy's combatController and works with it further, sending damage from own EntityCombat
    /// </summary>
    /// <param name="combatController">Enemy's Entity Combat that gets hit</param>
    public void SendDamageToEnemy(EntityCombat combatController)
    {
        Debug.Log("Hit happened");
        // TODO: make a delay for sending damage from player
        combatController.ReceiveDamage((int)WeaponDamages.STICK, _combatController);
        DisableActiveHitManager("SendDamageToEnemy()");
    }

    /// <summary>
    /// Enables the hit manager that is responsible for current weapon
    /// </summary>
    public void EnableActiveHitManager()
    {
        _weaponManagers[_activeWeaponIndex].Enable();
    }

    /// <summary>
    /// Disables the hit manager that is responsible for current weapon
    /// </summary>
    public void DisableActiveHitManager(string whoCalled)
    {
        _weaponManagers[_activeWeaponIndex].Disable();
    }
    #endregion

    #region CONSUMPTION SECTION

    /// <summary>
    /// Gets called when player eats food from the inventory
    /// </summary>
    /// <param name="value">amount of hunger to add(welp reduce actually but yknow we use the opposite of hunger as the value in the bar)</param>
    private void OnEatFood(int foodID)
    {
        switch (foodID)
        {
            case 2:
                _chars.Hunger += 2.0f;
                break;
            default:
                Debug.LogWarning("WRONG FOOD ID ON OnEatFood(int) IN " + GetType().Name);
                break;
        }
    }

    /// <summary>
    /// Gets called when player drinks some liquid from the inventory
    /// </summary>
    /// <param name="value"></param>
    private void OnDrinkLiquid(int liquidID)
    {
        switch (liquidID)
        {
            case (int)ItemID.Ale:
                _chars.Thirst += 3.0f;
                _chars.Hunger += 0.5f;

                _chars.ChangeHealth(-5.0f);
                _chars.ChangeStamina(-10.0f);
                _chars.DisableHealthRegen(_afterActionDelayTime);
                _chars.DisableStaminaRegen(_afterActionDelayTime);
                break;
            default:
                Debug.LogWarning("WRONG LIQUID ID ON OnDrinkLiquid(int) IN " + GetType().Name);
                break;
        }
    }
    #endregion

    #region MOVEMENT SECTION
    /// <summary>
    /// Makes player crouch
    /// </summary>
    private void Crouch()
    {
        _crouching = true;
        _walking = false;
        _playerMovementState = MovementState.Crouching;
        _movementController.ChangeMovementSpeed(_playerMovementState);

        _weaponHolder.transform.localPosition = new Vector3(0.6f, -1, 0);
        _playerMesh.transform.localPosition = new Vector3(0, -2, 0);
        _playerMesh.transform.localRotation = Quaternion.Euler(30, 0, 0);
    }

    /// <summary>
    /// Makes player walk normally
    /// </summary>
    private void Walk()
    {
        _crouching = false;
        _walking = true;
        _playerMovementState = MovementState.Walking;
        _movementController.ChangeMovementSpeed(_playerMovementState);
        _chars.DisableStaminaGraduateChange();

        _weaponHolder.transform.localPosition = new Vector3(0.6f, 0, 0);
        _playerMesh.transform.localPosition = new Vector3(0, -1, 0);
        _playerMesh.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Makes player run
    /// </summary>
    private void Run()
    {
        _crouching = false;
        _walking = false;
        _playerMovementState = MovementState.Running;
        _movementController.ChangeMovementSpeed(_playerMovementState);
        _chars.EnableStaminaGraduateChange(-_runStaminaPunish, true);

        _weaponHolder.transform.localPosition = new Vector3(0.6f, 0, 0);
        _playerMesh.transform.localPosition = new Vector3(0, -1, 0);
        _playerMesh.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Called after player stopped climbing a ladder
    /// </summary>
    public void RemoveLadder()
    {
        _currentLadder = null;
        _rigidbody.useGravity = true;
        _playerCollider.enabled = true;
    }

    /// <summary>
    /// Allows player movement
    /// </summary>
    public void AllowMovement()
    {
        _isAllowedToMove = true;
    }

    /// <summary>
    /// Forbids player movement
    /// </summary>
    public void ForbidMovement()
    {
        _isAllowedToMove = false;
    }

    public void AllowAttackAndInteractionMovement()
    {
        _isAllowedToAttack = true;
    }

    public void ForbidAttackAndInteractionMovement()
    {
        _isAllowedToAttack = false;
    }
    #endregion

    #region CHARACTERISTICS SECTION

    public void ChangeStamina(float value)
    {
        _chars.ChangeStamina(value);
    }

    public void ChangeHealth(float value)
    {
        _chars.ChangeHealth(value);
    }

    public void UpdateHealthInUI()
    {
        _UI.UpdateHealthStatus(_chars.Health, _chars.MaxHealth);
    }

    public void UpdateStaminaInUI()
    {
        _UI.UpdateStaminaStatus(_chars.Stamina, _chars.MaxStamina);
    }

    public void UpdateHungerInUI()
    {
        _UI.UpdateHungerStatus(_chars.Hunger, _chars.MaxHunger);
    }

    public void UpdateThirstInUI()
    {
        _UI.UpdateThirstStatus(_chars.Thirst, _chars.MaxThirst);
    }
    #endregion

    #region ANIMATION SECTION
    /// <summary>
    /// Gets called when LongHitStart animation has ended
    /// </summary>
    public void OnLongHitReady()
    {
        _longHitReady = true;
    }

    /// <summary>
    /// Decides which movement animation to play depening on the input. At current point it just decides what movement is happening right now
    /// </summary>
    private void AnimationMovementDecide()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_crouching) Walk();
            else            Crouch();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_walking || _crouching) Run();
            else          Walk();
        }
    }

    /// <summary>
    /// Decides which hit animation to play depending on the input
    /// </summary>
    private void AnimationHitDecide()
    {
        bool longHit = Input.GetMouseButton(0);
        bool block = Input.GetMouseButton(1);
        bool quickHit = Input.GetMouseButtonDown(2);

        if (quickHit && _chars.Stamina >= -S_PUNISH_QUICK_HIT)
        {
            _animationController.ActivateTrigger(PAnimatorStates.QuickHit);
            _chars.DisableStaminaRegen(_afterActionDelayTime);
        }
        else if (longHit && _chars.Stamina >= -S_PUNISH_LONG_HIT_N_FINISHED)
        {
            _animationController.ActivateTrigger(PAnimatorStates.StartLong);
            _chars.DisableStaminaRegen(_afterActionDelayTime);
        }
        else if (_longHitLastFrame && !longHit)
        {
            if (_longHitReady)
            {
                if (_chars.Stamina >= -S_PUNISH_LONG_HIT_FINISHED)
                    _animationController.ActivateTrigger(PAnimatorStates.ProceedLong);
                else
                    _animationController.ActivateTrigger(PAnimatorStates.CancelLong);

                _chars.DisableStaminaRegen(_afterActionDelayTime);
            }
            else
            {
                _animationController.ActivateTrigger(PAnimatorStates.CancelLong);
            }
            _longHitReady = false;
        }

        if (block && _chars.Stamina >= S_PUNISH_BLOCK)
        {
            _animationController.ActivateTrigger(PAnimatorStates.Block);
            _chars.DisableStaminaRegen(_afterActionDelayTime);
        }

        _longHitLastFrame = longHit;
    }
    #endregion

    #region COLLISION-INTERACTION
    private void OnTriggerStay(Collider other)
    {
        if (_isAllowedToMove)
        {
            if (other.gameObject.CompareTag("LadderPlatform") && Input.GetKeyDown(KeyCode.E))
            {
                _currentLadder = other.GetComponentInParent<Ladder>();
                other.GetComponent<LadderPlatform>().ActivateClimbing();
                ForbidMovement();
                _rigidbody.useGravity = false;
                _playerCollider.enabled = false;

                _movementController.LookAt(other.transform.parent.GetChild(other.transform.parent.childCount - 1).position);
            }
        }
        
    }
    #endregion

    #region COROUTINES

    /// <summary>
    /// coroutine that allows to create delays
    /// </summary>
    /// <param name="sec">time in seconds</param>
    /// <returns></returns>
    private IEnumerator Delay(float sec)
    {
        _delayEnded = false;
        yield return new WaitForSeconds(sec);
        _delayEnded = true;
    }
    #endregion
}
