// MoneyStackerManager.cs

using UnityEngine;

public class MoneyStackerManager : MonoBehaviour
{
    public static MoneyStackerManager I;

    [Header("ÁöÁ¡º° MoneyStacker")]
    public MoneyStacker takeOutStacker; // Æ÷Àå ¼Õ´Ô¿ë µ· ½×´Â °÷
    public MoneyStacker dineInStacker;  // ¸ÅÀå ½Ä»ç ¼Õ´Ô¿ë µ· ½×´Â °÷

    private void Awake()
    {
        I = this;
    }
    public void AddMoney(CustomerType customerType, int breadCount)
    {
        if (customerType == CustomerType.TakeOut && takeOutStacker != null)
        {
            takeOutStacker.AddMoneyForBread(breadCount);
        }
        else if (customerType == CustomerType.EatingIn && dineInStacker != null)
        {
            dineInStacker.AddMoneyForBread(breadCount);
        }
    }
}