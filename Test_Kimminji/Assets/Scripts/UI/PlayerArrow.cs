using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public ArrowView arrowView;

    private Transform arrowTransform;

    void Start()
    {

        arrowTransform = transform.Find("PlayerArrow");
        if (arrowTransform == null)
        {
            enabled = false;
            return;
        }

        if (arrowView == null)
        {
            arrowView = ArrowView.I;
        }

        if (arrowView == null)
        {
            Debug.LogError("ArrowView.I 인스턴스를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (arrowView == null || arrowTransform == null) return;

        Transform targetTransform = arrowView.GetCurrentTargetTransform();

        if (targetTransform == null)
        {
            arrowTransform.gameObject.SetActive(false);
            return;
        }
        arrowTransform.gameObject.SetActive(true);

        Vector3 playerPos = transform.position;
        Vector3 targetPos = targetTransform.position;

        Vector3 directionToTarget = new Vector3(targetPos.x, playerPos.y, targetPos.z) - playerPos;

        arrowTransform.localPosition = new Vector3(0, 0.1f, 1f);

        if (directionToTarget != Vector3.zero)
        {
            Quaternion worldRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            arrowTransform.localRotation = Quaternion.Inverse(transform.rotation) * worldRotation;
        }
    }
}