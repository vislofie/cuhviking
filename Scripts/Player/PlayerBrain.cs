using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    private PlayerMovement _movementController;
    private PlayerAnimation _animationController;
    private PlayerCombat _combatController;

    private bool _isAllowedToMove;

    private bool _longHitLastFrame;
    private bool _longHitReady;

    private void Awake()
    {
        _movementController = this.gameObject.GetComponent<PlayerMovement>();
        _animationController = this.gameObject.GetComponent<PlayerAnimation>();
        _combatController = this.gameObject.GetComponent<PlayerCombat>();

        _isAllowedToMove = true;

        _longHitLastFrame = false;
        _longHitReady = false;
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

            if (quickHit)
            {
                _animationController.ActivateTrigger(AnimatorStates.QuickHit);
            }
            else if (longHit)
            {
                _animationController.ActivateTrigger(AnimatorStates.StartLong);
            }
            else if (_longHitLastFrame && !longHit)
            {
                _animationController.ActivateTrigger(_longHitReady ? AnimatorStates.ProceedLong : AnimatorStates.CancelLong);
                _longHitReady = false;
            }

            if (block)
            {
                _animationController.ActivateTrigger(AnimatorStates.Block);
            }

            _longHitLastFrame = longHit;
        }
    }

    // calls when longhitstart animation has ended
    public void OnLongHitReady()
    {
        _longHitReady = true;
    }
}
