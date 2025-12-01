using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintPopupHUD : MonoBehaviour
{
    [Header("Panel Root")]
    public GameObject panelRoot;              // 패널 오브젝트(비활성로 시작 권장)


    [Header("Timing (sec)")]
    public float fadeIn = 0.15f;
    public float hold = 1.6f;
    public float fadeOut = 0.35f;

    [Header("Raycast")]
    public bool disableRaycastTargets = true; // 클릭/터치 막지 않도록 기본 off

    UnityEngine.UI.Graphic[] _graphics;
    Coroutine _co;

    void Awake()
    {
        if (!panelRoot) panelRoot = gameObject;
        _graphics = panelRoot.GetComponentsInChildren<Graphic>(includeInactive: true);

        if (disableRaycastTargets)
            foreach (var g in _graphics) g.raycastTarget = false;

        // 처음엔 투명 & 비활성 권장
        SetAlphaImmediate(0f);
        panelRoot.SetActive(false);
    }

    public void Show(Sprite sprite = null, string message = null, float? holdOverride = null)
    {
        if (holdOverride.HasValue) hold = holdOverride.Value;

        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        panelRoot.SetActive(true);

        // 인: 일단 0으로 스냅 후 페이드
        SetAlphaImmediate(0f);
        CrossFade(1f, fadeIn);
        yield return new WaitForSecondsRealtime(fadeIn + hold);

        // 아웃
        CrossFade(0f, fadeOut);
        yield return new WaitForSecondsRealtime(fadeOut);

        panelRoot.SetActive(false);
        _co = null;
    }

    void CrossFade(float target, float duration)
    {
        foreach (var g in _graphics)
            g.CrossFadeAlpha(target, duration, ignoreTimeScale: true);
    }

    void SetAlphaImmediate(float a)
    {
        foreach (var g in _graphics)
            g.canvasRenderer.SetAlpha(a);
    }
}