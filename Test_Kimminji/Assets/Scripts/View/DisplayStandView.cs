using System.Collections.Generic;
using UnityEngine;

public class DisplayStandView : MonoBehaviour
{
    public Transform gridOrigin; 
    public int rows = 2;
    public int cols = 4;
    public float spacingX = 0.5f;
    public float spacingZ = 0.5f;
    public TMPro.TextMeshProUGUI stockText; // 재고 UI 텍스트


    private List<GameObject> breadModelsOnStand = new List<GameObject>();


    public void UpdateStockVisual(Dictionary<BreadData, int> stock)
    {
        foreach (var bread in breadModelsOnStand)
        {
            Destroy(bread);
        }
        breadModelsOnStand.Clear();

        int currentIndex = 0;
        foreach (var item in stock)
        {
            for (int i = 0; i < item.Value; i++)
            {
                if (currentIndex >= rows * cols) break; 

                GameObject breadInstance = Instantiate(item.Key.prefab);
                PlaceBread(breadInstance, currentIndex);
                breadModelsOnStand.Add(breadInstance);

                currentIndex++;
            }
        }
    }

    public void PlaceBread(GameObject bread, int index)
    {
        int row = index / cols;
        int col = index % cols;

        Vector3 offset = new Vector3(col * spacingX, 0, row * spacingZ);
        bread.transform.SetParent(gridOrigin);
        bread.transform.localPosition = offset;
        bread.transform.localRotation = Quaternion.identity;
    }

    public void UpdateStockUI(int currentCount, int maxCapacity)
    {
        if (stockText != null)
        {
            stockText.text = $"{currentCount}/{maxCapacity}";
        }
    }
}