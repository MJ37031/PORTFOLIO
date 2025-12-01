using UnityEngine;
using System.Collections;

public static class AnimationHelper
{
    // 'static' 클래스로 만들어 어디서든 쉽게 접근 가능하게 합니다.

    public static IEnumerator SmoothRise_Coroutine(Transform obj, Vector3 targetPos, float duration = 0.4f)
    {
        float elapsed = 0f;
        Vector3 startPos = obj.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f); // EaseOut 효과
            obj.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        obj.position = targetPos;
    }
}