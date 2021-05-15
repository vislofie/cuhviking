using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitManager : MonoBehaviour
{
    [SerializeField]
    private EntityCombat _combatController;

    private bool _enabled;


    #region UNITY BUILT-IN METHODS
    private void OnCollisionStay(Collision collision)
    {
        if (_enabled)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                Debug.Log(collision.gameObject.name);
                _combatController.ReceiveHitSignal(collision.gameObject.GetComponent<EntityCombat>());
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_enabled)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                Debug.Log(collision.gameObject.name);
                _combatController.ReceiveHitSignal(collision.gameObject.GetComponent<EntityCombat>());
            }
        }
    }
    #endregion

    /// <summary>
    /// Disables ability to detect collisions
    /// </summary>
    public void Disable()
    {
        _enabled = false;
    }

    /// <summary>
    /// Enables ability to detect collisions
    /// </summary>
    public void Enable()
    {
        _enabled = true;
    }
}
