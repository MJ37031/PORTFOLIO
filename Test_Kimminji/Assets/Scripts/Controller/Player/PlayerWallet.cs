using UnityEngine;
using TMPro;

public class PlayerWallet : MonoBehaviour
{
    public int CurrentMoney { get; private set; } = 0;
    public TextMeshProUGUI moneyText;

    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (CurrentMoney < amount) return false;

        CurrentMoney -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = CurrentMoney.ToString();
    }
}