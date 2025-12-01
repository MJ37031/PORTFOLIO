using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBread", menuName = "Game/BreadData")]
public class BreadData : ScriptableObject
{
    public string breadName;        // 빵 이름
    public float productionTime;    // 생산 소요 시간 (초)
    public int salePrice;           // 판매 가격
    public GameObject prefab;       // 빵 프리팹 (완성품)
    public Sprite icon;
}
