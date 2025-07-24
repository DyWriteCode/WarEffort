#region v1 code
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Peque.Machines
//{
//    /// <summary>
//    /// 伤害区域机器，定期对范围内的物品造成伤害
//    /// </summary>
//    public class AttackMachine : Machine
//    {
//        // 影响范围半径
//        public float damageRange = 20f;
//        // 伤害间隔（单位：游戏刻）
//        public int damageInterval = 10;
//        // 每次伤害值
//        public int damagePerTick = 10;

//        private int currentTick = 0;


//        public AttackMachine(GameObject gameObject) : base(gameObject, Type.AttackMachine, gameObject.transform.position)
//        {
//            // 添加控制器组件
//            var controller = gameObject.AddComponent<AttackMachineController>();
//            controller.damageRange = info.damageRange;
//            controller.damageInterval = info.damageInterval;
//            controller.damagePerTick = info.damagePerTick;
//            controller.Initialize(this);
//        }

//        public override void Run()
//        {
//            base.Run(); // 调用基类方法（如果需要）

//            currentTick++;
//            if (currentTick < damageInterval) return;
//            currentTick = 0;

//            ApplyDamageToItemsInRange();
//        }

//        /// <summary>
//        /// 对范围内的所有物品应用伤害
//        /// </summary>
//        private void ApplyDamageToItemsInRange()
//        {
//            List<System.Guid> itemsToDestroy = new List<System.Guid>();

//            // 遍历所有物品
//            foreach (var itemEntry in GameGrid.Instance.items)
//            {
//                Item item = itemEntry.Value;
//                // 计算物品与建筑的距离
//                float distance = Vector3.Distance(position, item.position);

//                if (item != null )
//                {
//                    if (distance <= damageRange)
//                    {
//                        // 应用伤害
//                        item.Hp -= damagePerTick;

//                        if (item.Hp <= 0)
//                        {
//                            itemsToDestroy.Add(item.id);
//                        }
//                        else
//                        {
//                            HealthBar HpComponent;
//                            if(item.healthBar != null)
//                            {
//                                item.healthBar.TryGetComponent<HealthBar>(out HpComponent);
//                                if (HpComponent != null)
//                                {
//                                    HpComponent.SetHealth(item.Hp);
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            foreach (var itemId in itemsToDestroy)
//            {
//                GameGrid.Instance.SafeDestroyItem(itemId);
//            }

//            foreach (var machineEntry in GameGrid.Instance.machines.Values.ToList())
//            {
//                if (machineEntry is Belt belt)
//                {
//                    foreach (var itemId in itemsToDestroy)
//                    {
//                        belt.RemoveItemFromBelt(itemId);
//                    }
//                }
//            }
//        }
//    }
//}
#endregion

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Peque.Machines
{
    /// <summary>
    /// 伤害区域机器，定期对范围内的物品造成伤害
    /// </summary>
    public class AttackMachine : Machine
    {
        // 影响范围半径
        public float damageRange = 20f;
        // 伤害间隔（单位：游戏刻）
        public int damageInterval = 10;
        // 每次伤害值
        public int damagePerTick = 10;

        private int currentTick = 0;

        public AttackMachine(GameObject gameObject) : base(gameObject, Type.AttackMachine, gameObject.transform.position)
        {
            // 添加控制器组件并初始化
            var controller = gameObject.AddComponent<AttackMachineController>();
            controller.damageRange = info.damageRange;
            controller.damageInterval = info.damageInterval;
            controller.damagePerTick = info.damagePerTick;
            controller.Initialize(this);
        }

        public override void Run()
        {
            base.Run(); // 调用基类方法

            currentTick++;
            if (currentTick < damageInterval) return;
            currentTick = 0; // 重置计数器

            ApplyDamageToItemsInRange();
        }

        /// <summary>
        /// 对范围内的所有物品应用伤害
        /// </summary>
        private void ApplyDamageToItemsInRange()
        {
            List<System.Guid> itemsToDestroy = new List<System.Guid>();
            Vector3 attackPosition = position; // 缓存位置减少属性访问

            // 遍历所有物品
            foreach (var itemEntry in GameGrid.Instance.items.ToList()) // 使用ToList避免修改集合异常
            {
                Item item = itemEntry.Value;
                if (item == null || item.transform == null) continue;

                // 计算物品与建筑的距离
                float distance = Vector3.Distance(attackPosition, item.position);

                if (distance <= damageRange)
                {
                    // 应用伤害
                    item.Hp -= damagePerTick;

                    if (item.Hp <= 0)
                    {
                        itemsToDestroy.Add(item.id);
                    }
                }
            }

            // 销毁所有血量耗尽的物品
            foreach (var itemId in itemsToDestroy)
            {
                GameGrid.Instance.SafeDestroyItem(itemId);
            }
        }
    }
}