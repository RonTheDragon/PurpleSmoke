using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour , IPlayerComponent
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private Button _inventoryFirstSelected;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Transform _inventoryContent;

    [SerializeField] private MultiplayerEventSystem _multiplayerEventSystem;

    [SerializeField] private InventoryItemUI _itemUItoSpawn;
    [SerializeField] private InventoryItemSlot _meleeSlot, _rangeSlot, _dynamicSlot, _staticSlot;
    [SerializeField] private GameObject _canBeDroppedUI;

    private PlayerComponentsRefrences _playerComponents;
    private PlayerHealth _playerHealth;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerEquipUI _playerEquipUI;

    private GameObject _selected;

    private InventoryItemUI _itemToSwitch;

    [SerializeField] private List<InventoryItemWithAmount> _inventoryItems;
    private List<InventoryItemUI> _uiOfItems = new List<InventoryItemUI>();

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerComponents = playerComponents;
        _playerHealth = _playerComponents.GetPlayerHealth;
        _playerCombatSystem = _playerComponents.GetPlayerCombatSystem;
        _playerEquipUI = _playerComponents.GetPlayerEquipUI;

        _meleeSlot.GetButton.onClick.AddListener(MeleeSlotClear);
        _rangeSlot.GetButton.onClick.AddListener(RangeSlotClear);
        _staticSlot.GetButton.onClick.AddListener(StaticSlotClear);
        _dynamicSlot.GetButton.onClick.AddListener(DynamicSlotClear);
        _meleeSlot.SetColor();
        _rangeSlot.SetColor();
        _staticSlot.SetColor();
        _dynamicSlot.SetColor();

        SetUpInventoryContent();
        UpdateCanBeDroppedUI();
    }

    public GameObject GetSelected => _selected;

    public bool IsInventoryOpen => _inventoryUI.activeSelf;

    public InventoryItemUI GetSelectedItemUI()
    {
        foreach (InventoryItemUI item in _uiOfItems)
        {
            if (item.gameObject == _selected)
                return item;
        }
        return null;
    }


    public bool InventoryInput()
    {
        if (_playerHealth.GetIsDead)
        {
            return false;
        }

        if (_inventoryUI.activeSelf)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }

        return _inventoryUI.activeSelf;
    }

    public void InteractInput()
    {
        InventoryItemUI selectedItem = GetSelectedItemUI();
        if (selectedItem != null)
        {
            if (_itemToSwitch == null)
            {
                SetItemToSwitch(selectedItem);
            }
            else if (_itemToSwitch.gameObject == _selected)
            {
                SetItemToSwitch(null);
            }
            else
            {
                SwitchPlaces(_itemToSwitch, selectedItem);
                SetItemToSwitch(null);
            }
        }
    }

    private void SetItemToSwitch(InventoryItemUI newitem)
    {
        if (newitem == null && _itemToSwitch != null)
        {
            _itemToSwitch.SetSwitchPlaceUI(false);
            _itemToSwitch = null;
        }
        else if (newitem != null)
        {
            _itemToSwitch = newitem;
            _itemToSwitch.SetSwitchPlaceUI(true);
        }
    }

    private void SwitchPlaces(InventoryItemUI itemToSwitch, InventoryItemUI newLocation)
    {
        // Find the inventory items associated with the UI items
        InventoryItemWithAmount itemA = FindInventoryItem(itemToSwitch);
        InventoryItemWithAmount itemB = FindInventoryItem(newLocation);

        if (itemA != null && itemB != null)
        {
            // Swap the items in the inventory list
            InventoryItemWithAmount temp = itemA;
            int indexA = _inventoryItems.IndexOf(itemA);
            int indexB = _inventoryItems.IndexOf(itemB);

            _inventoryItems[indexA] = itemB;
            _inventoryItems[indexB] = temp;

            _uiOfItems[indexA] = itemB.UiOfItem;
            _uiOfItems[indexB] = temp.UiOfItem;

            newLocation.transform.SetSiblingIndex(indexA);
            itemToSwitch.transform.SetSiblingIndex(indexB);

            SetSelected(itemToSwitch.gameObject); // handle UI selection into the new location
            RefreshInventoryShortcutOrder();
        }
    }

    public void SlotInput(int slotIndex)
    {
        if (_playerEquipUI.IsSwitchedShortcuts) slotIndex += 4;

        if (slotIndex < _uiOfItems.Count)
            _uiOfItems[slotIndex]?.UseItem();
    }

    private void OpenInventory()
    {
        _inventoryUI.SetActive(true);
        _multiplayerEventSystem.SetSelectedGameObject(null);
        _multiplayerEventSystem.SetSelectedGameObject(_selected);
    }

    public void CloseInventory()
    {
        _inventoryUI.SetActive(false);
        SetItemToSwitch(null);
    }


    public void SelectionFix()
    {
        if (!_inventoryUI.activeSelf) return;

        if (_multiplayerEventSystem.currentSelectedGameObject == null)
        {
            _multiplayerEventSystem.SetSelectedGameObject(_selected);
        }
        _selected = _multiplayerEventSystem.currentSelectedGameObject;

        ScrollToSelected();
        UpdateCanBeDroppedUI();
    }

    private void UpdateCanBeDroppedUI()
    {
        if (GetSelectedItemUI() == null) { _canBeDroppedUI.SetActive(false); return; }
        _canBeDroppedUI.SetActive(GetSelectedItemUI().GetInventoryItem.CanBeDropped);
    }


    private void ScrollToSelected()
    {
        RectTransform _selectedRectTransform = (RectTransform)_selected.transform;

        RectTransform viewport = _scrollRect.viewport;
        RectTransform content = _scrollRect.content;

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;
        float targetPosY = -_selectedRectTransform.anchoredPosition.y;

        // Calculate scroll position
        float scrollPos = Mathf.Clamp01((targetPosY - viewportHeight / 2) / (contentHeight - viewportHeight));

        // Set the scroll position
        _scrollRect.verticalNormalizedPosition = 1 - scrollPos;
    }

    private void SetUpInventoryContent()
    {

        bool first = true;
        foreach (InventoryItemWithAmount item in _inventoryItems)
        {
            CreateItemUI(item);
            if (first)
            {
                first = false;
                SetSelected(item.UiOfItem.gameObject);
            }
        }

        FixNothingSelected();
        _playerEquipUI.Initialize(_playerComponents);
        RefreshInventoryShortcutOrder();
    }

    public void RefreshInventoryShortcutOrder()
    {
        if (_uiOfItems.Count == 0) return;
        for (int i = 0; i < _uiOfItems.Count; i++)
        {
            if (i < 8)
            {
                _uiOfItems[i].SetShortcutKey(i + 1);
            }
            else
            {
                _uiOfItems[i].SetShortcutKey(0);
            }
        }
        _playerEquipUI.SetUpShortcutsDisplay(_uiOfItems);
    }

    private void CreateItemUI(InventoryItemWithAmount item)
    {
        InventoryItemUI i = Instantiate(_itemUItoSpawn, _inventoryContent.position, _inventoryContent.rotation, _inventoryContent);
        i.SetUpItemUI(item, this);
        item.UiOfItem = i;
        _uiOfItems.Add(i);
    }

    private void FixNothingSelected()
    {
        if (_selected == null)
        {
            SetSelected(_inventoryFirstSelected.gameObject);
        }
    }

    public bool SpendConsumable(ConsumableItem consumable)
    {
        return _playerCombatSystem.ConsumeConsumable(consumable.GetConsumable);
    }


    public void SetMeleeWeapon(MeleeItem melee, InventoryItemUI itemUi)
    {
        SetItemInSlot(melee.GetMoveSet, _playerCombatSystem.GetCurrentMeleeMoveSet,
                _playerCombatSystem.SetMeleeMoveSet, _meleeSlot, _playerEquipUI.GetMeleeSlot, itemUi);
    }

    public void SetRangeWeapon(RangeItem range, InventoryItemUI itemUi)
    {
        SetItemInSlot(range.GetMoveSet, _playerCombatSystem.GetCurrentRangeMoveSet,
                _playerCombatSystem.SetRangeMoveSet, _rangeSlot, _playerEquipUI.GetRangeSlot, itemUi);
    }

    public void SetStaticUseable(UseableItem useable, InventoryItemUI itemUi)
    {
        SetItemInSlot(useable.GetUseable, _playerCombatSystem.GetCurrentStaticUseable,
                _playerCombatSystem.SetStaticUseable, _staticSlot, _playerEquipUI.GetStaticSlot, itemUi);
    }

    public void SetDynamicUseable(UseableItem useable, InventoryItemUI itemUi)
    {
        SetItemInSlot(useable.GetUseable, _playerCombatSystem.GetCurrentDynamicUseable,
                _playerCombatSystem.SetDynamicUseable, _dynamicSlot, _playerEquipUI.GetDynamicSlot, itemUi);
    }

    private void SetItemInSlot<T>(T newItem, T currentItem,
                            Action<T, InventoryItemUI> setItemAction, InventoryItemSlot slot, EquipDisplayItemSlot eSlot, InventoryItemUI itemUI) where T : Component
    {
        // Check if newItem is not null and compare it with currentItem
        if (newItem != null && (currentItem == null || newItem.name != currentItem?.name))
        {
            // Set the new item in the system and the UI
            setItemAction(newItem, itemUI);
            slot.SetSlot(itemUI);
            eSlot.SetSlot(itemUI);
        }
        else
        {
            // Handle case when newItem is null (remove item)
            setItemAction(default, null);  // Reset item in the combat system and UI
            slot.ClearSlot();  // Clear the visual slot
            eSlot.ClearSlot();  // Clear the equipment slot visual
        }
    }


    public void MeleeSlotClear()
    {
        _playerCombatSystem.SetMeleeMoveSet(null,null);
        _meleeSlot.ClearSlot();
        _playerEquipUI.GetMeleeSlot.ClearSlot();
    }
    public void RangeSlotClear()
    {
        _playerCombatSystem.SetRangeMoveSet(null, null);
        _rangeSlot.ClearSlot();
        _playerEquipUI.GetRangeSlot.ClearSlot();
    }
    public void StaticSlotClear()
    {
        _playerCombatSystem.SetStaticUseable(null, null);
        _staticSlot.ClearSlot();
        _playerEquipUI.GetStaticSlot.ClearSlot();
    }
    public void DynamicSlotClear()
    {
        _playerCombatSystem.SetDynamicUseable(null, null);
        _dynamicSlot.ClearSlot();
        _playerEquipUI.GetDynamicSlot.ClearSlot();
    }

    public void RemoveOneItem(InventoryItemUI itemUI)
    {
        RemoveAmountFromItem(itemUI);
    }

    public void RemoveAmountFromItem(InventoryItemUI itemUI, int amount=1)
    {
        InventoryItemWithAmount itemToRemove = FindInventoryItem(itemUI);

        if (itemToRemove != null)
        {
            itemToRemove.Amount-= amount;
            if (itemToRemove.Amount > 0)
            {
                itemUI.RemoveAmountFromItem();
            }
        }
    }

    public void RemoveItemIfEmpty(InventoryItemUI itemUI)
    {
        if (IsItemEmpty(itemUI)) RemoveItemStack(itemUI);
    }
   

    public bool IsItemEmpty(InventoryItemUI itemUI)
    {
        InventoryItemWithAmount itemToRemove = FindInventoryItem(itemUI);
        if (itemToRemove != null)
        {
            if (itemToRemove.Amount <= 0)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveItemStack(InventoryItemUI itemUI)
    {
        InventoryItemWithAmount itemToRemove = FindInventoryItem(itemUI);

        if (itemToRemove != null)
        {
            RemoveWholeItem(itemToRemove);
        }
    }

    public void RemoveWholeItem(InventoryItemUI itemUI)
    {
        InventoryItemWithAmount itemToRemove = FindInventoryItem(itemUI);
        if (IsSelectingThat(itemToRemove.UiOfItem.gameObject))
        {
            MoveSelectionAway();
        }
        _inventoryItems.Remove(itemToRemove);
        _uiOfItems.Remove(itemToRemove.UiOfItem);
        itemToRemove.UiOfItem.OnAmountChange = null;
        Destroy(itemToRemove.UiOfItem.gameObject);
        RefreshInventoryShortcutOrder();
    }

    public void RemoveWholeItem(InventoryItemWithAmount itemToRemove)
    {
        if (IsSelectingThat(itemToRemove.UiOfItem.gameObject))
        {
            MoveSelectionAway();
        }
        _inventoryItems.Remove(itemToRemove);
        _uiOfItems.Remove(itemToRemove.UiOfItem);
        itemToRemove.UiOfItem.OnAmountChange = null;
        Destroy(itemToRemove.UiOfItem.gameObject);
        RefreshInventoryShortcutOrder();
    }

    private InventoryItemWithAmount FindInventoryItem(InventoryItemUI itemUI)
    {
        return _inventoryItems.Find(item => item.Item == itemUI.GetInventoryItem);
    }

    public void AddInventoryItem(InventoryItem item, int amount = 1)
    {
        foreach (InventoryItemWithAmount i in _inventoryItems)
        {
            if (i.Item == item)
            {
                i.Amount += amount;       
                i.UiOfItem.AddAmountToItem(amount);
                
                return;
            }
        }
        InventoryItemWithAmount ItemWithAmount = new InventoryItemWithAmount() { Item = item, Amount = amount };
        _inventoryItems.Add(ItemWithAmount);
        CreateItemUI(ItemWithAmount);
        RefreshInventoryShortcutOrder();
    }

    private void SetSelected(GameObject newSelected)
    {
        _selected = newSelected;
        _multiplayerEventSystem.SetSelectedGameObject(newSelected);
        UpdateCanBeDroppedUI();
    }

    private bool IsSelectingThat(GameObject possiblySelected)
    {
        if (_selected == null || possiblySelected == null)
        {
            return false;
        }

        // Compare the currently selected GameObject with the possiblySelected one
        return _selected == possiblySelected;
    }


    private void MoveSelectionAway()
    {
        // Find the index of the currently selected item
        int selectedIndex = _uiOfItems.FindIndex(u => u.gameObject == _selected.gameObject);

        // If selectedIndex is not found, return early
        if (selectedIndex == -1)
        {
            Debug.LogError("Selected item not found in the list.");
            return;
        }

        // Move back in the list
        selectedIndex--;

        // If index is out of bounds, loop back to the last item or set to the first selected object
        if (selectedIndex < 0)
        {
            if (_uiOfItems.Count <= 1)
            {
                _multiplayerEventSystem.SetSelectedGameObject(_inventoryFirstSelected.gameObject);
                _selected = _inventoryFirstSelected.gameObject;
            }
            else
            {
                selectedIndex += 2;
                _multiplayerEventSystem.SetSelectedGameObject(_uiOfItems[selectedIndex].gameObject);
                _selected = _uiOfItems[selectedIndex].gameObject;
            }
        }
        else
        {
            _multiplayerEventSystem.SetSelectedGameObject(_uiOfItems[selectedIndex].gameObject);
            _selected = _uiOfItems[selectedIndex].gameObject;
        }
        UpdateCanBeDroppedUI();
    }



    [Serializable]
    public class InventoryItemWithAmount
    {
        public InventoryItem Item;
        public int Amount = 1;
        [HideInInspector] public InventoryItemUI UiOfItem;
    }
}
