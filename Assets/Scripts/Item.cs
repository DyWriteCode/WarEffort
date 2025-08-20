using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

namespace FactorySystem
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
        private Transform _transform;
        private int hp = 100; // 初始血量

        // 增加销毁标记
        private bool isDestroyed = false;

        public int maxHp = 100;
        public GameObject healthBar; // 关联的血条对象

        public ItemInfo Info 
        {
            get 
            {
                return GameApp.ItemManager.GetItemInfo(type);
            }
        }

        public int Hp { 
            get 
            {
                return hp;
            } 
            set 
            {
                hp = value;
            }  
        }

        public Transform transform
        {
            get
            {
                if (isDestroyed) return null;
                return _transform;
            }
            set { _transform = value; }
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

        public void Destroy()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            DestroyHealthBar();
            if (transform != null)
            {
                GameObject.Destroy(transform.gameObject);
                transform = null;
            }
        }
    }

}