using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

using Vector = UnityEngine.Vector3;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private float gridCellSize = 0.2f;
    [SerializeField] private LayerMask raycastLayers;

    [SerializeField] private GameObject previewPrefab;
    private GameObject _previewObject;
    private Shape selectedShape;

    private GameInputActions inputActions;

    private void Awake()
    {
        inputActions = InputManager.Instance.gameInput;
    }

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        _previewObject = Instantiate(previewPrefab);
        selectedShape = _previewObject.GetComponent<Shape>();
        _previewObject.SetActive(false);
    }

    private void Update()
    {
        RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward ,out hit, 100,raycastLayers)) 
        {
            Vector3 hitPoint = hit.point;
            Vector3 gridPosition = SnapToGrid(hitPoint, hit.normal);          
            
            _previewObject.transform.position = gridPosition;
            _previewObject.SetActive(true);
            

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                //_previewObject.transform.Rotate(0f, 90f, 0f);
                //selectedShape.SetPivotWithRotation();
            }
        }
        else
        {
            _previewObject.SetActive(false);
        }
    }


    private Vector3 SnapToGrid(Vector3 position, Vector3 normal)
    {
        Vector3 offset = normal * (gridCellSize * 0.5f);
        position += offset;

        float newX = Mathf.Floor(position.x / gridCellSize) * gridCellSize;
        float newY = Mathf.Floor(position.y / gridCellSize) * gridCellSize;
        float newZ = Mathf.Floor(position.z / gridCellSize) * gridCellSize;

        return new Vector3(newX, newY, newZ);
    }
 

   
    

}
