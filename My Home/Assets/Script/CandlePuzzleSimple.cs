using UnityEngine;

public class CandlePuzzleSimple : MonoBehaviour
{
    public CandleSlotSimple[] slots; // 소켓 4개 드래그
    public GameObject hintPaper;     // 시작 시 비활성화

    public bool revealOnce = true;
    bool _revealed;

    void Start()
    {
        foreach (var s in slots)
            if (s) s.onStateChanged.AddListener(CheckAll);

        CheckAll();
    }

    public void CheckAll()
    {
        if (_revealed && revealOnce) return;

        foreach (var s in slots)
        {
            if (!s || !s.IsCorrect)
            {
                if (!revealOnce && hintPaper) hintPaper.SetActive(false);
                return;
            }
        }

        if (hintPaper) hintPaper.SetActive(true);
        _revealed = true;
    }
}
