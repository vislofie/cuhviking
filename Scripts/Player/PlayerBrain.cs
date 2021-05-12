using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    // stamina punishes for all types of attacks
    public const int S_PUNISH_LONG_HIT_FINISHED = -15;
    public const int S_PUNISH_LONG_HIT_N_FINISHED = -20;
    public const int S_PUNISH_BLOCK = -15;
    public const int S_PUNISH_QUICK_HIT = -50;

    private PlayerCharacteristics _chars;
    private PlayerMovement _movementController;
    private PlayerAnimation _animationController;
    private PlayerCombat _combatController;
    private PlayerUI _UI;

    private bool _isAllowedToMove;

    private bool _longHitLastFrame;
    private bool _longHitReady; // tells whether LongHitStart has ended and animation should proceed

    private void Awake()
    {
        _chars = this.gameObject.GetComponent<PlayerCharacteristics>();
        _movementController = this.gameObject.GetComponent<PlayerMovement>();
        _animationController = this.gameObject.GetComponent<PlayerAnimation>();
        _combatController = this.gameObject.GetComponent<PlayerCombat>();
        _UI = this.gameObject.GetComponent<PlayerUI>();

        _isAllowedToMove = true;

        _longHitLastFrame = false;
        _longHitReady = false;
    }

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
        }
    }

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
        ChangeStamina(2);
        yield return new WaitForSeconds(1);
        RestoreStaminaCycle();
    }

    private IEnumerator BlockRestoringStamina()
    {
        StopCoroutine(RestoringStamina());
        yield return new WaitForSeconds(2);
        StartCoroutine(RestoringStamina());
    }

    // calls when LongHitStart animation has ended
    public void OnLongHitReady()
    {
        _longHitReady = true;
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
        Debug.Log("Change stamina " + delta);
        _UI.UpdateStaminaStatus(_chars.Stamina, _chars.MaxStamina);
    }

    public void SetMaxStamina(int value)
    {
        _chars.SetMaxStamina(value);
        _UI.UpdateStaminaStatus(_chars.Stamina, _chars.MaxStamina);
    }
}
