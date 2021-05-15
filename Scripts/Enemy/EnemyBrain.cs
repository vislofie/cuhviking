using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private Slider _healthBar;

    private EntityCharacteristics _chars;
    private EntityCombat _combatController;


    private void Awake()
    {
        _chars = this.GetComponent<EntityCharacteristics>();
        _chars.SetMaxHealth(100);
        _chars.SetMaxStamina(100);

        _chars.ChangeHealth(100);
        _chars.ChangeStamina(100);

        UpdateHealthInUI();
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
