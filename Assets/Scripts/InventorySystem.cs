using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public InventoryItem[] inventory = new InventoryItem[10];
    public int selectedItem = 0;

    public event Action onInventoryChanged;
    public event Action<int> onItemEquiped;

    private GameInputActions inputActions;

    private void Awake()
    {
        inputActions = InputManager.Instance.gameInput;
    }

    private void OnEnable()
    {
        inputActions.Hotbar.Enable();
        inputActions.Hotbar._1.performed += ctx => EquipItem(1);
        inputActions.Hotbar._2.performed += ctx => EquipItem(2);
        inputActions.Hotbar._3.performed += ctx => EquipItem(3);
        inputActions.Hotbar._4.performed += ctx => EquipItem(4);
        inputActions.Hotbar._5.performed += ctx => EquipItem(5);
        inputActions.Hotbar._6.performed += ctx => EquipItem(6);
        inputActions.Hotbar._7.performed += ctx => EquipItem(7);
        inputActions.Hotbar._8.performed += ctx => EquipItem(8);
        inputActions.Hotbar._9.performed += ctx => EquipItem(9);
        inputActions.Hotbar._0.performed += ctx => EquipItem(0);
    }

    private void OnDisable()
    {
        inputActions.Hotbar.Disable();
        inputActions.Hotbar._1.performed -= ctx => EquipItem(1);
        inputActions.Hotbar._2.performed -= ctx => EquipItem(2);
        inputActions.Hotbar._3.performed -= ctx => EquipItem(3);
        inputActions.Hotbar._4.performed -= ctx => EquipItem(4);
        inputActions.Hotbar._5.performed -= ctx => EquipItem(5);
        inputActions.Hotbar._6.performed -= ctx => EquipItem(6);
        inputActions.Hotbar._7.performed -= ctx => EquipItem(7);
        inputActions.Hotbar._8.performed -= ctx => EquipItem(8);
        inputActions.Hotbar._9.performed -= ctx => EquipItem(9);
        inputActions.Hotbar._0.performed -= ctx => EquipItem(0);
    }

    public void AddItem(Shape shapeObject, Sprite shapeIcon)
    {
        InventoryItem item = new InventoryItem
        {
            shape = shapeObject,
            iconSprite = shapeIcon,
            enabled = true
        };

        for (int i = 0; i < inventory.Length; i++) 
        {
            if (inventory[i].shape == null)
            {
                inventory[i] = item;
                break;
            }
        }
        onInventoryChanged?.Invoke();

        //Also Check if added item satisfies the filter. Normally I would have made a dedicated filter check method but currently there is one filter so we can directly call it
        UIManager.Instance.CheckComplexShapesFilter();
    }

   
    public void RemoveItem(int id)
    {
        inventory[id].shape = null;
        inventory[id].iconSprite = null;
        inventory[id].enabled = false;
        onInventoryChanged?.Invoke();
    }

    private void EquipItem(int itemNumber)
    {
        if(itemNumber == 0)
        {
            selectedItem = 9;
            onItemEquiped?.Invoke(9);
        }else
        {
            selectedItem = itemNumber - 1;
            onItemEquiped?.Invoke(itemNumber - 1);
        }
    }

    public bool IsInventoryEmpty()
    {
        for (int i = 0;i < inventory.Length;i++) 
        {
            if (inventory[i].shape != null)
                return false;
        }
        return true;
    }
}

[System.Serializable]
public class InventoryItem
{
    public Shape shape;
    public Sprite iconSprite;
    public bool enabled;
}
