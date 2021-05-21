using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEyes : MonoBehaviour
{
    public List<Transform> VisibleTargets { get { return _visibleTargets; } }

    public float ViewRadius { get { return _viewRadius; } }
    public float ViewAngle { get { return _viewAngle; } }

    [SerializeField]
    private float _viewRadius;
    [SerializeField]
    private float _viewAngle;

    [Tooltip("Layers of entities that this entity should follow")]
    [SerializeField]
    private LayerMask _entitiesMask;
    [Tooltip("Layers of objects or entites that this entity wouldn't be able to see through")]
    [SerializeField]
    private LayerMask _obstaclesMask;

    /// <summary>
    /// targets that current entity sees at current frame
    /// </summary>
    private List<Transform> _visibleTargets = new List<Transform>();

    /// <summary>
    /// Finds visible targets for current entity
    /// </summary>
    public void FindVisibleTargets()
    {
        _visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _entitiesMask);

        foreach (Collider targetCollider in targetsInViewRadius)
        {
            Transform targetTransform = targetCollider.transform;
            Vector3 dirToTarget = targetTransform.position - transform.position;
            dirToTarget.Normalize();
            Debug.Log(Vector3.Angle(transform.forward, dirToTarget));
            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                
                float distance = Vector3.Distance(transform.position, targetTransform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distance, _obstaclesMask))
                {
                    _visibleTargets.Add(targetTransform);
                }
            }   
        }
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
}
