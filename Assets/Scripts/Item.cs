using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Peque
{
    public class Item
    {
        public enum Type
        {
            Coal = 1,
            IronOre = 2,
            Iron = 3,
        }

        public System.Guid id;
        public Vector3 parent;
        public Vector3 position;
        public Type type;
        public Transform transform;
        private int hp = 100; // 初始血量
        
        public int maxHp = 100;
        public GameObject healthBar; // 关联的血条对象

        public ItemInfo info {
            get {
                return GameGrid.Instance.GetItemInfo(type);
            }
        }

        public int Hp { get => hp; set {
                hp = value;
                if (healthBar != null)
                {
                    Debug.Log("healthBar.GetComponent<HealthBar>().SetHealth(hp);");
                    healthBar.GetComponent<HealthBar>().SetHealth(hp);
                }
            }  
        }

        public void DestroyHealthBar()
        {
            if (healthBar != null)
            {
                // 安全销毁血条
                GameObject.Destroy(healthBar);
                healthBar = null;
            }
        }

        public void ShowHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.SetActive(true);
            }
        }

        public void HideHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.SetActive(false);
            }
        }
    }

}