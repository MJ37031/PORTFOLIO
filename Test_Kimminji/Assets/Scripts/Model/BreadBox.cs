using System.Collections.Generic;
using UnityEngine;

public class BreadBox : MonoBehaviour
{
    public ProductionController oven; // 인스펙터에서 오븐 연결
    public Transform ovenposition;

    private List<GameObject> breads = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bread"))
        {
            ArrowView.I.ShowArrow(ovenposition, 20); 
            if (!breads.Contains(other.gameObject))
                breads.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 빵이 상자 밖으로 나가면 제거
        if (other.CompareTag("Bread"))
        {
            breads.Remove(other.gameObject);
        }
    }

    // 플레이어가 빵을 집을 때 호출
    public GameObject TakeBread()
    {
        if (breads.Count > 0 && oven != null)
        {
            GameObject bread = oven.TakeBread();
            ArrowView.I.HideArrow(ovenposition);
            if (bread != null)
            {
                breads.Remove(bread);
                return bread;
            }
        }
        return null;
    }

    public bool HasBread()
    {
        return breads.Count > 0;
    }
}
