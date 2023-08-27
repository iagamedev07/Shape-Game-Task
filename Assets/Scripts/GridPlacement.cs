using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Vector3 = UnityEngine.Vector3;
using Vector3Int = UnityEngine.Vector3Int;

public class GridPlacement : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridVisual;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask shapeLayerMask;

    //Shape Object Variables
    [SerializeField] private GameObject shapePrefab;
    [HideInInspector] public Shape shapeInRange;
    private Shape selectedShape;

    //Grid Related Variables
    [SerializeField] private Transform placementGround;
    [SerializeField] private int maxLevel;
    public int currentLevel;

    private Vector3 lastPos;

    //Input & Inventory
    private GameInputActions inputActions;
    private InventorySystem inventorySystem;

    //Current State Variable in State Design Implementation
    private IPlacementState currentState;

    private void Awake()
    {
        inputActions = InputManager.Instance.gameInput;
        inventorySystem = GetComponent<InventorySystem>();
    }

    private void Start()
    {
        currentState = IdleState.Instance;
        gridVisual.SetActive(false);

        shapeInRange = null;
    }

    private void OnEnable()
    {
        inputActions.Placement.Enable();
        inputActions.Placement.Level.performed += OnChangeLevel;
        inputActions.Placement.Select.performed += OnSelectObject;
        inputActions.Placement.Place.performed += OnPlaceObject;
        inputActions.Placement.Rotate.performed += OnRotateObject;
        inputActions.Placement.Toggle.performed += OnTogglePlacement;
    }

    private void OnDisable()
    {
        inputActions.Placement.Disable();
        inputActions.Placement.Level.performed -= OnChangeLevel;
        inputActions.Placement.Select.performed -= OnSelectObject;
        inputActions.Placement.Place.performed -= OnPlaceObject;
        inputActions.Placement.Rotate.performed -= OnRotateObject;
        inputActions.Placement.Toggle.performed -= OnTogglePlacement;
    }

    private void Update()
    {
        currentState.Update(this);
    }

    #region Input Functions
    public void SetState(IPlacementState state)
    {
        currentState = state;
    }

    public void OnSelectObject(InputAction.CallbackContext context)
    {
        if (shapeInRange == null)
            return;

        currentState.SelectObject(this);
    }

    public void OnPlaceObject(InputAction.CallbackContext context)
    {
        if (currentState is IdleState)
            return;

        if (!selectedShape.canBePlaced)
            return;

        currentState.PlaceObject(this);
    }

    public void OnChangeLevel(InputAction.CallbackContext context)
    {
        currentState.UpdateLevel(this, context.ReadValue<float>());
    }

    public void OnRotateObject(InputAction.CallbackContext context)
    {
        if(currentState is PlacingState)
        {
            float val = context.ReadValue<float>();
            RotateObject((int)val);
        }   
    }

    public void OnTogglePlacement(InputAction.CallbackContext context)
    {

        if (inventorySystem.IsInventoryEmpty())
            return;

        //if current slot has shape or not
        if (inventorySystem.inventory[inventorySystem.selectedItem].shape == null)
            return;

        if (!inventorySystem.inventory[inventorySystem.selectedItem].enabled)
            return;       

        currentState.TogglePlacement(this);
    }

    #endregion

    #region State Functions

    public void UpdatePlacementPosition()
    {
        Vector3 cursorPos = GetMapPosition();
        Vector3Int gridPos = grid.WorldToCell(cursorPos);
        selectedShape.transform.position = grid.CellToWorld(gridPos);
    }

    public void UpdateSelectionStatus()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10, shapeLayerMask))
        {
            //Update the object in range and indication
            shapeInRange = hit.transform.GetComponent<Shape>();
        }else
        {
            // Nothing in range
            shapeInRange = null;
        }
    }

    public void SelectShapeInRange()
    {

        selectedShape = shapeInRange;
        shapeInRange = null;

        inventorySystem.AddItem(selectedShape, selectedShape.Icon);
        selectedShape.gameObject.SetActive(false);

    }

    public void TogglePlacementMode(bool value)
    {
        selectedShape = inventorySystem.inventory[inventorySystem.selectedItem].shape;

        selectedShape.gameObject.SetActive(value);
        selectedShape.TogglePlacementMode(true);

        InputManager.Instance.ToggleInventoryInputs(!value);
        InputManager.Instance.ToggleUIInputs(!value);

        //Enable/Disable the grid Preview
        gridVisual.SetActive(value);
    }

    public void PlaceObject()
    {
        //Delete the item from inventory
        inventorySystem.RemoveItem(inventorySystem.selectedItem);

        //Toggle the placement gui for the shape
        selectedShape.TogglePlacementMode(false);

        //Re-enable Inputs
        InputManager.Instance.ToggleInventoryInputs(true);
        InputManager.Instance.ToggleUIInputs(true);

        //Disable the grid Preview
        gridVisual.SetActive(false);
    }


    #endregion

    private void RotateObject(int val)
    {
        if (selectedShape == null)
            return;
        selectedShape.transform.Rotate(0f, 90f * val, 0f);
    }

    private Vector3 GetMapPosition()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward,out hit, 100, groundLayerMask))
        {
            lastPos = hit.point;
        }
        return lastPos;
    }

    #region PlacementLevel

    public void ChangeGridLevel(float val)
    {
        if(val >= 1) 
        {
            ChangeCurrentLevel(1);
        }
        else if(val <= 1)
        {
            ChangeCurrentLevel(-1);
        }

        UpdatePlacementGround();
    }

    private void ChangeCurrentLevel(int diff)
    {
        if (currentLevel <= 0 && diff == -1)
            return;

        if (currentLevel >= maxLevel && diff == 1)
            return;

        currentLevel += diff;
            
    }

    private void UpdatePlacementGround()
    {
        placementGround.position = new Vector3(placementGround.position.x,
                                               Constants.gridSize * currentLevel,
                                               placementGround.position.z);
    }
    #endregion

    #region Helper Functions

    #endregion
}
