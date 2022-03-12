using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PAnimatorStates
{
    QuickHit, CancelLong, StartLong, ProceedLong, Block, Unblock,
    Crouch, Walk, Run
};

public class PlayerAnimation : MonoBehaviour
{
    private const int ANIMATOR_STATES_COUNT = 9;

    private PlayerBrain _brain;

    [SerializeField]
    private Dictionary<PAnimatorStates, bool> _triggersState = new Dictionary<PAnimatorStates, bool>(); // if animation is allowed to be played the value of a key is true, if not then it is false

    private Animator _animator;

    private void Awake()
    {
        for (int i = 0; i < ANIMATOR_STATES_COUNT; i++)
            _triggersState.Add((PAnimatorStates)i, true);

        _animator = GetComponent<Animator>();
        _brain = GetComponent<PlayerBrain>();
    }

    private void ResetTriggers()
    {
        _animator.ResetTrigger("QuickHit");
        _animator.ResetTrigger("CancelLong");
        _animator.ResetTrigger("StartLong");
        _animator.ResetTrigger("ProceedLong");
        _animator.ResetTrigger("Block");
    }

    public void ActivateTrigger(PAnimatorStates trigger)
    {
        if (_triggersState[trigger] == true)
        {
            switch (trigger)
            {
                case PAnimatorStates.Block:
                    _animator.SetTrigger("Block");
                    _triggersState[trigger] = false;
                    _brain.ChangeStamina(PlayerBrain.S_PUNISH_BLOCK);
                    break;
                case PAnimatorStates.CancelLong:
                    _animator.SetTrigger("CancelLong");
                    break;
                case PAnimatorStates.ProceedLong:
                    _animator.SetTrigger("ProceedLong");
                    _triggersState[trigger] = false;
                    _brain.ChangeStamina(PlayerBrain.S_PUNISH_LONG_HIT_FINISHED);
                    break;
                case PAnimatorStates.QuickHit:
                    _animator.SetTrigger("QuickHit");
                    _triggersState[trigger] = false;
                    _brain.ChangeStamina(PlayerBrain.S_PUNISH_QUICK_HIT);
                    break;
                case PAnimatorStates.StartLong:
                    _animator.SetTrigger("StartLong");
                    _triggersState[trigger] = false;
                    _brain.ChangeStamina(PlayerBrain.S_PUNISH_LONG_HIT_N_FINISHED);
                    break;
                case PAnimatorStates.Unblock:
                    _animator.SetTrigger("Unblock");
                    break;
                case PAnimatorStates.Crouch:
                    // TODO: add crouch triggering animation
                    break;
                case PAnimatorStates.Walk:
                    // TODO: add walk triggering animation
                    break;
                case PAnimatorStates.Run:
                    // TODO: add run triggering animation
                    break;
                default:
                    Debug.Log("bruh you dum as hell like fr");
                    break;
            }
        }
        else
        {
            //Debug.Log("bibibobo you tried to play the animation that is already playing!");
        }
    }

    public void OnAnimationEnd(PAnimatorStates trigger)
    {
        switch (trigger)
        {
            case PAnimatorStates.Block:
                break;
            case PAnimatorStates.CancelLong:
                break;
            case PAnimatorStates.ProceedLong:
                _triggersState[trigger] = true;
                _triggersState[PAnimatorStates.StartLong] = true;
                break;
            case PAnimatorStates.QuickHit:
                _triggersState[trigger] = true;
                break;
            case PAnimatorStates.StartLong:
                _triggersState[trigger] = true;
                break;
            case PAnimatorStates.Unblock:
                for (int i = 0; i < ANIMATOR_STATES_COUNT; i++)
                {
                    _triggersState[(PAnimatorStates)i] = true;

                    ResetTriggers();
                }
                break;
            default:
                //Debug.Log("joe biden wake up");
                break;
        }
    }
}

