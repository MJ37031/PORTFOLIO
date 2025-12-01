// DocumentGoal.cs
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DocumentGoal : MonoBehaviour
{
    [Header("Goal")]
    public int total = 5;
    public int collected = 0;

    [Header("UI (optional)")]
    public TextMeshProUGUI counterText; // "Docs 0/5" 같은 표시
    public HintPopupHUD hintHUD;     // 위 스크립트
    public AudioSource audio;
    public AudioClip picksound;

    [Header("Reward (optional)")]
    public GameObject keyObject;        // 열쇠 오브젝트(처음엔 비활성 권장)
    public bool deactivateKeyAtStart = true;


    public UnityEvent onProgressChanged;


    void Start() => UpdateUI();

    public void AddOne()
    {
        if (collected >= total) return;
        collected = Mathf.Clamp(collected + 1, 0, total);
        onProgressChanged?.Invoke();
        UpdateUI();
       

        if (collected >= total)
        {
            if (keyObject) keyObject.SetActive(true); // 다 모이면 열쇠 활성화
            if (hintHUD) hintHUD.Show();

        }
    }

    void UpdateUI()
    {
        if (counterText) counterText.text = $"Docs {collected}/{total}";
        audio.PlayOneShot(picksound);
    }
}