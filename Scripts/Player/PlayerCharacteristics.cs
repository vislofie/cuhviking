using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacteristics : MonoBehaviour
{
    public int Health { get { return _health; } }
    public int Stamina { get { return _stamina; } }

    public int MaxHealth { get { return _maxHealth; } }
    public int MaxStamina { get { return _maxStamina; } }

    private int _maxHealth;
    private int _maxStamina;

    private int _health;
    private int _stamina;

    private void Awake()
    {
        _health = 0;
        _stamina = 0;
    }

    public void SetMaxHealth(int value)
    {
        _maxHealth = value;
        if (_health > _maxHealth)
            _health = _maxHealth;

        Debug.Log("Player's new max health is " + _maxHealth);
    }

    public void SetMaxStamina(int value)
    {
        _maxStamina = value;
        if (_stamina > _maxStamina)
            _stamina = _maxStamina;

        Debug.Log("Player's new max stamina is " + _maxStamina);
    }

    public void ChangeHealth(int delta)
    {
        _health = Mathf.Clamp(_health + delta, 0, _maxHealth);
        if (_health == 0) Debug.Log("The player has died!");
    }

    public void ChangeStamina(int delta)
    {
        _stamina = Mathf.Clamp(_stamina + delta, 0, _maxStamina);
        if (_stamina == 0) Debug.Log("The player is out of stamina!");
        Debug.Log("Change stamina2 " + delta);
    }
}
