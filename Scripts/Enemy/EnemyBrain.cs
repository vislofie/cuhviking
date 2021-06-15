using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AIState { Idle, MovingToPoI, MovingToTarget, EngagingTarget, LookingAround, Patroling, Dead }
public class EnemyBrain : MonoBehaviour
{
    public AIState CurrentState { get { return _currentState; } }

    [SerializeField]
    private AIState _currentState;

    [SerializeField]
    private Slider _healthBar;

    private EntityCharacteristics _chars;
    private EnemySenses _senses;
    private EntityCombat _combatController;
    private EnemyMovement _movementController;

    // a position that enemy is moving towards that is not an entity
    private Vector3 _pointOfInterest;

    private Transform _targetToFollow;

    [SerializeField]
    private float _closestToTargetDistance = 0.5f;

    [Tooltip("Overall time for rotating and looking. \n" +
             "Formula for one rotation: lookingAroundTime / 3 - searchingTime\n" +
             "For example:\n" +
             "if lookingAroundTime = 6 and searchingTime = 1 then enemy will rotate for 1 second and look for 1 second")]
    [SerializeField]
    private float _lookingAroundTime = 3.0f;
    [Tooltip("Time to look after enemy has rotated")]
    [SerializeField]
    private float _searchingTime = 1.0f;

    private void Awake()
    {
        _senses = this.GetComponent<EnemySenses>();
        _chars = this.GetComponent<EntityCharacteristics>();
        _movementController = this.GetComponent<EnemyMovement>();

        _chars.SetMaxHealth(100);
        _chars.SetMaxStamina(100);

        _chars.ChangeHealth(100);
        _chars.ChangeStamina(100);

        _currentState = AIState.Idle;
        _targetToFollow = null;

        UpdateHealthInUI();
    }

    private void FixedUpdate()
    {
        FindVisibleTargetsAndAct();
    }


    /// <summary>
    /// Finds visible targets and acts dependant on what it has seen
    /// </summary>
    private void FindVisibleTargetsAndAct()
    {
        _senses.FindVisibleTargets();

        bool wasTargetBefore = false;
        Vector3 lastTargetToFollowPosition = Vector3.zero;

        if (_targetToFollow != null)
        {
            if (!_senses.InsideTheZone(_targetToFollow))
            {
                lastTargetToFollowPosition = _targetToFollow.position;
                _targetToFollow = null;
                wasTargetBefore = true;
            }
        }

        List<Transform> visibleTargets = _senses.VisibleTargets;

        if (visibleTargets.Count > 0)
        {
            float minDistance = Vector3.Distance(transform.position, visibleTargets[0].position);
            foreach (Transform target in visibleTargets)
            {
                if (minDistance >= Vector3.Distance(target.position, transform.position))
                {
                    Debug.Log("DistanceToCur is bigguh");
                    _targetToFollow = target;
                    minDistance = Vector3.Distance(target.position, transform.position);
                }
            }

            
        }

        if (_targetToFollow == null)
        {
            if (wasTargetBefore)
            {
                MoveToLastSeenPosition(lastTargetToFollowPosition);
            }

            if (_movementController.IsIdle() && _currentState == AIState.MovingToPoI)
            {
                LookAround();
            }
        }
        else
        {
            bool agentStopped = _movementController.IsStopped();
            StopCoroutine(LookingAround());
            if (Vector3.Distance(transform.position, _targetToFollow.position) >= _closestToTargetDistance)
            {
                if (agentStopped)
                    _movementController.AllowMovement();
                MoveToTarget(_targetToFollow);
            }
            else
            {
                if (!agentStopped)
                    _movementController.StopMovement();
            }
        }

    }

    /// <summary>
    /// Starts the "Look around" event.
    /// </summary>
    private void LookAround()
    {
        _currentState = AIState.LookingAround;
        StartCoroutine(LookingAround());
    }

