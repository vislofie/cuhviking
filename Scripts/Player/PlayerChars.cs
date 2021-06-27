using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChars : EntityCharacteristics
{
    private PlayerBrain _brain;

    private void Awake()
    {
        _brain = this.GetComponent<PlayerBrain>();
    }

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
}
