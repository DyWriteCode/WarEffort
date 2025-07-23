using UnityEngine;

namespace Peque.Machines
{
    public class AttackMachineVisualizer : MonoBehaviour
    {
        private AttackMachineController controller;

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

            var renderer = rangeVisualizer.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Transparent/Diffuse"))
            {
                color = new Color(1, 0, 0, 0.2f)
            };
            Destroy(rangeVisualizer.GetComponent<Collider>());
        }
    }
}