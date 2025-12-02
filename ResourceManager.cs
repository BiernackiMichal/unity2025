using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    [Header("Player Resources")]
    public int gold = 100;
    public int wood = 0;
    public int stone = 0;
    public int food = 0;

    // Eventy do aktualizacji UI
    public event Action<int> OnGoldChanged;
    public event Action<int> OnWoodChanged;
    public event Action<int> OnStoneChanged;
    public event Action<int> OnFoodChanged;

    // Dodawanie surowców
    public void EarnGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    public void EarnWood(int amount)
    {
        wood += amount;
        OnWoodChanged?.Invoke(wood);
    }

    public void EarnStone(int amount)
    {
        stone += amount;
        OnStoneChanged?.Invoke(stone);
    }

    public void EarnFood(int amount)
    {
        food += amount;
        OnFoodChanged?.Invoke(food);
    }

    // Sprawdzanie kosztów
    public bool CanAfford(int goldCost, int woodCost, int stoneCost, int foodCost = 0)
    {
        return gold >= goldCost && wood >= woodCost && stone >= stoneCost && food >= foodCost;
    }

    // Wydawanie surowców
    public bool SpendResources(int goldCost, int woodCost, int stoneCost, int foodCost = 0)
    {
        if (!CanAfford(goldCost, woodCost, stoneCost, foodCost)) return false;

        gold -= goldCost;
        wood -= woodCost;
        stone -= stoneCost;
        food -= foodCost;

        OnGoldChanged?.Invoke(gold);
        OnWoodChanged?.Invoke(wood);
        OnStoneChanged?.Invoke(stone);
        OnFoodChanged?.Invoke(food);

        return true;
    }
}
