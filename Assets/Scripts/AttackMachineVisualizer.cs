using UnityEngine;

namespace FactorySystem.Machines
{
    public class AttackMachineVisualizer : MonoBehaviour
    {
        private AttackMachineController controller;
        private Material rangeMaterial;

        public void Initialize(AttackMachineController controller)
        {
            this.controller = controller;
            CreateRangeVisualizer();
        }

        private void CreateRangeVisualizer()
        {
            GameObject rangeVisualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rangeVisualizer.transform.SetParent(transform);
            rangeVisualizer.transform.localPosition = Vector3.zero;
            rangeVisualizer.transform.localScale = Vector3.one * controller.damageRange * 2;

            rangeMaterial = new Material(Shader.Find("Transparent/Diffuse"))
            {
                color = new Color(1, 0, 0, 0.2f)
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