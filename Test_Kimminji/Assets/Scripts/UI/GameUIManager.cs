using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager I;

    [Header("UI")]
    public TextMeshProUGUI moneyText;

    private int money = 0;

    private void Awake()
    {
        I = this;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }
}
