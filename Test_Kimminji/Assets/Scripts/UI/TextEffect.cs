using UnityEngine;
using DG.Tweening; // DOTween 네임스페이스 추가

public class TextEffect : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(1.5f, 0.4f).SetLoops(-1, LoopType.Yoyo);
    }
}