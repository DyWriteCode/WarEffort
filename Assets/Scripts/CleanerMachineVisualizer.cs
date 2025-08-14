using UnityEngine;

namespace FactorySystem.Machines
{
    /// <summary>
    /// 清洁机器可视化组件
    /// </summary>
    public class CleanerMachineVisualizer : MonoBehaviour
    {
        private CleanerMachine controller;
        private Material rangeMaterial;

        public void Initialize(CleanerMachine controller)
        {
            this.controller = controller;
            CreateRangeVisualizer();
        }

        private void CreateRangeVisualizer()
        {
            GameObject rangeVisualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rangeVisualizer.transform.SetParent(transform);
            rangeVisualizer.transform.localPosition = Vector3.zero;
            rangeVisualizer.transform.localScale = Vector3.one * 5f; // 固定大小表示清洁范围

            rangeMaterial = new Material(Shader.Find("Transparent/Diffuse"))
            {
                color = new Color(0, 0.5f, 1, 0.1f) // 蓝色表示清洁
            };

            var renderer = rangeVisualizer.GetComponent<Renderer>();
            renderer.material = rangeMaterial;
            Destroy(rangeVisualizer.GetComponent<Collider>());
        }

        private void OnDestroy()
        {
            if (rangeMaterial != null)
            {
                Destroy(rangeMaterial);
            }
        }
    }
}