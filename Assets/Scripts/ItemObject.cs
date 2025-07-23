using UnityEngine;

namespace Peque
{
    public class ItemObject : MonoBehaviour
    {
        public Item item;

        void Start()
        {
            // 确保物品有碰撞体
            if (GetComponent<Collider>() == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }
    }
}