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

        [Header("Pollution Settings")]
        [Tooltip("物品被破坏时产生的污染系数")]
        public float pollutionFactor = 1.0f;
    }
}