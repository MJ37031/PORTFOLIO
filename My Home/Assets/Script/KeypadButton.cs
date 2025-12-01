using UnityEngine;

public class KeypadButton : MonoBehaviour
{
    public KeypadManager keypad;
    public string buttonValue;

    public void OnPressButton()
    {
        if (buttonValue == "Enter")  // 엔터눌리면 체크코드 불러
        {
            keypad.CheckCode();
        }
        else
        {
            keypad.AddDigit(buttonValue);  // 아니면 추가
        }
    }
}

