using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitManager : MonoBehaviour
{
    [SerializeField]
    private EntityCombat _combatController;

    private bool _enabled;


    #region UNITY BUILT-IN METHODS
    private void OnTriggerStay(Collider collider)
    {
        if (_enabled)
        {
            if (collider.CompareTag("Enemy"))
            {
                Debug.Log(collider.gameObject.name);
                _combatController.ReceiveHitSignal(collider.gameObject.GetComponent<EntityCombat>());
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (_enabled)
        {
            if (collider.CompareTag("Enemy"))
            {
                Debug.Log(collider.gameObject.name);
                _combatController.ReceiveHitSignal(collider.gameObject.GetComponent<EntityCombat>());
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
