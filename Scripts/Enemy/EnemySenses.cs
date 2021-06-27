using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySenses : MonoBehaviour
{
    /// <summary>
    /// Types of entities that enemy can hear
    /// </summary>
    public enum EntityHearTypes { Player };

    private EnemyBrain _brain;

    public Transform ClosestVisibleTarget
    {
        get
        {
            if (_visibleTargets.Count > 0)
            {
                float shortestDistance = Vector3.Distance(transform.position, _visibleTargets[0].position);
                Transform closestTarget = _visibleTargets[0];
                foreach (Transform target in _visibleTargets)
                {
                    float curDist = Vector3.Distance(transform.position, target.position);
                    if (curDist < shortestDistance)
                    {
                        shortestDistance = curDist;
                        closestTarget = target;
                    }
                }
                return closestTarget;
            }
            else
            {
                return null;
            }
        }
    }
    public List<Transform> VisibleTargets { get { return _visibleTargets; } }

    public float NormalHearRadius { get { return _normalHearRadius; } }
    public float SneakingHearRadius { get { return _sneakingHearRadius; } }
    public float NormalViewRadius { get { return _normalViewRadius; } }
    public float SneakingViewRadius { get { return _sneakingViewRadius; } }
    public float ViewAngle { get { return _viewAngle; } }

    [SerializeField]
    private float _normalHearRadius = 5;
    [SerializeField]
    private float _sneakingHearRadius = 3; // hearRadius for hearing entities that are sneaking
    [SerializeField]
    private float _normalViewRadius = 10;
    [SerializeField]
    private float _sneakingViewRadius = 5; // viewRadius for seeing entities that are sneaking
    [SerializeField]
    private float _viewAngle;

    [Tooltip("Layers of entities that this entity should follow")]
    [SerializeField]
    private LayerMask _entitiesMask;
    [Tooltip("Layers of objects or entites that this entity wouldn't be able to see through")]
    [SerializeField]
    private LayerMask _obstaclesMask;

    [Tooltip("Trigger that hears other entities")]
    [SerializeField]
    private SphereCollider _hearTrigger;

    /// <summary>
    /// targets that current entity sees at current frame
    /// </summary>
    private List<Transform> _visibleTargets = new List<Transform>();

    private void Awake()
    {
        _brain = this.GetComponent<EnemyBrain>();
        _hearTrigger.radius = _normalHearRadius;
    }

    #region FOV
    /// <summary>
    /// Finds visible targets for current entity
    /// </summary>
    public void FindVisibleTargets()
    {
        _visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _normalViewRadius, _entitiesMask);

        foreach (Collider targetCollider in targetsInViewRadius)
        {
            Transform targetTransform = targetCollider.transform;
            Vector3 dirToTarget = targetTransform.position - transform.position;
            dirToTarget.Normalize();
            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float distance = Vector3.Distance(transform.position, targetTransform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distance, _obstaclesMask))
                {
                    if (!targetTransform.CompareTag("Player"))
                    {
                        _visibleTargets.Add(targetTransform);
                        continue;
                    }

                    if (targetTransform.GetComponent<PlayerBrain>().PlayerMovementState == MovementState.Crouching)
                    {
                        if (distance <= _sneakingViewRadius)
                        {
                            Debug.Log("distance <= sneakingViewRadius");
                            _visibleTargets.Add(targetTransform);
                        }
                    }
                    else
                    {
                        _visibleTargets.Add(targetTransform);
                    }
                }
            }   
        }
    }

    /// <summary>
    /// Tells whether the target is inside view radius and is not hidden by any obstacles
    /// </summary>
    /// <param name="target"></param>
    /// <returns>true if the target is inside the zone and false if not</returns>
    public bool InsideTheZone(Transform target)
    {
        Vector3 dirToTarget = target.position - transform.position;
        float distance = Vector3.Distance(transform.position, target.position);
        if (!Physics.Raycast(transform.position, dirToTarget, distance, _obstaclesMask))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Creates a direction vector from a given angle
    /// </summary>
    /// <param name="angleInDegrees"></param>
    /// <param name="angleIsGlobal"></param>
    /// <returns></returns>
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion

    #region EARS
    /// <summary>
    /// Receives a signal from EarsTrigger about an enity that was "heard".
    /// </summary>
    /// <param name="type">Type of entity</param>
    /// <param name="entityObj">GameObject of an entity</param>
    public void ReceiveTriggerAdd(EntityHearTypes type, GameObject entityObj)
    {
        switch(type)
        {
            case EntityHearTypes.Player:
                    _brain.AddToHearList(type, entityObj);
                break;
            default:
                break;
        }
    }

    public void ReceiveTriggerRemove(EntityHearTypes type, GameObject entityObj)
    {
        switch(type)
        {
            case EntityHearTypes.Player:
                _brain.RemoveFromHearList(type, entityObj);
                break;
            default:
                break;
        }
    }
    #endregion
}
