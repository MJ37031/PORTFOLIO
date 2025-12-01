using UnityEngine;
using System;

public class EconomyModel : MonoBehaviour
{
    public static EconomyModel I;

    [Header("Economy Data")]
    [SerializeField] private int currentMoney = 0;

    public event Action<int> OnMoneyChanged; 

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public bool TrySpendMoney(int amount)
    {
        if (currentMoney < amount) return false;
        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney);
        return true;
    }

    public int GetMoney()
    {
        return currentMoney;
    }
}

