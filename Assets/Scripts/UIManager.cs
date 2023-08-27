using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
    private GameInputActions inputActions;

    [SerializeField] private Image[] hotbarSlots;
    [SerializeField] private Image equipIndicator;

    [SerializeField] private InventorySystem inventorySystem;

    [Header("Filter Menu")]
    [SerializeField]private RectTransform filterMenu;
    [SerializeField] private Toggle complexShapeToggle;
    [SerializeField] private float filterMenuDuration;
    WaitForSeconds filterMenuYield;

    private void Awake()
    {
        inputActions = InputManager.Instance.gameInput;
    }

    private void Start()
    {
        if(complexShapeToggle.gameObject.activeInHierarchy)
            complexShapeToggle.isOn = false;

        filterMenuYield = new WaitForSeconds(filterMenuDuration);

        //Reset Filter Menu Variables
        filterMenu.DOScale(0f, 0.1f);
        filterMenu.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.FilterMenu.performed += OnFilterMenu;

        inventorySystem.onInventoryChanged += UpdateHotbarUI;
        inventorySystem.onItemEquiped += UpdateEquipUI;
    }

    private void OnDisable()
    {
        inputActions.UI.Disable();
        inputActions.UI.FilterMenu.performed -= OnFilterMenu;

        inventorySystem.onInventoryChanged -= UpdateHotbarUI;
        inventorySystem.onItemEquiped -= UpdateEquipUI;
    }

    private void UpdateEquipUI(int index)
    {
        equipIndicator.rectTransform.DOAnchorPos(hotbarSlots[index].rectTransform.anchoredPosition, 0.1f);
    }

    /// <summary>
    /// Updates the UI of Inventory Bar (Bottom One)
    /// </summary>
    private void UpdateHotbarUI()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (inventorySystem.inventory[i].iconSprite != null && inventorySystem.inventory[i].enabled)
            {
                hotbarSlots[i].overrideSprite = inventorySystem.inventory[i].iconSprite;
                hotbarSlots[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                hotbarSlots[i].overrideSprite=null;
                hotbarSlots[i].color = new Color(1f, 1f, 1f, 0.5f);
            }
        }
    }

    #region Filter

    private void OnFilterMenu(InputAction.CallbackContext context)
    {
        if (filterMenu.gameObject.activeSelf)
        {
            StartCoroutine(DelayedFilterMenuClose());

            //Enables Input
            InputManager.Instance.ToggleInventoryInputs(true);
            InputManager.Instance.TogglePlacementInputs(true);
            InputManager.Instance.TogglePlayerInputs(true);

            //Disables Cursor
            InputManager.Instance.ToggleMouseCursor(false);
        }           
        else if (!filterMenu.gameObject.activeSelf)
        {
            filterMenu.gameObject.SetActive(true);
            filterMenu.DOScale(new Vector3(1f, 1f, 1f), filterMenuDuration).SetEase(Ease.OutBack);

            //Disables Input
            InputManager.Instance.ToggleInventoryInputs(false);
            InputManager.Instance.TogglePlacementInputs(false);
            InputManager.Instance.TogglePlayerInputs(false);

            //Enables Cursor
            InputManager.Instance.ToggleMouseCursor(true);
        }
            
    }

    public void ToggleFilterMenu(bool value)
    {

        if(!value)
        {
            StartCoroutine(DelayedFilterMenuClose());

            //Enables Input
            InputManager.Instance.ToggleInventoryInputs(true);
            InputManager.Instance.TogglePlacementInputs(true);
            InputManager.Instance.TogglePlayerInputs(true);

            //Disables Cursor
            InputManager.Instance.ToggleMouseCursor(false);
        }
        else if(value)
        {
            filterMenu.gameObject.SetActive(value);
            filterMenu.DOScale(new Vector3(1f,1f,1f), filterMenuDuration).SetEase(Ease.OutBack);

            //Disables Input
            InputManager.Instance.ToggleInventoryInputs(false);
            InputManager.Instance.TogglePlacementInputs(false);
            InputManager.Instance.TogglePlayerInputs(false);

            //Enables Cursor
            InputManager.Instance.ToggleMouseCursor(true);
        }

    }

    IEnumerator DelayedFilterMenuClose()
    {
        filterMenu.DOScale(new Vector3(0f, 0f, 0f), filterMenuDuration).SetEase(Ease.InBack);
        yield return filterMenuYield;
        filterMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the Inventory Items according to filter
    /// </summary>
    public void CheckComplexShapesFilter()
    {
        for (int i = 0; i < inventorySystem.inventory.Length; i++)
        {
            if (inventorySystem.inventory[i].shape != null)
            {
                if (complexShapeToggle.isOn)
                {
                    if (!inventorySystem.inventory[i].shape.isComplex)
                        inventorySystem.inventory[i].enabled = false;
                }else
                {
                    inventorySystem.inventory[i].enabled = true;
                }
            }            
        }

        UpdateHotbarUI();
    }

    #endregion
}
