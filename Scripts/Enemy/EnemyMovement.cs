using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = this.GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Moves to target using NavMeshAgent.
    /// </summary>
    /// <param name="targetTransform">transform of a target</param>
    public void MoveToTarget(Transform targetTransform)
    {
        _agent.SetDestination(targetTransform.position);
    }

    /// <summary>
    /// Moves to position using NavMeshAgent
    /// </summary>
    /// <param name="position">position to move to</param>
    public void MoveToPosition(Vector3 position)
    {
        _agent.SetDestination(position);
    }
}
