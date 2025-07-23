using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    public const float FLOOR_HEIGHT = 3f;

    //public event EventHandler OnAnyUnitMovedGridPosition;
    public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition;
    public class OnAnyUnitMovedGridPositionEventArgs : EventArgs
    {
        public Unit unit;
        public GridPosition fromGridPosition;
        public GridPosition toGridPosition;
    }

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private bool seeGridDebugObjectPrefab;
    [SerializeField] private int floorAmount;
    private List<GridSystem<GridObject>> gridSystemList;
    public float CellScale;
    [SerializeField] Transform gameHolder;
    public Transform GetHolderTransform => gameHolder;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("LevelGrid " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gridSystemList = new List<GridSystem<GridObject>>();

        for (int floor = 0; floor < floorAmount; floor++)
        {
            GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
           // gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
            gridSystemList.Add(gridSystem);
        }

        if (seeGridDebugObjectPrefab)
        {
            //gridSystemList.CreateDebugObjects(gridDebugObjectPrefab);
        }   
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
    }

    private GridSystem<GridObject> GetGridSystem(int floor)
    {
       // Debug.Log("floor: "+floor);
        return gridSystemList[floor];
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPostion(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);

        //OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
        OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs
        {
            unit = unit,
            fromGridPosition = fromGridPosition,
            toGridPosition = toGridPosition,
        });
    }

    //public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public int GetFloor(Vector3 worldPosition)
    {
        //Debug.Log("World Position: " + worldPosition);
        //return Mathf.RoundToInt((worldPosition.y-transform.root.position.y) / FLOOR_HEIGHT);
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {

        //Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        int floor = GetFloor(worldPosition);
        GridPosition gridPosition= GetGridSystem(floor).GetGridPosition(worldPosition);
        
        Debug.Log("wolrdPos: "+ worldPosition.x + ", " + worldPosition.z);
        Debug.Log("gridPos: "+ gridPosition.x + ", " + gridPosition.z);


        return gridPosition;


        /*Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        int x = Mathf.RoundToInt(localPosition.x / cellSize);
        int z = Mathf.RoundToInt(localPosition.z / cellSize);
        int yFloor = Mathf.RoundToInt(localPosition.y / FLOOR_HEIGHT);

        // luego validas que yFloor esté en [0, gridSystems.Count)
        yFloor = Mathf.Clamp(yFloor, 0, gridSystemList.Count - 1);

        return GetGridSystem(yFloor).GetGridPosition(new Vector3(x, localPosition.y,z));
    */
    }

    //public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

    //public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        if (gridPosition.floor < 0 || gridPosition.floor >= floorAmount)
        {
            return false;
        }
        else
        {
            return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
        }
    }

    //public int GetWidth() => gridSystem.GetWidth();
    public int GetWidth() => GetGridSystem(0).GetWidth();

    //public int GetHeight() => gridSystem.GetHeight();
    public int GetHeight() => GetGridSystem(0).GetHeight();

    public int GetFloorAmount() => floorAmount;

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }

    public void ClearInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.ClearInteractable();
    }
}
