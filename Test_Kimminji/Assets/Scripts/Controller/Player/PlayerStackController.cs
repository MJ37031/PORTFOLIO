using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStackController : MonoBehaviour
{
    [Header("Stack Settings")]
    public Transform stackPoint;       // 빵 쌓일 기준점 (머리 위 빈 오브젝트)
    public float stackOffset = 0.3f;   // 빵 하나 높이
    public float moveDuration = 0.25f; // 빵 움직이는 시간
    public float grabInterval = 0.3f;  // 빵 집는 간격
    public int maxStack = 8;           // 최대 들 수 있는 빵 개수
    public AudioSource putbreadsound;  // 빵 내릴 때 효과음
    public AudioSource getbreadsound;  // 빵 얻을 때 효과음

    [Header("References")]
    public Animator animator;   // 플레이어 Animator
    public MaxUI maxUI;         // MAX UI (꽉 찼을 때 표시)

    [Header("ArrowPoint")]
    public Transform saleCouterPoint;

    private List<GameObject> carriedBreads = new List<GameObject>(); // 현재 들고 있는 빵
    private Queue<GameObject> breadQueue = new Queue<GameObject>();  // 애니메이션 대기열
    private BreadBox currentBox;    // 지금 충돌 중인 빵 상자
    private bool isAnimating = false;
    private bool isCollecting = false;

    private void OnTriggerEnter(Collider other)
    {
        // 빵 상자와 충돌 → 집기 시작
        BreadBox box = other.GetComponent<BreadBox>();
        if (box != null)
        {
            currentBox = box;
            if (!isCollecting)
            {
                ArrowView.I.ShowArrow(saleCouterPoint, 40);  // 진열대 화살표 생성
                StartCoroutine(CollectFromBox());
            }
        }

        // 판매대와 충돌 → 전부 내려놓기
        SaleCounterController counter = other.GetComponent<SaleCounterController>();
        if (counter != null)
            StartCoroutine(SellAllBreads(counter));
    }

    private void OnTriggerExit(Collider other)
    {
        BreadBox box = other.GetComponent<BreadBox>();
        if (box != null && box == currentBox)
        {
            currentBox = null;
            isCollecting = false;
        }
    }

    // -------------------------
    // 빵 상자에서 일정 간격으로 집기
    // -------------------------
    private IEnumerator CollectFromBox()
    {
        isCollecting = true;
        
        while (currentBox != null)
        {
            if (carriedBreads.Count + breadQueue.Count >= maxStack)
            {
                if (maxUI != null) maxUI.ShowMax(true);
                yield return null;
                continue;
            }

            if (currentBox.HasBread())
            {
                GameObject bread = currentBox.TakeBread(); // 씬 안에 있는 빵 하나 꺼내오기
                if (bread != null)
                {
                    PrepareBread(bread);

                    if (!isAnimating)
                        StartCoroutine(ProcessBreadQueue());

                    yield return new WaitForSeconds(grabInterval);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    // -------------------------
    // 빵을 스택에 붙일 준비
    // -------------------------
    private void PrepareBread(GameObject bread)
    {
        if (carriedBreads.Count + breadQueue.Count >= maxStack)
        {
            if (maxUI != null) maxUI.ShowMax(true);
            return;
        }

        if (bread.TryGetComponent<Rigidbody>(out var rb)) Destroy(rb);
        if (bread.TryGetComponent<Collider>(out var col)) col.enabled = false;

        bread.transform.SetParent(stackPoint);
        bread.transform.localRotation = Quaternion.Euler(0, 90, 0);
        bread.transform.localPosition = Vector3.zero;

        breadQueue.Enqueue(bread);
    }

    // -------------------------
    // 큐에 있는 빵을 하나씩 애니메이션으로 쌓기
    // -------------------------
    private IEnumerator ProcessBreadQueue()
    {
        isAnimating = true;

        while (breadQueue.Count > 0)
        {
            GameObject bread = breadQueue.Dequeue();
            if (getbreadsound != null)
            {
                getbreadsound.PlayOneShot(getbreadsound.clip);
            }

            Vector3 startPos = bread.transform.localPosition;
            Vector3 targetPos = Vector3.up * stackOffset * carriedBreads.Count;

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                t = 1f - Mathf.Pow(1f - t, 3f); // easeOut
                bread.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            bread.transform.localPosition = targetPos;
            carriedBreads.Add(bread);

            // Animator 갱신
            if (animator != null)
                animator.SetBool("IsStack", true);
        }

        isAnimating = false;
    }

    // -------------------------
    // 판매대에 빵 전부 내려놓기
    // -------------------------
    private IEnumerator SellAllBreads(SaleCounterController counter)
    {
        // 맨 위에 있는 빵부터 내려놓기 위해 리스트의 마지막 요소부터 처리
        int index = carriedBreads.Count - 1;
        ArrowView.I.HideArrow(saleCouterPoint);  // 진열대 화살표 제거
        while (index >= 0)
        {
            GameObject bread = carriedBreads[index];
            carriedBreads.RemoveAt(index);

            // 판매대 View에 빵을 추가
            counter.AddBread(bread, index);

            if (putbreadsound != null)
            {
                putbreadsound.PlayOneShot(putbreadsound.clip);
            }

            yield return new WaitForSeconds(0.2f);
            index--;
        }

        if (maxUI != null) maxUI.ShowMax(false);
        if (animator != null)
            animator.SetBool("IsStack", false);
    }
}