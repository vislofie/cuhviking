using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Weapon Damages
/// </summary>
public enum WeaponDamages
{
    STICK = 20,
    HEAD = 10
};
public class EntityCombat : MonoBehaviour
{
    [Tooltip("tells either this combat system belongs to player or nah")]
    [SerializeField]
    private bool _playersCombat;

    private PlayerBrain _playerBrain;
    private EnemyBrain _enemyBrain;

    private void Awake()
    {
        if (_playersCombat) _playerBrain = GetComponent<PlayerBrain>();
        else                _enemyBrain = GetComponent<EnemyBrain>();
    }

    /// <summary>
    /// ReceiveDamage. ReceivesDamage from an entity
    /// </summary>
    /// <param name="damage">amount of damage</param>
    /// <param name="senderEntityCombat">EntityCombat of an entity who sent the damage</param>
    public void ReceiveDamage(int damage, EntityCombat senderEntityCombat)
    {
        // TODO: Send damage to Brain
        if (_playersCombat) _playerBrain.ReceiveDamage(damage, senderEntityCombat);
        else                _enemyBrain.ReceiveDamage(damage, senderEntityCombat);
    }

    /// <summary>
    /// SendDamage. Sends damage to an entity
    /// </summary>
    /// <param name="damage">amount of damage</param>
    /// <param name="otherEntityCombat">EntityCombat of an entity who is going to receive the damage</param>
    public void SendDamage(int damage,  EntityCombat otherEntityCombat)
    {
        otherEntityCombat.ReceiveDamage(damage, this);
    }
    
    /// <summary>
    /// ReceiveHitSignal. Gets a signal from WeaponHitManager about a hit from entity
    /// </summary>
    /// <param name="combatController">EntityCombat that got hit</param>
    public void ReceiveHitSignal(EntityCombat combatController)
    {
        Debug.Log("RECEIVED HIT SIGNAL");
        if (_playersCombat) _playerBrain.SendDamageToEnemy(combatController);
        else _enemyBrain.SendDamageToEntity(combatController);
    }

    
}
