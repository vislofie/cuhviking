using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    private Transform _startPlatform;
    [SerializeField]
    private Transform _endPlatform;

    private int _currentStep;

    private int _stepCount;
    private Transform[] _steps;
    private Transform _playerTransform;

    private void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        Transform stepHolder = transform.GetChild(0);
        _stepCount = stepHolder.childCount;
        _steps = new Transform[_stepCount];

        for (int i = 0; i < _stepCount; i++)
            _steps[i] = stepHolder.GetChild(i);

        _currentStep = -1;
    }

    /// <summary>
    /// Updates player's position on steps
    /// </summary>
    private void UpdatePlayerPosition()
    {
        _playerTransform.position = _steps[_currentStep].position + new Vector3(0.0f, 1.0f, 0.0f);
    }

    /// <summary>
    /// Finishes climbing
    /// </summary>
    /// <param name="down">if true, player gets on start platfrom, if false end platform</param>
    private void FinishClimbing(bool down)
    {
        if (down)
        {
            _playerTransform.position = _startPlatform.position;
        }
        else
        {
            _playerTransform.position = _endPlatform.position;
        }
        _playerTransform.GetComponent<PlayerBrain>().AllowMovement();
        _playerTransform.GetComponent<PlayerBrain>().RemoveLadder();
    }

    /// <summary>
    /// Activates climbing
    /// </summary>
    /// <param name="start">whether the player should start climbing from the start platform or the end platform</param>
    public void ActivateClimbing(bool start)
    {
        _currentStep = start ? 0 : _stepCount - 1;
        UpdatePlayerPosition();
    }

    /// <summary>
    /// Climbs up
    /// </summary>
    public void ClimbUp()
    {
        if (_currentStep < _stepCount - 1)
        {
            _currentStep++;
            UpdatePlayerPosition();
        }
        else
        {
            FinishClimbing(false);
        }
        
    }

    /// <summary>
    /// Climbs down
    /// </summary>
    public void ClimbDown()
    {
        if (_currentStep > 0)
        {
            _currentStep--;
            UpdatePlayerPosition();
        }    
        else
        {
            FinishClimbing(true);
        }
    }
}
