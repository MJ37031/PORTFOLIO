using UnityEngine;
using DG.Tweening;

public class FingerMovement : MonoBehaviour
{
    public Transform[] pathPoints;

    void Start()
    {
        Vector3[] path = new Vector3[pathPoints.Length];
        for (int i = 0; i < pathPoints.Length; i++)
        {
            path[i] = pathPoints[i].position;
        }

        transform.DOPath(path, 1.5f, PathType.CatmullRom)
            .SetOptions(false)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
}