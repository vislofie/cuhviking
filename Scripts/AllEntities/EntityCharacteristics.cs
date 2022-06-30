using System.Collections;
using UnityEngine;

public class EntityCharacteristics : MonoBehaviour
{

    public float Health { get { return _health; } }
    public float Stamina { get { return _stamina; } }

    public float MaxHealth { get { return _maxHealth; } }
    public float MaxStamina { get { return _maxStamina; } }

    private float _maxHealth;
    private float _maxStamina;

    private float _health;
    private float _stamina;

    private IEnumerator _graduateHealthIncreaser;
    private IEnumerator _graduateStaminaIncreaser;

    private IEnumerator _healthIncreaserRestorer;
    private IEnumerator _staminaIncreaserRestorer;

    private IEnumerator _graduateHealthChanger;
    private IEnumerator _graduateStaminaChanger;

    private IEnumerator _graduateHealthChangerDisabler;
    private IEnumerator _graduateStaminaChangerDisabler;

    [SerializeField]
    private int _normalHealthRegenValue = 1;
    [SerializeField]
    private float _healthRegenStepTime = 1.0f;
    [SerializeField]
    private int _normalStaminaRegenValue = 1;
    [SerializeField]
    private float _staminaRegenStepTime = 1.0f;
    [SerializeField]
    private float _staminaDecreaseStepTime = 0.1f;

    public float AfterActionDelayTime { get { return _afterActionDelayTime; } }
    [Tooltip("Time it takes to restore health or stamina regen after some action")]
    [SerializeField]
    private float _afterActionDelayTime = 2.0f;

