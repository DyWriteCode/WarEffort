using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;

namespace Peque { 
    public class GameGrid : MonoBehaviour
    {
        public enum Position
        {
            Top = 1,
            Left = 2,
            Bottom = 3,
            Right = 4,
        }

        public static GameGrid Instance;
        public Dictionary<Vector3, Machine> machines = new Dictionary<Vector3, Machine>();
        public Dictionary<Vector3, Vector3> grid = new Dictionary<Vector3, Vector3>();
        public Dictionary<System.Guid, Item> items = new Dictionary<System.Guid, Item>();
        public Queue<System.Guid> itemsToCreate = new Queue<System.Guid>();
        public Queue<System.Guid> itemsToMove = new Queue<System.Guid>();

        public MachineInfo[] machinesInfo;
        public ItemInfo[] itemsInfo;

        public Material beltMaterial;
        public GameObject floorTile;

        public int money = 10000;

        public GameObject healthBarPrefab;

        private float moveTextureSpeed = 1;
        private int ticksPerSecond = 6;
        float moveX;

        [SerializeField]
        private int size = 1;

        private void Awake() {
            Instance = this;

            processMachineInfo();
        }

        private void Start() {
            InvokeRepeating("moveItemsInBelts", 0, 1.0f / ticksPerSecond);
            InvokeRepeating("executeMachines", 0, 1.0f / ticksPerSecond);
        }

        void Update() {
            animateConveyorBelt();
            moveItemsInBelts();
            createItems();
            moveItems();
        }

