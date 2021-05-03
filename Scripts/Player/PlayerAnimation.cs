using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorTriggers
{
    QuickHit, CancelLong, StartLong, ProceedLong
};

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Dictionary<AnimatorTriggers, bool> _triggersState = new Dictionary<AnimatorTriggers, bool>(); // if animation is allowed to be played the value of a key is true, if not then it is false

    private Animator _animator;

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
            _triggersState.Add((AnimatorTriggers)i, true);

        _animator = this.gameObject.GetComponent<Animator>();
    }

    public void ActivateTrigger(AnimatorTriggers trigger)
    {
        if (_triggersState[trigger] == true)
        {
            switch (trigger)
            {
                case AnimatorTriggers.CancelLong:
                    _animator.SetTrigger("CancelLong");
                    break;
                case AnimatorTriggers.ProceedLong:
                    _animator.SetTrigger("ProceedLong");
                    _triggersState[trigger] = false;
                    break;
                case AnimatorTriggers.QuickHit:
                    _animator.SetTrigger("QuickHit");
                    _triggersState[trigger] = false;
                    break;
                case AnimatorTriggers.StartLong:
                    _animator.SetTrigger("StartLong");
                    _triggersState[trigger] = false;
                    break;
                default:
                    Debug.Log("bruh you dum as hell like fr");
                    break;
            }
        }
        else
        {
            Debug.Log("bibibobo you tried to play the animation that is already playing!");
        }   
    }

    public void OnAnimationEnd(AnimatorTriggers trigger)
    {
        switch (trigger)
        {
            case AnimatorTriggers.CancelLong:
                break;
            case AnimatorTriggers.ProceedLong:
                _triggersState[trigger] = true;
                break;
            case AnimatorTriggers.QuickHit:
                _triggersState[trigger] = true;
                break;
            case AnimatorTriggers.StartLong:
                _triggersState[trigger] = true;
                break;
            default:
                Debug.Log("joe biden wake up");
                break;
        }
    }
}
