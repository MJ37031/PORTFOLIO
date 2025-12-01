using UnityEngine;

public class FloatingArrow : MonoBehaviour
{
    public float floatAmplitude = 0.2f; // 위아래 범위
    public float floatFrequency = 2f;   // 속도

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
