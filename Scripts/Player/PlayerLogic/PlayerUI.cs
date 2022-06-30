using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Slider _healthBar;
    [SerializeField]
    private Slider _staminaBar;
    [SerializeField]
    private Slider _hungerBar;
    [SerializeField]
    private Slider _thirstBar;

    public void UpdateHealthStatus(float value, float maxValue)
    {
        _healthBar.value = value / maxValue;
    }

    public void UpdateStaminaStatus(float value, float maxValue)
    {
        _staminaBar.value = value / maxValue;
    }

    public void UpdateHungerStatus(float value, float maxValue)
    {
        _hungerBar.value = value / maxValue;
    }

    public void UpdateThirstStatus(float value, float maxValue)
    {
        _thirstBar.value = value / maxValue;
    }
}
