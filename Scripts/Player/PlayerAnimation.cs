using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorStates
{
    QuickHit, CancelLong, StartLong, ProceedLong, Block, Unblock
};

public class PlayerAnimation : MonoBehaviour
{
    private const int ANIMATOR_STATES_COUNT = 6;

    private PlayerBrain _brain;

    [SerializeField]
    private Dictionary<AnimatorStates, bool> _triggersState = new Dictionary<AnimatorStates, bool>(); // if animation is allowed to be played the value of a key is true, if not then it is false

    private Animator _animator;

    private void Awake()
    {
        for (int i = 0; i < ANIMATOR_STATES_COUNT; i++)
            _triggersState.Add((AnimatorStates)i, true);

        _animator = this.gameObject.GetComponent<Animator>();
        _brain = this.gameObject.GetComponent<PlayerBrain>();
    }

    private void ResetTriggers()
    {
        _animator.ResetTrigger("QuickHit");
        _animator.ResetTrigger("CancelLong");
        _animator.ResetTrigger("StartLong");
        _animator.ResetTrigger("ProceedLong");
        _animator.ResetTrigger("Block");
    }

    public void ActivateTrigger(AnimatorStates trigger)
    {
        if (_triggersState[trigger] == true)
        {
            switch (trigger)
            {
                case AnimatorStates.Block:
                    _animator.SetTrigger("Block");
                    _triggersState[trigger] = false;
                    _brain.ChangeStamina(PlayerBrain.S_PUNISH_BLOCK);
                    break;
                case AnimatorStates.CancelLong:
                    _animator.SetTrigger("CancelLong");
                    break;
                case AnimatorStates.ProceedLong:
                    _animator.SetTrigger("ProceedLong");
                    _triggersState[trigger] = false;
                    _brain.ChangeStamina(PlayerBrain.S_PUNISH_LONG_HIT_FINISHED);
                    break;
                case AnimatorStates.QuickHit:
                    _animator.SetTrigger("QuickHit");
                    _triggersState[trigger] = false;
                    _brain.ChangeStamina(PlayerBrain.S_PUNISH_QUICK_HIT);
                    break;
                case AnimatorStates.StartLong:
                    _animator.SetTrigger("StartLong");
                    _triggersState[trigger] = false;
                    _brain.ChangeStamina(PlayerBrain.S_PUNISH_LONG_HIT_N_FINISHED);
                    break;
                case AnimatorStates.Unblock:
                    _animator.SetTrigger("Unblock");
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

    public void OnAnimationEnd(AnimatorStates trigger)
    {
        switch (trigger)
        {
            case AnimatorStates.Block:
                break;
            case AnimatorStates.CancelLong:
                break;
            case AnimatorStates.ProceedLong:
                _triggersState[trigger] = true;
                _triggersState[AnimatorStates.StartLong] = true;
                break;
            case AnimatorStates.QuickHit:
                _triggersState[trigger] = true;
                break;
            case AnimatorStates.StartLong:
                _triggersState[trigger] = true;
                break;
            case AnimatorStates.Unblock:
                for (int i = 0; i < ANIMATOR_STATES_COUNT; i++)
                {
                    _triggersState[(AnimatorStates)i] = true;

                    ResetTriggers();
                }
                break;
            default:
                //Debug.Log("joe biden wake up");
                break;
        }
    }
}

