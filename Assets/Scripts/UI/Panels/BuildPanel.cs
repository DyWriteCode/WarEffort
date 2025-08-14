using FactorySystem;
using FactorySystem.Machines;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildPanel : MonoBehaviour
{
    public static BuildPanel Instance;
    public Transform spacePreviewer;

    public Material canBuildMat;
    public Material cannotBuildMat;

    private Vector3 previousMousePosition = Vector3.zero;
    private List<Vector3> currentBelt = new List<Vector3>();
    private MachineInfo selectedMachine;
    private bool isPlacing = false;

    private Vector3 _placementStartPosition; // 记录放置开始位置

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (selectedMachine == null) {
            return;
        }
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool gotHit = Physics.Raycast(ray, out hitInfo);

        if (gotHit && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            moveSpacePreviewer(hitInfo.point);
        }
        if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && gotHit) {
            isPlacing = true;
            // it's on the same PlaceMachine, not worth doing anything
            if (previousMousePosition == hitInfo.point) {
                return;
            }
            if (previousMousePosition == Vector3.zero) {
                previousMousePosition = hitInfo.point;
            }

            if (selectedMachine.type == Machine.Type.Belt) 
            {
                placeBelt(hitInfo.point);
            } 
            else 
            {
                placeMachine(hitInfo.point);
            }
        } else if (Input.GetMouseButton(1) && gotHit) {
            removeMachine(hitInfo.point);
        }

        // on mouse release reset some vars
        if (Input.GetMouseButtonUp(0) && isPlacing) {
            validateBelt();
            isPlacing = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 记录放置开始位置
            _placementStartPosition = Tools.GetMouseWorldPosition();
        }
    }

    public void ShowMachineInfo (Machine.Type type) {
        InfoPanel.Instance.setSelectedMachine(GameApp.MachineManager.GetMachineInfo(type));
    }

    private void moveSpacePreviewer (Vector3 position) {
        Vector3 gridPosition = GameApp.GameGridManager.GetNearestPointOnGrid(position);
        bool canPlaceIt = true;

        // machines may take multiple blocks, so we need to make sure all are available
        foreach (Vector3 blockPosition in GameApp.MachineManager.GetMachineBlocks(gridPosition, selectedMachine.type)) {
            if (!GameApp.MachineManager.IsPositionOccupied(blockPosition)) {
                canPlaceIt = false;
                break;
            }
        }

        spacePreviewer.GetComponent<MeshRenderer>().material = canPlaceIt ? canBuildMat : cannotBuildMat;
        spacePreviewer.transform.position = getPlacingPosition(gridPosition);
    }

    private Vector3 getPlacingPosition (Vector3 gridPosition) {
        // to fit blocks bigger than 1x1 in a 1x1 grid, we make this hack
        return gridPosition + new Vector3((selectedMachine.x - 1) * 0.5f, 0, (selectedMachine.y - 1) * 0.5f);
    }

    private void validateBelt() {
        previousMousePosition = Vector3.zero;
        currentBelt = new List<Vector3>();
    }

    private void removeMachine(Vector3 clickPoint) {
        Vector3 gridPosition = GameApp.GameGridManager.GetNearestPointOnGrid(clickPoint);

        // avoid overlapping
        if (GameApp.MachineManager.IsPositionOccupied(gridPosition)) {
            return;
        }

        GameApp.MachineManager.GetMachineAt(gridPosition).Delete();
    }

    private void placeMachine (Vector3 clickPoint) {

        if (!CanAffordMachine())
        {
            Debug.LogWarning($"资金不足! 需要 {selectedMachine.price}$");
            return;
        }

        Vector3 gridPosition = GameApp.GameGridManager.GetNearestPointOnGrid(clickPoint);
        Machine machine = null;

        // 机器可能需要多个块，因此我们需要确保所有块都可用
        foreach (Vector3 blockPosition in GameApp.MachineManager.GetMachineBlocks(gridPosition, selectedMachine.type)) 
        {
            // 避免重叠
            if (gridPosition.y > 0.5f || !GameApp.MachineManager.IsPositionOccupied(blockPosition, selectedMachine.type)) 
            {
                previousMousePosition = gridPosition;
                return;
            }
        }

        GameObject obj = Instantiate(selectedMachine.prefab, getPlacingPosition(gridPosition), Quaternion.identity);
        //GameObject obj = Instantiate(selectedMachine.prefab, gridPosition, Quaternion.identity);
        obj.name = gridPosition.ToString();

        //Vector3 currentPosition = Tools.GetMouseWorldPosition();
        //Machine.Direction direction = DirectionHelper.GetDirectionFromMouseMovement(
        //    _placementStartPosition,
        //    currentPosition
        //);

        //// 创建机器
        //Machine machine = GameApp.MachineManager.CreateAndPlaceMachine(
        //    selectedMachine.type,
        //    currentPosition,
        //    direction
        //);
        if (selectedMachine.type == Machine.Type.AttackMachine)
        {
            machine = new AttackMachine(obj);
        }
        else if (selectedMachine.type == Machine.Type.CleanerMachine)
        {
            machine = new CleanerMachine(obj);
        }
        else
        {
            machine = new Machine(obj, selectedMachine.type, gridPosition);
        }
        GameApp.MachineManager.PlaceMachine(machine);
    }

    private void placeBelt(Vector3 clickPoint) {

        if (!CanAffordMachine())
        {
            Debug.LogWarning($"资金不足! 需要 {selectedMachine.price}$");
            return;
        }

        Vector3 finalPosition = GameApp.GameGridManager.GetNearestPointOnGrid(clickPoint);

        // avoid overlapping
        if (finalPosition.y > 0.5f || !GameApp.MachineManager.IsPositionOccupied(finalPosition)) {
            // seems like he's trying to link existing belts
            if (previousMousePosition != finalPosition &&
                !GameApp.MachineManager.IsPositionOccupied(previousMousePosition) &&
                GameApp.MachineManager.GetMachineAt(finalPosition) != GameApp.MachineManager.GetMachineAt(previousMousePosition)
                //GameGirdManager.Instance.GetMachineAt(previousMousePosition).type == Machine.Type.Belt &&
                //GameGirdManager.Instance.GetMachineAt(finalPosition).type == Machine.Type.Belt
                ) 
            {
                GameApp.MachineManager.GetMachineAt(finalPosition).AddConnection(previousMousePosition, Machine.ConnectionType.Input);
                GameApp.MachineManager.GetMachineAt(previousMousePosition).AddConnection(finalPosition, Machine.ConnectionType.Output);
            }
            previousMousePosition = finalPosition;
            return;
        }

        Vector3 mouseDirection = finalPosition - previousMousePosition;
        Vector3 itemDirection = Vector3.zero;
        Machine.Direction direction = Machine.Direction.Right;

        if (mouseDirection.x < 0) {
            itemDirection.y = (float)Machine.Direction.Left;
            direction = Machine.Direction.Left;
        } else if (mouseDirection.z > 0) {
            itemDirection.y = (float)Machine.Direction.Up;
            direction = Machine.Direction.Up;
        } else if (mouseDirection.z < 0) {
            itemDirection.y = (float)Machine.Direction.Down;
            direction = Machine.Direction.Down;
        }

        GameObject obj = Instantiate(selectedMachine.prefab, finalPosition, Quaternion.Euler(itemDirection));
        obj.name = finalPosition.ToString();
        Belt machine = new Belt(obj);
        machine.direction = direction;

        // if player started dragging from an existing conveyor belt, link it to the new one
        if (GameApp.MachineManager.GetMachineAt(previousMousePosition) != null)
        {
            if (previousMousePosition != Vector3.zero &&
            !currentBelt.Contains(previousMousePosition) &&
            !GameApp.MachineManager.IsPositionOccupied(previousMousePosition) &&
            GameApp.MachineManager.GetMachineAt(previousMousePosition).type == Machine.Type.Belt)
            {
                if (currentBelt.Count > 0)
                {
                    validatePreviousBlockDirection(GameApp.MachineManager.GetMachineAt(currentBelt.Last()), GameApp.MachineManager.GetMachineAt(previousMousePosition));

                    GameApp.MachineManager.GetMachineAt(currentBelt.Last()).AddConnection(previousMousePosition, Machine.ConnectionType.Output);
                    GameApp.MachineManager.GetMachineAt(previousMousePosition).AddConnection(currentBelt.Last(), Machine.ConnectionType.Input);
                }
                currentBelt.Add(previousMousePosition);
            }
        }
        GameApp.MachineManager.PlaceMachine(machine);;

        Belt lastMachine = null;

        if (currentBelt.Count > 0) {
            lastMachine = (Belt)GameApp.MachineManager.GetMachineAt(currentBelt.Last());

            // new item and latest one are not neighbors
            if (!GameApp.MachineManager.GetNeighbors(finalPosition).Contains(lastMachine.position)) {
                var lastItemNeighbors = GameApp.MachineManager.GetNeighbors(lastMachine.position);

                // find a common available neighbor to create the union
                foreach (Vector3 pos in GameApp.MachineManager.GetNeighbors(finalPosition)) {
                    if (lastItemNeighbors.Contains(pos) && GameApp.MachineManager.IsPositionOccupied(pos)) {
                        placeBelt(pos);

                        // refresh last item
                        lastMachine = (Belt)GameApp.MachineManager.GetMachineAt(currentBelt.Last());
                        break;
                    }
                }
            }
        }

        currentBelt.Add(finalPosition);

        if (lastMachine != null) {
            validatePreviousBlockDirection(lastMachine, machine);

            // connect them
            GameApp.MachineManager.GetMachineAt(lastMachine.position).AddConnection(finalPosition, Machine.ConnectionType.Output);
            GameApp.MachineManager.GetMachineAt(finalPosition).AddConnection(lastMachine.position, Machine.ConnectionType.Input);
        }

        previousMousePosition = finalPosition;
    }

    void validatePreviousBlockDirection(Machine previousMachine, Machine nextMachine) {
        // change previous block direction if necessary
        if (previousMachine.position.z == nextMachine.position.z) { // horizontal relation --
            Vector3 newRotation = Vector3.zero;
            Machine.Direction newDirection = (previousMachine.position.x > nextMachine.position.x) ? Machine.Direction.Left : Machine.Direction.Right;

            newRotation.y = (float)newDirection;
            GameApp.MachineManager.GetMachineAt(previousMachine.position).gameObject.transform.rotation = Quaternion.Euler(newRotation);
            GameApp.MachineManager.GetMachineAt(previousMachine.position).direction = newDirection;

        } else if (previousMachine.position.x == nextMachine.position.x && previousMachine.direction != nextMachine.direction) { // vertical relation :
            GameApp.MachineManager.GetMachineAt(previousMachine.position).gameObject.transform.rotation = nextMachine.gameObject.transform.rotation;
            GameApp.MachineManager.GetMachineAt(previousMachine.position).direction = nextMachine.direction;
        }
    }

    public void SelectMachine(Machine.Type type) {
        selectedMachine = GameApp.MachineManager.GetMachineInfo(type);
        showSpacePreviewer();
    }

    public void StopBuilding () {
        selectedMachine = null;
        spacePreviewer.gameObject.SetActive(false);
    }

    private void showSpacePreviewer () {
        spacePreviewer.GetComponent<MeshRenderer>().material = cannotBuildMat;
        spacePreviewer.gameObject.SetActive(true);
        spacePreviewer.localScale = new Vector3(selectedMachine.x, 1, selectedMachine.y);
    }

    private bool CanAffordMachine()
    {
        return selectedMachine != null &&
               GameApp.EcomoneyManager.CanAfford(selectedMachine.price);
    }

}