    /// <summary>
    /// Used for generating random angles and rotating enemy according to this angle to allow it to "look around"
    /// </summary>
    /// <returns></returns>
    private IEnumerator LookingAround()
    {
        int rotateCount = 0;
        while (rotateCount != 3)
        {
            float randAngle = Random.Range(0, 2) == 0 ? Random.Range(75, 180) : Random.Range(-75, -180);
            StartCoroutine(RotateInGivenTime(randAngle, _lookingAroundTime / 3 - _searchingTime));
            yield return new WaitForSeconds(_lookingAroundTime / 3);
            rotateCount++;
        }

        _currentState = AIState.Idle;
    }

    /// <summary>
    /// Rotates this enemy at the given angle and within the given time
    /// </summary>
    /// <param name="angle">angle of rotation</param>
    /// <param name="time">time, given to rotate</param>
    /// <returns></returns>
    private IEnumerator RotateInGivenTime(float angle, float time)
    {
        float timeBefore = Time.time;
        Quaternion newRot = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + angle, transform.rotation.eulerAngles.z));
        while (Time.time - timeBefore < time)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * time);
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Moves enemy to last seen position of a target
    /// </summary>
    /// <param name="position">last seen position</param>
    private void MoveToLastSeenPosition(Vector3 position)
    {
        _pointOfInterest = position;
        _movementController.MoveToPosition(_pointOfInterest);
        _currentState = AIState.MovingToPoI;
    }

    /// <summary>
    /// Moves to point of interest
    /// </summary>
    private void MoveToPointOfInterest()
    {
        _movementController.MoveToPosition(_pointOfInterest);
        _currentState = AIState.MovingToPoI;
    }

    /// <summary>
    /// Moves enemy to target
    /// </summary>
    /// <param name="target">target's transform</param>
    private void MoveToTarget(Transform target)
    {
        _movementController.MoveToTarget(target);
        _currentState = AIState.MovingToTarget;
    }

    /// <summary>
    /// Stops movement of this enemy
    /// </summary>
    private void StopMoving()
    {
        _movementController.StopMovement();
        _currentState = AIState.Idle;
    }


    public void UpdateHealthInUI()
    {
        _healthBar.value = _chars.Health / (float)_chars.MaxHealth;
    }

    /// <summary>
    /// Adds current gameObject to a list of "hearers" that belongs to the entityObj
    /// </summary>
    /// <param name="type">Type of entity</param>
    /// <param name="entityObj">GameObject of an entity</param>
    public void AddToHearList(EnemySenses.EntityHearTypes type, GameObject entityObj)
    {
        // TODO: add a delegate to entityeventhandler
        entityObj.GetComponent<EntityEventHandler>().AddHearer(ReceiveNoiseSignal);
    }

    /// <summary>
    /// Remove current gameObject from a list of "hearers" that belongs to the entityObj
    /// </summary>
    /// <param name="type">Type of entity</param>
    /// <param name="entityObj">GameObject of an entity</param>
    public void RemoveFromHearList(EnemySenses.EntityHearTypes type, GameObject entityObj)
    {
        entityObj.GetComponent<EntityEventHandler>().RemoveHearer(ReceiveNoiseSignal);
    }

    /// <summary>
    /// Called by delegate in entityEventHandler from an entity that was in hear radius and made a noise
    /// </summary>
    /// <param name="obj">GameObject of an entity that made the noise</param>
    public void ReceiveNoiseSignal(GameObject obj)
    {
        //Debug.Log(obj.name + " made noise and " + gameObject.name + "heard it");
        if (!(obj.CompareTag("Player") && obj.GetComponent<PlayerBrain>().PlayerMovementState == MovementState.Crouching &&
            Vector3.Distance(transform.position, obj.transform.position) > _senses.SneakingHearRadius * 2))
        {
            _pointOfInterest = obj.transform.position;
            MoveToPointOfInterest();
        }
    }

    /// <summary>
    /// ReceiveDamage. Gets damage and senderEntityCombat from EntityCombat script and works with it further.
    /// </summary>
    /// <param name="damage">amount of damage</param>
    /// <param name="senderEntityCombat">EntityCombat of an entity who sent the damage</param>
    public void ReceiveDamage(int damage, EntityCombat senderEntityCombat)
    {
        Debug.Log("Health before - " + _chars.Health);
        _chars.ChangeHealth(-damage);
        UpdateHealthInUI();
        Debug.Log("Heatlh after - " + _chars.Health);
    }


    
}
