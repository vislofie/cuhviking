using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private float _enemyToTargetDistanceLimit = 0.1f;

    [SerializeField]
    private Slider _healthBar;
    


    private EntityCharacteristics _chars;
    private EnemyEyes _eyes;
    private EntityCombat _combatController;
    private EnemyMovement _movementController;


    private void Awake()
    {
        _eyes = this.GetComponent<EnemyEyes>();
        _chars = this.GetComponent<EntityCharacteristics>();
        _movementController = this.GetComponent<EnemyMovement>();
        _chars.SetMaxHealth(100);
        _chars.SetMaxStamina(100);

        _chars.ChangeHealth(100);
        _chars.ChangeStamina(100);

        UpdateHealthInUI();
    }

    private void FixedUpdate()
    {
        _eyes.FindVisibleTargets();
        List<Transform> visibleTargets = _eyes.VisibleTargets;
        if (visibleTargets.Count > 0)
        {
            Transform closestTarget = visibleTargets[0];
            foreach (Transform target in visibleTargets)
                if (Vector3.Distance(transform.position, target.position) < Vector3.Distance(transform.position, closestTarget.position))
                    closestTarget = target;

            if (Vector3.Distance(transform.position, closestTarget.position) >= _enemyToTargetDistanceLimit)
                _movementController.MoveToTarget(closestTarget);
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


    public void UpdateHealthInUI ()
    {
        _healthBar.value = _chars.Health / (float)_chars.MaxHealth;
    }
}
