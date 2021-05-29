﻿using System.Collections;
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
    private EnemySenses _senses;
    private EntityCombat _combatController;
    private EnemyMovement _movementController;

    // a position that enemy is moving towards that is not an entity
    private Vector3 _pointOfInterest;

    // whether this enemy is moving to target at the moment
    private bool _movingToTarget;


    private void Awake()
    {
        _senses = this.GetComponent<EnemySenses>();
        _chars = this.GetComponent<EntityCharacteristics>();
        _movementController = this.GetComponent<EnemyMovement>();
        _chars.SetMaxHealth(100);
        _chars.SetMaxStamina(100);

        _chars.ChangeHealth(100);
        _chars.ChangeStamina(100);

        UpdateHealthInUI();
    }

    private void Start()
    {
        _movingToTarget = false;
    }

    private void FixedUpdate()
    {
        _senses.FindVisibleTargets();
        List<Transform> visibleTargets = _senses.VisibleTargets;
        if (visibleTargets.Count > 0)
        {
            Transform closestTarget = visibleTargets[0];
            foreach (Transform target in visibleTargets)
                if (Vector3.Distance(transform.position, target.position) < Vector3.Distance(transform.position, closestTarget.position))
                    closestTarget = target;

            if (Vector3.Distance(transform.position, closestTarget.position) >= _enemyToTargetDistanceLimit)
            {
                _movementController.MoveToTarget(closestTarget);
                _movingToTarget = true;
            }
            else
            {
                _movingToTarget = false;
            }
        }
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
        Debug.Log(obj.name + " made noise and " + gameObject.name + "heard it");
        _pointOfInterest = obj.transform.position;
        if (!_movingToTarget)
            _movementController.MoveToPosition(_pointOfInterest);
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
