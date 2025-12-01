using UnityEngine;

[DisallowMultipleComponent]
public class LightEffect : MonoBehaviour
{
    [Header("Target Light")]
    public UnityEngine.Light target; // ¡ç ¿©±â!

    [Header("Brightness")]
    public float baseIntensity = 0f;
    public float minMultiplier = 0.5f;
    public float maxMultiplier = 1.2f;

    [Header("Step Timing (sec)")]
    public Vector2 stepInterval = new Vector2(0.05f, 0.25f);

    float _timer;
    float _holdTime;
    float _currentMul = 1f;

    void Awake()
    {
        if (!target) target = GetComponent<UnityEngine.Light>(); 
        if (target)
        {
            if (baseIntensity <= 0f) baseIntensity = target.intensity;
            _currentMul = Random.Range(minMultiplier, maxMultiplier);
            PickNextStepTime();
            Apply();
        }
    }

    void Update()
    {
        if (!target) return;

        _timer += Time.deltaTime;
        if (_timer >= _holdTime)
        {
            _timer = 0f;
            _currentMul = Random.Range(minMultiplier, maxMultiplier);
            PickNextStepTime();
            Apply();
        }
    }

    void PickNextStepTime()
    {
        float min = Mathf.Max(0.01f, Mathf.Min(stepInterval.x, stepInterval.y));
        float max = Mathf.Max(stepInterval.x, stepInterval.y);
        _holdTime = Random.Range(min, max);
    }

    void Apply()
    {
        target.intensity = baseIntensity * _currentMul; 
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (minMultiplier > maxMultiplier)
        {
            float t = minMultiplier;
            minMultiplier = maxMultiplier;
            maxMultiplier = t;
        }
    }
#endif
}
