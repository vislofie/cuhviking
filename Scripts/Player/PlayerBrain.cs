using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponDamages
{
    STICK = 20
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

    [Tooltip("An object that carries all weapons inside")]
    [SerializeField]
    private Transform _weaponHolder;
    private List<WeaponHitManager> _weaponManagers = new List<WeaponHitManager>();
    private int _activeWeaponIndex; // an index of a weapon that player carries at the moment; -1 if player doesnt carry anything

    private bool _isAllowedToMove;

    private bool _longHitLastFrame;
    private bool _longHitReady; // tells whether LongHitStart has ended and animation should proceed

    private void Awake()
    {
        _chars = this.gameObject.GetComponent<EntityCharacteristics>();
        _eventHandler = this.gameObject.GetComponent<EntityEventHandler>();
        _movementController = this.gameObject.GetComponent<PlayerMovement>();
        _animationController = this.gameObject.GetComponent<PlayerAnimation>();
        _combatController = this.gameObject.GetComponent<EntityCombat>();
        _UI = this.gameObject.GetComponent<PlayerUI>();

        _isAllowedToMove = true;

        _longHitLastFrame = false;
        _longHitReady = false;

        FindAllWeaponManagers();
        _activeWeaponIndex = 0;

        DisableActiveHitManager("Awake");
    }

    #region BASIC-FUNCTIONS SECTION
    private void Start()
    {
        SetMaxHealth(100);
        ChangeHealth(100);
        SetMaxStamina(100);
        ChangeStamina(100);

        RestoreStaminaCycle();
        
    }

    private void FixedUpdate()
    {
        if (_isAllowedToMove)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool longHit = Input.GetMouseButton(0);
            bool block = Input.GetMouseButton(1);
            bool quickHit = Input.GetMouseButtonDown(2);

            _movementController.UpdatePosition(horizontal, vertical);
            _movementController.UpdateRotation();

            if (quickHit && _chars.Stamina >= -S_PUNISH_QUICK_HIT)
            {
                _animationController.ActivateTrigger(AnimatorStates.QuickHit);
                BlockStamina();
            }
            else if (longHit && _chars.Stamina >= -S_PUNISH_LONG_HIT_N_FINISHED)
            {
                _animationController.ActivateTrigger(AnimatorStates.StartLong);
                BlockStamina();
            }
            else if (_longHitLastFrame && !longHit)
            {
                if (_longHitReady)
                {
                    if (_chars.Stamina >= -S_PUNISH_LONG_HIT_FINISHED)
                        _animationController.ActivateTrigger(AnimatorStates.ProceedLong);
                    else
                        _animationController.ActivateTrigger(AnimatorStates.CancelLong);

                    BlockStamina();
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
                BlockStamina();
            }

            _longHitLastFrame = longHit;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _eventHandler.CallHearers();
            }
        }
    }
    #endregion

    #region WEAPON-COMBAT SECTION
    private void FindAllWeaponManagers()
    {
        _weaponManagers.Clear();
        for (int i = 0; i < _weaponHolder.childCount; i++)
        {
            _weaponManagers.Add(_weaponHolder.GetChild(i).GetChild(0).GetComponent<WeaponHitManager>());
        }

    }

    /// <summary>
    /// ReceiveDamage. Gets damage and senderEntityCombat from EntityCombat script and works with it further.
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

    public void EnableActiveHitManager()
    {
        _weaponManagers[_activeWeaponIndex].Enable();
    }

    public void DisableActiveHitManager(string whoCalled)
    {
        _weaponManagers[_activeWeaponIndex].Disable();
        Debug.Log(whoCalled);
    }
    #endregion

    #region CHARACTERISTICS SECTION
    private void BlockStamina()
    {
        StopAllCoroutines();
        StartCoroutine(BlockRestoringStamina());
    }

    private void RestoreStaminaCycle()
    {
        StartCoroutine(RestoringStamina());
    }

    private IEnumerator RestoringStamina()
    {
        ChangeStamina(5);
        yield return new WaitForSeconds(.1f);
        RestoreStaminaCycle();
    }

    private IEnumerator BlockRestoringStamina()
    {
        StopCoroutine(RestoringStamina());
        yield return new WaitForSeconds(2);
        StartCoroutine(RestoringStamina());
    }

    

    public void ChangeHealth(int delta)
    {
        _chars.ChangeHealth(delta);
        _UI.UpdateHealthStatus(_chars.Health, _chars.MaxHealth);
    }

    public void SetMaxHealth(int value)
    {
        _chars.SetMaxHealth(value);
        _UI.UpdateHealthStatus(_chars.Health, _chars.MaxHealth);
    }

    public void ChangeStamina(int delta)
    {
        _chars.ChangeStamina(delta);
        //Debug.Log("Change stamina " + delta);
        _UI.UpdateStaminaStatus(_chars.Stamina, _chars.MaxStamina);
    }

    public void SetMaxStamina(int value)
    {
        _chars.SetMaxStamina(value);
        _UI.UpdateStaminaStatus(_chars.Stamina, _chars.MaxStamina);
    }
    #endregion

    #region ANIMATION SECTION
    // calls when LongHitStart animation has ended
    public void OnLongHitReady()
    {
        _longHitReady = true;
    }
    #endregion
}
