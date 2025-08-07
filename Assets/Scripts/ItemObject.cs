using UnityEngine;

namespace FactorySystem
{
    public class ItemObject : MonoBehaviour
    {
        [System.NonSerialized]
        public Item item;

        void Start()
        {
            // 确保物品有碰撞体
            if (GetComponent<Collider>() == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }

        void OnDestroy()
        {
            // 确保销毁时清除引用
            if (item != null)
            {
                item.transform = null;
                item = null;
            }
        }
    }
}