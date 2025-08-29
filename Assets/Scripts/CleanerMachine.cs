using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace FactorySystem.Machines
{
    /// <summary>
    /// 清洁机器，定期减少全局污染度
    /// </summary>
    public class CleanerMachine : Machine
    { 
        public CleanerMachine(GameObject gameObject) : base(gameObject, Type.CleanerMachine, gameObject.transform.position)
        {
            // 添加可视化组件
            var visualizer = gameObject.AddComponent<CleanerMachineVisualizer>();
            visualizer.Initialize(this);
            // 注册清洁任务
            RegisterCleaningTask();
        }

        private void RegisterCleaningTask()
        {
            // 使用TimerManager注册清洁任务
            string taskId = $"{position}_Cleaning";
            List<Action> callbacks = new List<Action>();
            callbacks.Add(CleanPollution);
            GameApp.TimerManager.RegisterTask(
                taskId: taskId,
                callback: callbacks,
                interval: Info.cleanInterval,
                unit: TimeUnit.Ticks,
                isLoop: true,
                owner: this
            );
        }

        public override void Run()
        {
            base.Run();
        }

        /// <summary>
        /// 执行污染清洁
        /// </summary>
        private void CleanPollution()
        {
            GameApp.PollutionManager.ReducePollution(Info.pollutionReduction);

            // 添加清洁特效
            if (gameObject != null)
            {
                // 实际项目中可以添加粒子系统
                // Debug.Log($"{gameObject.name} 清洁污染，减少 {Info.pollutionReduction} 点污染");
            }
        }
    }
}