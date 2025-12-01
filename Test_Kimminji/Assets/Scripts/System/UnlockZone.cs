using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UnlockZone : MonoBehaviour
{
    public int unlockCost = 50;         // 해제 비용
    public GameObject lockCanvas;       // 잠금 표시용 UI(Canvas)
    public GameObject destroyObjs;    // 해제 시 사라질 오브젝트
    public GameObject[] furnitureObjs;  // 해제 시 등장할 가구들
    public GameObject moneyDropPrefab;  // 돈 뱉는 연출 프리팹
    public Transform moneySpawnPoint;   // 돈 뱉는 위치 (구역 앞)
    public ParticleSystem congraturation;
    public Transform unlockzonePosition;


    [Header("UI")]
    public TextMeshProUGUI costText;

    private Collider zoneCollider; // 영역 정보를 가져오기 위한 변수
    private bool isUnlocked = false;

    public bool IsUnlocked => isUnlocked;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
    }

    public void Unlock()
    {
        isUnlocked = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUnlocked) return;
        if (!other.CompareTag("Player")) return;
        ArrowView.I.HideArrow(unlockzonePosition);

        var wallet = other.GetComponent<PlayerWallet>();
        if (wallet != null && wallet.CurrentMoney >= unlockCost)
        {
            wallet.SpendMoney(unlockCost);
            StartCoroutine(UnlockSequence());
        }

        else
        {
            Debug.Log("돈이 부족합니다!");
        }
    }




    private IEnumerator UnlockSequence()
    {

        int currentCostToPay = unlockCost;

        for (int i = 0; i < unlockCost; i++)

        {

            Vector3 spawnPos = moneySpawnPoint.position + Vector3.up * 2f;
            GameObject obj = Instantiate(moneyDropPrefab, spawnPos, Quaternion.identity);

            Destroy(obj, 2.0f);
            Rigidbody rb = obj.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 playerForward = moneySpawnPoint.forward;
                playerForward.y = 0;
                playerForward.Normalize();


                Vector3 upwardForce = Vector3.up * Random.Range(1f, 3f);
                Vector3 randomHorizontalOffset = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
                Vector3 forwardForce = (playerForward + randomHorizontalOffset).normalized * Random.Range(1f, 2f);
                Vector3 finalForce = upwardForce + forwardForce;

                rb.AddForce(finalForce, ForceMode.Impulse);

            }
            currentCostToPay--;

            if (costText != null)
            {
                costText.text = $"{currentCostToPay}";
            }

            yield return new WaitForSeconds(0.05f);
        }
        if (lockCanvas != null) lockCanvas.SetActive(false);

        if (destroyObjs != null) destroyObjs.SetActive(false);



        foreach (var obj in furnitureObjs)

        {
            obj.SetActive(true);
            StartCoroutine(PopAnimation(obj.transform));
        }

        if (congraturation != null)
        {
            congraturation.Play();
        }
        isUnlocked = true;
    }


    private IEnumerator PopAnimation(Transform target)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        target.localScale = startScale;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f); // EaseOut
            target.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        target.localScale = endScale;
    }
}
