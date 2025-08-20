using System;
using UnityEngine;

namespace FactorySystem
{
    [Serializable]
    public class ItemInfo
    {
        public string name;
        public string description;
        public GameObject prefab;
        public Item.Type type;

        [Header("Movement Settings")]
        [Tooltip("物品在传送带上的移动速度（槽位/秒）")]
        public float MoveSpeed = 1.0f;

        [Header("Pollution Settings")]
        [Tooltip("物品被破坏时产生的污染系数")]
        public float pollutionFactor = 1.0f;
    }
}