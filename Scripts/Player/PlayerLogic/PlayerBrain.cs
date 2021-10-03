using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponDamages
{
    STICK = 20
};

// state of movement of this player
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

    private EntityCharacteristics _chars;
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



    #region BASIC-FUNCTIONS SECTION
    private void Awake()
    {
        _chars = this.GetComponent<PlayerChars>();
        _eventHandler = this.GetComponent<EntityEventHandler>();
        _movementController = this.GetComponent<PlayerMovement>();
        _animationController = this.GetComponent<PlayerAnimation>();
        _combatController = this.GetComponent<EntityCombat>();
        _UI = this.GetComponent<PlayerUI>();
        _FOV = this.GetComponent<PlayerFOV>();
        _inventory = this.GetComponent<PlayerInventory>();

        _rigidbody = this.GetComponent<Rigidbody>();
        _playerCollider = this.GetComponent<Collider>();

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

        Walk();
    }

    private void Update()
    {
        if (_isAllowedToMove)
        {
            if (_isAllowedToAttack)
                AnimationHitDecide();
            AnimationMovementDecide();
            
            if (Input.GetKeyDown(KeyCode.H)) // loud noise
            {
                _eventHandler.CallHearers();
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_inventory.IsMainInventoryActive) _inventory.DisableMainInventory();
            else                                  _inventory.ActivateMainInventory();
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
        }
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
    #endregion

    #region INVENTORY SECTION
    /// <summary>
    /// Drops the item from the inventory
    /// </summary>
    /// <param name="slotID">id of the slot where the item is</param>
    /// <param name="quickSlot">whether the slot is a quick one</param>
    /// <param name="amount">how many of the item is going to be dropped. -1 means all of them</param>
    /// <param name="fromMousePos">whether its gonna drop on mouse position or from near the player</param>
    public void DropItem(int slotID, bool quickSlot, int amount = -1)
    {
        GameObject prefab = _inventory.GetPrefabFromItemInSlot(slotID, quickSlot);
        Instantiate(prefab, transform.position + new Vector3(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1)), Quaternion.identity);
        
        _inventory.RemoveItem(slotID);

    }

    public Item GetItemFromSlotID(int slotID, bool quickSlot)
    {
        return _inventory.GetItemFromSlotID(slotID, quickSlot);
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
            _animationController.ActivateTrigger(AnimatorStates.QuickHit);
            _chars.DisableStaminaRegen(_afterActionDelayTime);
        }
        else if (longHit && _chars.Stamina >= -S_PUNISH_LONG_HIT_N_FINISHED)
        {
            _animationController.ActivateTrigger(AnimatorStates.StartLong);
            _chars.DisableStaminaRegen(_afterActionDelayTime);
        }
        else if (_longHitLastFrame && !longHit)
        {
            if (_longHitReady)
            {
                if (_chars.Stamina >= -S_PUNISH_LONG_HIT_FINISHED)
                    _animationController.ActivateTrigger(AnimatorStates.ProceedLong);
                else
                    _animationController.ActivateTrigger(AnimatorStates.CancelLong);

                _chars.DisableStaminaRegen(_afterActionDelayTime);
            }
            else
            {
                _animationController.ActivateTrigger(AnimatorStates.CancelLong);
            }
            _longHitReady = false;
        }

        if (block && _chars.Stamina >= S_PUNISH_BLOCK)
        {
            _animationController.ActivateTrigger(AnimatorStates.Block);
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
            else if (other.gameObject.CompareTag("CollectableIcon") && Input.GetKeyDown(KeyCode.E))
            {
                Collectable collectableInstance = other.transform.parent.GetComponent<Collectable>();
                _inventory.AddItem(collectableInstance.ItemID, collectableInstance.Type, collectableInstance.Amount, collectableInstance.MaxAmount);
                Destroy(other.transform.parent.gameObject);
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
