using FactorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public static InfoPanel Instance;
    public Text machineName;
    public Text description;
    public Text storedItems;
    public Text money;

    private Machine selectedMachine;
    private MachineInfo selectedMachineInfo;

    private bool updaterStarted = false;

    private void Awake() 
    {
        Instance = this;
    }

    void Update()
    {
        updateShownInfo();
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            trySelectMachine();
        }
    }

    private void trySelectMachine() {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool gotHit = Physics.Raycast(ray, out hitInfo);

        if (!gotHit) {
            return;
        }

        Vector3 gridPosition = GameApp.GameGridManager.GetNearestPointOnGrid(hitInfo.point);

        if (GameApp.MachineManager.IsPositionOccupied(gridPosition)) {
            return;
        }

        selectedMachine = GameApp.MachineManager.GetMachineAt(gridPosition);
        if (selectedMachine != null)
        {
            if (!updaterStarted)
            {
                updaterStarted = true;
                InvokeRepeating(nameof(updateShownInfo), 0, 1);
            }
            setSelectedMachine(selectedMachine.Info);
        }
    }

    public void setSelectedMachine (MachineInfo machineInfo) {
        selectedMachineInfo = machineInfo;

        machineName.text = machineInfo.name;
        description.text = machineInfo.description;

        if (machineInfo.executionType == Machine.ExecutionType.Converter) 
        {
            description.text += " | Converter ";
        }
        else if (machineInfo.executionType == Machine.ExecutionType.Building)
        {
            description.text += " | Building ";
        }
        else if (machineInfo.executionType == Machine.ExecutionType.Generator)
        {
            description.text += " | Generator ";
        }
        else if (machineInfo.executionType == Machine.ExecutionType.Seller)
        {
            description.text += " | Seller ";
        }
    }

    private void updateShownInfo () {
        string storedItemsSummary = "";
        if (selectedMachine != null)
        {
            if (selectedMachine.storedItems.Count > 0)
            {
                foreach (KeyValuePair<Item.Type, int> storedItem in selectedMachine.storedItems)
                {
                    int value = 0;
                    selectedMachineInfo.storageLimits.TryGetValue(storedItem.Key, out value);
                    storedItemsSummary += " " + storedItem.Key + ": " + storedItem.Value + "/" + value;
                }
            }
        }
        storedItems.text = storedItemsSummary;
        money.text = GameApp.EcomoneyManager.Money + "$";
    }
}
