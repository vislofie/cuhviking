﻿using System.Collections;
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

    private bool _longHitLastFrame;
    private bool _longHitReady; // tells whether LongHitStart has ended and animation should proceed



    #region BASIC-FUNCTIONS SECTION
    private void Awake()
    {
        _chars = this.gameObject.GetComponent<PlayerChars>();
        _eventHandler = this.gameObject.GetComponent<EntityEventHandler>();
        _movementController = this.gameObject.GetComponent<PlayerMovement>();
        _animationController = this.gameObject.GetComponent<PlayerAnimation>();
        _combatController = this.gameObject.GetComponent<EntityCombat>();
        _UI = this.gameObject.GetComponent<PlayerUI>();
        _FOV = this.gameObject.GetComponent<PlayerFOV>();

        _isAllowedToMove = true;

        _longHitLastFrame = false;
        _longHitReady = false;

        FindAllWeaponManagers();
        _activeWeaponIndex = 0;

        DisableActiveHitManager("Awake");

        _afterActionDelayTime = _chars.AfterActionDelayTime;
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
            AnimationHitDecide();
            AnimationMovementDecide();
            
            if (Input.GetKeyDown(KeyCode.H))
            {
                _eventHandler.CallHearers();
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

            _FOV.SetAimDirection(_movementController.MousePosition - transform.position);
            _FOV.SetOrigin(transform.position);
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
        Debug.Log(whoCalled);
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
}
