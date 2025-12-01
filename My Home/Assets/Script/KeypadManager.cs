using System;
using TMPro;
using UnityEngine;

public class KeypadManager : MonoBehaviour
{
    public TMP_Text passwordScreen;
    public string correctCode = "12";
    public Animator doorAnimator1;
    public Animator doorAnimator2;
    public float openTime = 3.0f;  // 문이 열려있는 시간

    string currentInput = "";  // 내가 누르는 숫자 (correct 와 비교)
    bool isOpen = false;  // 시작할 때 문 잠겨있음

    public void RefreshScreen() // 화면을 현재 인풋으로 보여줘라 (업데이트 UI)
    {
        passwordScreen.text = currentInput;
    }

    public void AddDigit(string digit)
    {
        if (currentInput.Length < correctCode.Length)  // 인풋이 비번보다 작을 때(길이가)만 숫자를 추가해서 UI표시
        {
            currentInput += digit;
            RefreshScreen();
        }
    }

    public void CheckCode()
    {
        if (currentInput == correctCode)
        {
            Debug.Log("Success");
            OpenDoor();
            ClearInput();
        }
        else
        {
            Debug.Log("Fail");
            ClearInput();
        }
    }

    private void OpenDoor()
    {
        if (!isOpen)
        {
            doorAnimator1.SetBool("IsOpen", true);  //여는 애니메이션 다음
            doorAnimator2.SetBool("IsOpen", true);  //여는 애니메이션 다음
            isOpen = true;
            Invoke("CloseDoor", openTime);  // 3초뒤에 CloseDoor 불러와라
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            doorAnimator1.SetBool("IsOpen", false);
            doorAnimator2.SetBool("IsOpen", false);
            isOpen = false;
        }
    }

    private void ClearInput()
    {
        currentInput = "";
        RefreshScreen();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
