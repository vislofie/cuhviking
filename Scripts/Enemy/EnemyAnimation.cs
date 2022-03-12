using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAnimatorStates
{
    Hit
}

public class EnemyAnimation : MonoBehaviour
{
    private const int ANIMATOR_STATES_COUNT = 1;

    private EnemyBrain _brain;

    [SerializeField]
    private Dictionary<EAnimatorStates, bool> _triggersState = new Dictionary<EAnimatorStates, bool>(); // if animation is allowed to be played the value of a key is true, if not then it is false

    private Animator _animator;

    private void Awake()
    {
        for (int i = 0; i < ANIMATOR_STATES_COUNT; i++)
            _triggersState.Add((EAnimatorStates)i, true);

        _animator = GetComponent<Animator>();
        _brain = GetComponent<EnemyBrain>();
    }

    private void ResetTriggers()
    {
        _animator.ResetTrigger("Hit");
    }

    public void ActivateTrigger(EAnimatorStates trigger)
    {
        if (_triggersState[trigger] == true)
        {
            switch(trigger)
            {
                case EAnimatorStates.Hit:
                    _animator.SetTrigger("Hit");
                    _triggersState[EAnimatorStates.Hit] = false;
                    _brain.ForbidAttacks();
                    break;
                default:
                    Debug.Log("Wrong trigger state in ActivateTrigger on " + this.name);
                    break;
            }
        }
    }

    public void OnAnimationEnd(EAnimatorStates trigger)
    {
        switch (trigger)
        {
            case EAnimatorStates.Hit:
                _triggersState[EAnimatorStates.Hit] = true;
                _brain.AllowAttacks();
                break;
            default:
                Debug.Log("Wrong trigger state in OnAnimationEnd on " + this.name);
                break;
        }
    }
}