    #region SETTING-UP FUNCTIONS
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
        OnHealthChange();
    }

    public void SetMaxStamina(int value)
    {
        _maxStamina = value;
        if (_stamina > _maxStamina)
            _stamina = _maxStamina;
        OnStaminaChange();
    }
    #endregion

    #region CHANGING-VALUE FUNCTIONS
    public void ChangeHealth(float delta)
    {
        _health = Mathf.Clamp(_health + delta, 0, _maxHealth);
        OnHealthChange(delta);
    }

    public void ChangeStamina(float delta)
    {
        _stamina = Mathf.Clamp(_stamina + delta, 0, _maxStamina);
        OnStaminaChange(delta);
    }
    #endregion

    #region HEALTH
    /// <summary>
    /// Enables graduate health regen
    /// </summary>
    /// <param name="value">Value of adding stamina each step</param>
    /// <param name="stepTime">Value that defines how much time is one step going to take</param>
    public void EnableHealthRegen()
    {
        if (_graduateHealthIncreaser != null) StopCoroutine(_graduateHealthIncreaser);
        _graduateHealthIncreaser = GraduateChanger(_normalHealthRegenValue, _healthRegenStepTime, true);
        StartCoroutine(_graduateHealthIncreaser);
    }

    /// <summary>
    /// Disables graduate health regen
    /// </summary>
    public void DisableHealthRegen()
    {
        if (_graduateHealthIncreaser != null) StopCoroutine(_graduateHealthIncreaser);
        if (_healthIncreaserRestorer != null) StopCoroutine(_healthIncreaserRestorer);
    }

    
    /// <summary>
    /// Disables graduate health regen and enables it after some time
    /// </summary>
    /// <param name="timeToEnable">time it takes to turn it on again</param>
    public void DisableHealthRegen(float timeToEnable)
    {
        if (_graduateHealthIncreaser != null) StopCoroutine(_graduateHealthIncreaser);

        if (_healthIncreaserRestorer != null) StopCoroutine(_healthIncreaserRestorer);
        _healthIncreaserRestorer = RestoreIncreaser(_graduateHealthIncreaser, timeToEnable);
        StartCoroutine(_healthIncreaserRestorer);
    }
    
    /// <summary>
    /// Enables graduate health change by using the default step-time and this change works for the given timeToDisable
    /// </summary>
    /// <param name="value">value to change over step time</param>
    /// <param name="timeToDisable">time it takes to disable change</param>
    /// <param name="disableRegularRegen">if true, regular regen would not work while this change is going on</param>
    public void EnableHealthGraduateChange(float value, float timeToDisable, bool disableRegularRegen)
    {
        if (_graduateHealthChanger != null) StopCoroutine(_graduateHealthChanger);
        _graduateHealthChanger = GraduateChanger(value, _healthRegenStepTime, true);
        StartCoroutine(_graduateHealthChanger);

        if (_graduateHealthChangerDisabler != null) StopCoroutine(_graduateHealthChangerDisabler);
        _graduateHealthChangerDisabler = GraduateChangerDisabler(_graduateHealthChanger, timeToDisable);
        StartCoroutine(_graduateHealthChangerDisabler);

        if (disableRegularRegen) DisableHealthRegen(timeToDisable + _afterActionDelayTime);
    }

    /// <summary>
    /// Enables graduate health change by using the default step-time. This change needs to be shut down by DisableHealthGraduateChange()
    /// </summary>
    /// <param name="value">value to change over step time</param>
    /// <param name="disableRegularRegen">if true, regular regen would not work while this change is going on</param>
    public void EnableHealthGraduateChange(float value, bool disableRegularRegen)
    {
        if (_graduateHealthChanger != null) StopCoroutine(_graduateHealthChanger);
        _graduateHealthChanger = GraduateChanger(value, _healthRegenStepTime, false);
        StartCoroutine(_graduateHealthChanger);

        if (_graduateHealthChangerDisabler != null) StopCoroutine(_graduateHealthChangerDisabler);

        if (disableRegularRegen) DisableHealthRegen();
    }

    public void DisableHealthGraduateChange()
    {
        if (_graduateHealthChanger != null) StopCoroutine(_graduateHealthChanger);
        DisableHealthRegen(_afterActionDelayTime);
    }
    #endregion

    #region STAMINA
    /// <summary>
    /// Enables graduate stamina regen
    /// </summary>
    public void EnableStaminaRegen()
    {
        if (_graduateStaminaIncreaser != null) StopCoroutine(_graduateStaminaIncreaser);
        _graduateStaminaIncreaser = GraduateChanger(_normalStaminaRegenValue, _staminaRegenStepTime, false);
        StartCoroutine(_graduateStaminaIncreaser);
    }

    /// <summary>
    /// Disables graduate stamina regen
    /// </summary>
    public void DisableStaminaRegen()
    {
        if (_graduateStaminaIncreaser != null) StopCoroutine(_graduateStaminaIncreaser);
        if (_staminaIncreaserRestorer != null) StopCoroutine(_staminaIncreaserRestorer);
    }

    /// <summary>
    /// Disables graduate stamina regen and enables it after some time
    /// </summary>
    /// <param name="timeToEnable">time it takes to turn it on again</param>
    public void DisableStaminaRegen(float timeToEnable)
    {
        if (_graduateStaminaIncreaser != null) StopCoroutine(_graduateStaminaIncreaser);

        if (_staminaIncreaserRestorer != null) StopCoroutine(_staminaIncreaserRestorer);
        _staminaIncreaserRestorer = RestoreIncreaser(_graduateStaminaIncreaser, timeToEnable);
        StartCoroutine(_staminaIncreaserRestorer);
    }

    /// <summary>
    /// Enables graduate stamina change by using the default step-time and this change works for the given timeToDisable
    /// </summary>
    /// <param name="value">value to change over step time</param>
    /// <param name="timeToDisable">time it takes to disable change</param>
    /// <param name="disableRegularRegen">if true, regular regen would not work while this change is going on</param>
    public void EnableStaminaGraduateChange(float value, float timeToDisable, bool disableRegularRegen)
    {
        if (_graduateStaminaChanger != null) StopCoroutine(_graduateStaminaChanger);
        _graduateStaminaChanger = GraduateChanger(value, _staminaDecreaseStepTime, false);
        StartCoroutine(_graduateStaminaChanger);

        if (_graduateStaminaChangerDisabler != null) StopCoroutine(_graduateStaminaChangerDisabler);
        _graduateStaminaChangerDisabler = GraduateChangerDisabler(_graduateStaminaChanger, timeToDisable);
        StartCoroutine(_graduateStaminaChangerDisabler);

        if (disableRegularRegen) DisableStaminaRegen(timeToDisable + _afterActionDelayTime);
    }

    /// <summary>
    /// Enables graduate stamina change by using the default step-time. This change needs to be shut down by DisableStaminaGraduateChange()
    /// </summary>
    /// <param name="value">value to change over step time</param>
    /// <param name="disableRegularRegen">if true, regular regen would not work while this change is going on</param>
    public void EnableStaminaGraduateChange(float value, bool disableRegularRegen)
    {
        if (_graduateStaminaChanger != null) StopCoroutine(_graduateStaminaChanger);
        _graduateStaminaChanger = GraduateChanger(value, _staminaDecreaseStepTime, false);
        StartCoroutine(_graduateStaminaChanger);

        if (_graduateStaminaChangerDisabler != null) StopCoroutine(_graduateStaminaChangerDisabler);

        if (disableRegularRegen) DisableStaminaRegen();
    }

    public void DisableStaminaGraduateChange()
    {
        if (_graduateStaminaChanger != null) StopCoroutine(_graduateStaminaChanger);
        DisableStaminaRegen(_afterActionDelayTime);
    }
    #endregion

    #region COROUTINES
    /// <summary>
    /// Gradually changes health or stamina 
    /// </summary>
    /// <param name="value">value that is going to change per step</param>
    /// <param name="stepTime">time it takes to make one step</param>
    /// <param name="health">if true, increases health else increases stamina</param>
    protected IEnumerator GraduateChanger(float value, float stepTime, bool health)
    {
        while (true)
        {
            if (health) ChangeHealth(value);
            else        ChangeStamina(value);
            yield return new WaitForSeconds(stepTime);
        }
    }

    /// <summary>
    /// Restores GraduateChanger by given IEnumerator in given time
    /// </summary>
    /// <param name="increaser">GraduateChanger to turn on</param>
    /// <param name="timeToEnable">Time it takes to enable the increaser</param>
    protected IEnumerator RestoreIncreaser(IEnumerator increaser, float timeToEnable)
    {
        yield return new WaitForSeconds(timeToEnable);
        StartCoroutine(increaser);
    }

    /// <summary>
    /// Disables GraduateChanger by given IEnumerator in given time
    /// </summary>
    /// <param name="changer"></param>
    /// <param name="timeToDisable"></param>
    /// <returns></returns>
    protected IEnumerator GraduateChangerDisabler(IEnumerator changer, float timeToDisable)
    {
        yield return new WaitForSeconds(timeToDisable);
        if (changer != null) StopCoroutine(changer);
    }
    #endregion

    #region INHERITABLE EVENTS
    public virtual void OnHealthChange(float healthDelta)
    {
        // idk what to write here tbh
    }

    public virtual void OnHealthChange()
    {
        // idk what to write here tbh
    }

    public virtual void OnStaminaChange(float staminaDelta)
    {
        // idk what to write here tbh
    }

    public virtual void OnStaminaChange()
    {
        // idk what to write here tbh
    }
    #endregion
}
