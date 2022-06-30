using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChars : EntityCharacteristics
{
    private PlayerBrain _brain;

    private float _maxHunger;
    private float _maxThirst;

    private float _hunger;
    private float _thirst;

    [SerializeField]
    private float _hungerSpeed = 0.1f;
    [SerializeField]
    private float _thirstSpeed = 0.1f;

    public float MaxHunger { get { return _maxHunger; } }
    public float MaxThirst { get { return _maxThirst; } }

    public float Hunger
    {
        get
        {
            return _hunger;
        }
        set
        {
            if (_hunger >= 0) _hunger = Mathf.Clamp(value, 0, _maxHunger);
            OnHungerChange();
        }
    }
    public float Thirst
    {
        get
        {
            return _thirst;
        }
        set
        {
            if (_thirst >= 0) _thirst = Mathf.Clamp(value, 0, _maxThirst);
            OnThirstChange();
        }
    }

    public float HungerSpeed 
    { 
        get 
        { 
            return _hungerSpeed; 
        } 
        set 
        { 
            if (_hungerSpeed >= 0) _hungerSpeed = value; 
        } 
    }
    public float ThirstSpeed 
    { 
        get 
        { 
            return _thirstSpeed; 
        }
        set 
        {
            if (_thirstSpeed >= 0) _thirstSpeed = value; 
        } 
    }

    private void Awake()
    {
        _brain = this.GetComponent<PlayerBrain>();

        _maxHunger = 100.0f;
        _maxThirst = 100.0f;

        _hunger = _maxHunger;
        _thirst = _maxThirst;
    }

    private void Update()
    {
        Hunger -= _hungerSpeed * Time.deltaTime;
        Thirst -= _thirstSpeed * Time.deltaTime;
    }

    public void OnHungerChange()
    {
        _brain.UpdateHungerInUI();
    }

    public void OnThirstChange()
    {
        _brain.UpdateThirstInUI();
    }


    #region INHERITED-EVENTS
    public override void OnHealthChange(float healthDelta)
    {
        _brain.UpdateHealthInUI();
    }

    public override void OnHealthChange()
    {
        _brain.UpdateHealthInUI();
    }

    public override void OnStaminaChange(float staminaDelta)
    {
        _brain.UpdateStaminaInUI();
    }

    public override void OnStaminaChange()
    {
        _brain.UpdateStaminaInUI();
    }
    #endregion
}
