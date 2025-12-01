using UnityEngine;
using System.Collections.Generic;

public class PlayerStackView : MonoBehaviour
{
    [Header("View Settings")]
    public Transform stackPoint;
    public float stackOffset = 0.3f;

    private List<GameObject> visuals = new List<GameObject>();

    public void Bind(StackManager manager)
    {
        manager.OnItemAddedRequest += (factory, index) =>
        {
            GameObject prefab = factory();
            GameObject visual = Instantiate(prefab, stackPoint, false);
            visual.transform.localPosition = Vector3.up * stackOffset * index;
            visual.transform.localRotation = Quaternion.identity;

            visuals.Add(visual);
            manager.RegisterVisual(visual); 

        };

        manager.OnItemRemoved += (index) =>
        {
            if (index < visuals.Count)
            {
                GameObject last = visuals[index];
                visuals.RemoveAt(index);
                Destroy(last);
            }
        };
    }
}
