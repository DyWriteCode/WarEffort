using System.Collections.Generic;
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
            // 添加控制器组件
            var controller = gameObject.AddComponent<AttackMachineController>();
            controller.damageRange = info.damageRange;
            controller.damageInterval = info.damageInterval;
            controller.damagePerTick = info.damagePerTick;
            controller.Initialize(this);
        }

        public override void Run()
        {
            base.Run(); // 调用基类方法（如果需要）

            currentTick++;
            if (currentTick < damageInterval) return;
            currentTick = 0;

            ApplyDamageToItemsInRange();
        }

        /// <summary>
        /// 对范围内的所有物品应用伤害
        /// </summary>
        private void ApplyDamageToItemsInRange()
        {
            // 遍历所有物品
            foreach (var itemEntry in GameGrid.Instance.items)
            {
                Item item = itemEntry.Value;
                // 计算物品与建筑的距离
                float distance = Vector3.Distance(position, item.position);

                if (distance <= damageRange)
                {
                    // 应用伤害
                    item.Hp -= damagePerTick;

                    if (item.Hp <= 0)
                    {
                        // 如果物品血量<=0，则销毁
                        GameGrid.Instance.SafeDestroyItem(item.id);
                    }
                }
            }
        }
    }
}