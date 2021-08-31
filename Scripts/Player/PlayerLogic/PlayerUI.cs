using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Slider _healthBar;
    [SerializeField]
    private Slider _staminaBar;

    public void UpdateHealthStatus(float value, float maxValue)
    {
        _healthBar.value = value / maxValue;
    }

    public void UpdateStaminaStatus(float value, float maxValue)
    {
        _staminaBar.value = value / maxValue;
    }
}
