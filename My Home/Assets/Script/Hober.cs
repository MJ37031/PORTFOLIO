using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class Hober : MonoBehaviour
{
    public Color normalColor = Color.cyan; // 기본 아웃라인 색
    public Color hoverColor = Color.red; // Hover시 색

    private QOutline outline; // Quick Outline 컴포넌트
    void Awake()
    {
        outline = GetComponent<QOutline>();
        if (outline != null)
            outline.OutlineColor = normalColor;
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (outline != null)
            outline.OutlineColor = hoverColor;
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        if (outline != null)
            outline.OutlineColor = normalColor;
    }
}
