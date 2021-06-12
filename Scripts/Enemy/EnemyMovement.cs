using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent _agent;

    // a variable that stores Rotator coroutine
    private IEnumerator _rotCoroutine;

    private void Awake()
    {
        _agent = this.GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// tells whether this agent has reached its destination or doesnt have a destination at all
    /// </summary>
    /// <returns>true if reached, false if nah</returns>
    public bool IsIdle()
    {
        if ((_agent.pathStatus == NavMeshPathStatus.PathComplete && _agent.remainingDistance == 0) || (_agent.pathStatus == NavMeshPathStatus.PathInvalid))
            return true;
        return false;

    }

    public bool IsStopped()
    {
        return _agent.isStopped;
    }

    /// <summary>
    /// Stops movement of the player
    /// </summary>
    public void StopMovement()
    {
        _agent.isStopped = true;
    }

    public void AllowMovement()
    {
        _agent.isStopped = false;
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

    /// <summary>
    /// Rotates this entity smoothly through a coroutine at a given angle around Y axis
    /// </summary>
    /// <param name="angle">angle of rotation</param>
    public void Rotate(float angle)
    {
        DisableRotator();

        Quaternion newRot = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + angle, transform.rotation.eulerAngles.z));
        _rotCoroutine = Rotator(newRot);
        StartCoroutine(_rotCoroutine);
    }

    /// <summary>
    /// Disables the coroutine that rotates this entity
    /// </summary>
    public void DisableRotator()
    {
        if (_rotCoroutine != null)
            StopCoroutine(_rotCoroutine);
    }

    /// <summary>
    /// Gradual rotation of an entity. Starts from Rotate method
    /// </summary>
    /// <param name="rot"></param>
    /// <returns></returns>
    IEnumerator Rotator(Quaternion rot)
    {
        while (transform.rotation != rot)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 2.0f);
            yield return new WaitForEndOfFrame();
        }
    }
}
