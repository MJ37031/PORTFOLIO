using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "Game/CustomerData")]
public class CustomerData : ScriptableObject
{
    public string customerType;

    [Tooltip("빵이 없는 경우 기다릴 수 있는 최대 시간")]
    public float patienceTime = 30f;

    [Tooltip("한 번에 구매하는 빵의 개수")]
    public int buyAmount = 1;

    [Tooltip("이동 속도")]
    public float moveSpeed = 5f;
}