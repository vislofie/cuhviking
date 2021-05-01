using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    private PlayerMovement _movementController;
    private PlayerAnimation _animationController;
    private PlayerCombat _combatController;

    private bool _isAllowedToMove;

    private void Awake()
    {
        _movementController = this.gameObject.GetComponent<PlayerMovement>();
        _animationController = this.gameObject.GetComponent<PlayerAnimation>();
        _combatController = this.gameObject.GetComponent<PlayerCombat>();

        _isAllowedToMove = true;
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool quickHit = Input.GetMouseButtonDown(2);

        if (_isAllowedToMove)
        {
            _movementController.UpdatePosition(horizontal, vertical);
        }
        else
        {

        }

        if (quickHit)
        {
            _animationController.PlayAnimation(PlayerAnimations.QuickHit);
        }
    }
}
