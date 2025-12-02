using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;

    public int CurrentHealth { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;

    public delegate void HealthChanged(int current);
    public event HealthChanged OnHealthChanged;

    private void Start()
    {
        CurrentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        CurrentHealth -= amount;
        if (CurrentHealth < 0) CurrentHealth = 0;

        UpdateUI();
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void UpdateUI()
    {
        if (healthText != null)
            healthText.text = CurrentHealth.ToString();
    }
}
