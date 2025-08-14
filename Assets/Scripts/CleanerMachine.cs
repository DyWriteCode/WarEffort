using UnityEngine;

namespace FactorySystem.Machines
{
    /// <summary>
    /// 清洁机器，定期减少全局污染度
    /// </summary>
    public class CleanerMachine : Machine
    {
        // 清洁间隔（单位：游戏刻）
        public int cleanInterval = 20;
        // 每次清洁减少的污染值
        public float pollutionReduction = 10f;

        private int currentTick = 0;

        public CleanerMachine(GameObject gameObject) : base(gameObject, Type.CleanerMachine, gameObject.transform.position)
        {
            // 添加可视化组件
            var visualizer = gameObject.AddComponent<CleanerMachineVisualizer>();
            visualizer.Initialize(this);
        }

        public override void Run()
        {
            base.Run();

            currentTick++;
            if (currentTick < cleanInterval) return;
            currentTick = 0;

            CleanPollution();
        }

        /// <summary>
        /// 执行污染清洁
        /// </summary>
        private void CleanPollution()
        {
            GameApp.PollutionManager.ReducePollution(pollutionReduction);

            // 添加清洁特效
            if (gameObject != null)
            {
                // 实际项目中可以添加粒子系统
                Debug.Log($"{gameObject.name} 清洁污染，减少 {pollutionReduction} 点污染");
            }
        }
    }
}