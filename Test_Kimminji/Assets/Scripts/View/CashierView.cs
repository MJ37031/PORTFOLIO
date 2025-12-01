using UnityEngine;
using TMPro;

public class CashierView : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI moneyText;   
    public GameObject envelopeUI;      
    public void UpdateMoney(int amount)
    {
        if (moneyText != null)
            moneyText.text = $"?{amount}";
    }

    public void ShowEnvelopeUI(bool show)
    {
        if (envelopeUI != null)
            envelopeUI.SetActive(show);
    }
}