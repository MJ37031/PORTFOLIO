using System.Collections;
using TMPro;
using UnityEngine;

public class CustomerView : MonoBehaviour
{
    [Header("Bubble UI")]
    public GameObject itemBubble;

    [Header("Icons")]
    public GameObject breadIconObj;     // 빵 아이콘 오브젝트
    public GameObject cashierIconObj;   // 계산기 아이콘 오브젝트
    public TextMeshProUGUI amountText;  // 구매 개수 텍스트

    [Header("Visuals")]
    public Animator animator;
    public ParticleSystem happyEffect;
    public ParticleSystem trashEffect;

    private int remainingAmount;

    public void SetAnimation(CustomerState state, bool hasStack)
    {
        if (!animator) return;

        bool walking = (state == CustomerState.WalkingToStand ||
                        state == CustomerState.WalkingToCashier ||
                        state == CustomerState.Leaving);

        animator.SetBool("isWalking", walking);
        animator.SetBool("hasStack", hasStack);
    }

    public void ShowDesiredItem(BreadData breadData, int amount)
    {
        if (!itemBubble) return;

        itemBubble.SetActive(true);
        remainingAmount = amount;

        breadIconObj.SetActive(true);
        cashierIconObj.SetActive(false);

        UpdateAmountText();
    }

    public void TakeOneItem()
    {
        if (remainingAmount > 0)
            remainingAmount--;

        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        if (!amountText) return;

        if (remainingAmount > 0)
        {
            amountText.text = remainingAmount.ToString();
        }
        else
        {
            amountText.text = "";
            breadIconObj.SetActive(false);
            cashierIconObj.SetActive(true);
        }
    }

    public void HideDesiredItem()
    {
        if (itemBubble) itemBubble.SetActive(false);
    }

    public void ShowHappyEffect(float duration = 1.5f)
    {
        if (happyEffect != null)
        {
            happyEffect.Play();
           
        }
    }

    public void ShowTrashEffect(float duration = 1.5f)
    {
        if (trashEffect != null)
        {
            trashEffect.Play();

        }
    }


}
