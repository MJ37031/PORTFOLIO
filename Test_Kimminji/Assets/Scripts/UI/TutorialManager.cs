using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialUI; 

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            tutorialUI.SetActive(false);
            this.enabled = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            tutorialUI.SetActive(false);
            this.enabled = false;
        }
    }
}