        /*
         * Reads pending items queue and creates them
         */
        private void createItems () {
            while (itemsToCreate.Any()) {
                System.Guid id = itemsToCreate.Dequeue();
                Item item = items[id];
                GameObject obj = Instantiate(item.info.prefab, item.position, Quaternion.Euler(-90, 0, 0), getMachineAt(item.parent).gameObject.transform);
                item.transform = obj.transform;

                // 创建血条
                if (healthBarPrefab)
                {
                    GameObject healthBar = Instantiate(healthBarPrefab, obj.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
                    healthBar.GetComponent<HealthBar>().target = obj.transform;
                    item.healthBar = healthBar;
                }
            }
        }

        private void moveItems() {
            while (itemsToMove.Any()) {
                System.Guid id = itemsToMove.Dequeue();
                Item item = items[id];
                item.transform.parent = getMachineAt(item.parent).gameObject.transform;
                item.transform.localPosition = item.position;
            }
        }

        public void moveItem (System.Guid id, Vector3 position) {
            items[id].position = position;
            itemsToMove.Enqueue(id);
        }

        private void processMachineInfo () {
            // make some data easier to access
            foreach (MachineInfo machine in machinesInfo) {
                foreach (ItemUI item in machine._storageLimits) {
                    machine.storageLimits.Add(item.type, item.quantity);
                }
                machine._storageLimits = null;
            }
        }

        private void animateConveyorBelt () {
            // animate conveyor belt texture
            moveX = (moveTextureSpeed * Time.deltaTime) + moveX;
            if (moveX > 1) moveX = 0;
            beltMaterial.mainTextureOffset = new Vector2(moveX, 0);
        }
        private void moveItemsInBelts () {
            foreach (Machine machine in machines.Values) {
                if (machine.type != Machine.Type.Belt) {
                    continue;
                }
            }
        }

        private void executeMachines () {
            /*
            var job = new MachineJob();
            JobHandle handle = job.Schedule();
            handle.Complete();
            */
            foreach (Machine machine in machines.Values)
            {
                machine.run();
            }
            /*
            foreach (Machine machine in machines.Values) {
               machine.run();
            }*/
        }

        public Vector3 GetNearestPointOnGrid(Vector3 position)
        {
            position.y = 0.3f;
            position -= transform.position;

            int xCount = Mathf.RoundToInt(position.x / size);
            int yCount = Mathf.RoundToInt(position.y / size);
            int zCount = Mathf.RoundToInt(position.z / size);

            Vector3 result = new Vector3(
                (float)xCount * size,
                (float)yCount * size,
                (float)zCount * size);

            result += transform.position;

            return result;
        }

        public Vector3[] getNeighbors(Vector3 position) {
            return new Vector3[] {
                new Vector3(position.x - 1, position.y, position.z),
                new Vector3(position.x + 1, position.y, position.z),
                new Vector3(position.x, position.y, position.z - 1),
                new Vector3(position.x, position.y, position.z + 1),
            };
        }

        public Position getNeighborPosition (Vector3 position, Vector3 neighbor) {
            if (neighbor.z == position.z) { // horizontal relation --
                return (neighbor.x > position.x) ? Position.Right : Position.Left;
            } else {
                return (neighbor.z > position.z) ? Position.Top : Position.Bottom;
            }
        }

        public bool isAvailable(Vector3 pos) {
            return !grid.ContainsKey(pos);
        }

        public Machine getMachineAt (Vector3 pos) {
            return machines[grid[pos]];
        }

        public void place (Machine item) {
            if (!isAvailable(item.position) || item.info.price > money) {
                return;
            }
            machines.Add(item.position, item);

            foreach (Vector3 pos in getMachineBlocks(item.position, item.type)) {
                grid.Add(pos, item.position);
            }
            money -= item.info.price;
        }

        public Vector3[] getMachineBlocks (Vector3 gridPosition, Machine.Type type) {
            MachineInfo machineInfo = getMachineInfo(type);
            Vector3[] blocks = new Vector3[machineInfo.x * machineInfo.y];
            Vector3 extraBlock = gridPosition;

            int i = 0;
            int x = 0;
            int y = 0;

            while (x < machineInfo.x) {
                y = 0;
                extraBlock.z = gridPosition.z;

                while (y < machineInfo.y) {
                    blocks[i] = extraBlock;
                    extraBlock.z++;
                    y++;
                    i++;
                }

                extraBlock.x++;
                x++;
            }
            return blocks;
        }

        public MachineInfo getMachineInfo(Machine.Type type) {
            foreach (MachineInfo machineInfo in machinesInfo) {
                if (machineInfo.type == type) {
                    return machineInfo;
                }
            }
            throw new System.Exception("Requested machine type not found: " + type);
        }
        public ItemInfo getItemInfo(Item.Type type) {
            foreach (ItemInfo itemInfo in itemsInfo) {
                if (itemInfo.type == type) {
                    return itemInfo;
                }
            }
            throw new System.Exception("Requested item type not found: " + type);
        }

        // GameGrid.cs (添加监控)
        void OnGUI()
        {
            GUILayout.Label($"待创建物品: {itemsToCreate.Count}");
            GUILayout.Label($"待移动物品: {itemsToMove.Count}");

            foreach (var machine in machines.Values)
            {
                if (machine.storedItems.Count > 0)
                {
                    string items = string.Join(",",
                        machine.storedItems.Select(kv => $"{kv.Key}:{kv.Value}"));
                    GUILayout.Label($"{machine.type} 库存: {items}");
                }
            }
        }

        public void ClearAllPlayerBuildings()
        {
            // 1. 创建待删除列表（避免修改正在遍历的集合）
            List<Machine> machinesToDelete = new List<Machine>(machines.Values);
            List<Item> itemsToDelete = new List<Item>(items.Values);

            // 2. 删除所有物品实体
            foreach (Item item in itemsToDelete)
            {
                if (item.transform != null)
                {
                    Destroy(item.transform.gameObject);
                    Destroy(item.healthBar);
                }
                items.Remove(item.id);
            }
            itemsToCreate.Clear();
            itemsToMove.Clear();

            // 3. 删除所有机器
            foreach (Machine machine in machinesToDelete)
            {
                // 删除机器游戏对象
                if (machine.gameObject != null)
                {
                    Destroy(machine.gameObject);
                }

                // 清理网格占用
                Vector3[] blocks = getMachineBlocks(machine.position, machine.type);
                foreach (Vector3 block in blocks)
                {
                    grid.Remove(block);
                }
            }

            // 4. 重置核心数据结构
            machines.Clear();
            grid.Clear();
            items.Clear();
        }
    }
}