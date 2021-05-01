using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimations
{
    QuickHit
};

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Dictionary<PlayerAnimations, bool> _animationsState; // if animation is allowed to be played the value of a key is true, if not then it is false

    private Animator _animator;

    private void Awake()
    {
        _animationsState.Add(PlayerAnimations.QuickHit, true);
        _animator = this.gameObject.GetComponent<Animator>();
    }

    public void PlayAnimation(PlayerAnimations animation)
    {
        if (_animationsState[animation] == true)
        {
            switch (animation)
            {
                case PlayerAnimations.QuickHit:
                    _animator.SetTrigger("QuickHit");
                    _animationsState[animation] = false;
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

    public void OnAnimationEnd(PlayerAnimations animation)
    {
        switch (animation)
        {
            case PlayerAnimations.QuickHit:
                _animationsState[animation] = true;
                break;
            default:
                Debug.Log("wake up");
                break;
        }
    }
}
