using System.Collections;
using UnityEngine;
public class PlayerEnter : MonoBehaviour
{


    [Header("UI Scale Animation")]
    public RectTransform targetUIRect; // 크기를 변경할 UI 이미지의 RectTransform
    public Vector3 scaleOnEnter = new Vector3(1.2f, 1.2f, 1.2f); // 확대할 크기
    public Vector3 scaleDefault = new Vector3(1f, 1f, 1f);       // 기본 크기
    public float scaleDuration = 0.2f;                           // 애니메이션 시간

    private Coroutine scaleCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    // 플레이어가 영역에 진입했을 때
    private void OnTriggerEnter(Collider other)
    {
        // 'Player' 태그를 가진 오브젝트인지 확인
        if (other.CompareTag("Player") && targetUIRect != null)
        {
            // 현재 실행 중인 코루틴이 있다면 멈춥니다.
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            // 확대 애니메이션 시작
            scaleCoroutine = StartCoroutine(ScaleToTarget(scaleOnEnter));
        }
    }

    // 플레이어가 영역에서 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        // 'Player' 태그를 가진 오브젝트인지 확인
        if (other.CompareTag("Player") && targetUIRect != null)
        {
            // 현재 실행 중인 코루틴이 있다면 멈춥니다.
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            // 축소 애니메이션 시작 (기본 크기로 돌아감)
            scaleCoroutine = StartCoroutine(ScaleToTarget(scaleDefault));
        }
    }

    // 확대/축소 애니메이션을 처리하는 코루틴 함수
    private IEnumerator ScaleToTarget(Vector3 targetScale)
    {
        Vector3 startScale = targetUIRect.localScale;
        float timer = 0f;

        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;
            float t = timer / scaleDuration;

            // Lerp를 사용하여 시작 크기에서 목표 크기로 부드럽게 보간합니다.
            targetUIRect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        // 애니메이션이 끝난 후, 정확히 목표 크기로 설정
        targetUIRect.localScale = targetScale;
        scaleCoroutine = null; // 코루틴이 종료되었음을 표시
    }
}