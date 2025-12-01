using UnityEngine;
using UnityEngine.UI;

public class MaxUI : MonoBehaviour
{
    public GameObject maxPanel; 

    public void ShowMax(bool show)
    {
        if (maxPanel != null)
            maxPanel.SetActive(show);
    }
}