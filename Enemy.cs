using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 20;
    public int damageToPlayer = 1;

    [Header("Rewards")]
    public int goldReward = 5;
    public int woodReward = 0;
    public int stoneReward = 0;
    public int foodReward = 0;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // Dodaj surowce graczowi
        ResourceManager resMgr = FindFirstObjectByType<ResourceManager>();
        ResourceUI resUI = FindFirstObjectByType<ResourceUI>();

        if (resMgr != null)
        {
            if (goldReward > 0) resMgr.EarnGold(goldReward);
            if (woodReward > 0) resMgr.EarnWood(woodReward);
            if (stoneReward > 0) resMgr.EarnStone(stoneReward);
            if (foodReward > 0) resMgr.EarnFood(foodReward);
        }

        // Pokaż popup
        if (resUI != null)
        {
            if (goldReward > 0) resUI.ShowGoldPopup(goldReward);
            if (woodReward > 0) resUI.ShowWoodPopup(woodReward);
            if (stoneReward > 0) resUI.ShowStonePopup(stoneReward);
            if (foodReward > 0) resUI.ShowFoodPopup(foodReward);
        }

        Destroy(gameObject);
    }

    public void ReachEnd()
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
            player.TakeDamage(damageToPlayer);

        Destroy(gameObject);
    }
}
