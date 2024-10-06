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

    [SerializeField] private ItemUI _itemUItoSpawn;
    [SerializeField] private InventoryItemSlot _meleeSlot, _rangeSlot, _dynamicSlot, _staticSlot;

    private GameManager _gameManager;
    private PickupPooler _pickupPooler;
    private PlayerComponentsRefrences _playerComponents;
    private PlayerHealth _playerHealth;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerEquipUI _playerEquipUI;

    private GameObject _selected;

    private ItemUI _itemToSwitch;

    [SerializeField] private List<InventoryItemWithAmount> _inventoryItems;
    private List<ItemUI> _uiOfItems = new List<ItemUI>();

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerComponents = playerComponents;
        _playerHealth = _playerComponents.GetPlayerHealth;
        _playerCombatSystem = _playerComponents.GetPlayerCombatSystem;
        _playerEquipUI = _playerComponents.GetPlayerEquipUI;

        _meleeSlot.GetButton.onClick.AddListener(MeleeSlotClick);
        _rangeSlot.GetButton.onClick.AddListener(RangeSlotClick);
        _staticSlot.GetButton.onClick.AddListener(StaticSlotClick);
        _dynamicSlot.GetButton.onClick.AddListener(DynamicSlotClick);

        SetUpInventoryContent();
    }

    public GameObject GetSelected => _selected;

    public bool IsInventoryOpen => _inventoryUI.activeSelf;

    public ItemUI GetSelectedItemUI()
    {
        foreach (ItemUI item in _uiOfItems)
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
        ItemUI selectedItem = GetSelectedItemUI();
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

    private void SetItemToSwitch(ItemUI newitem)
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

    private void SwitchPlaces(ItemUI itemToSwitch, ItemUI newLocation)
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
        }
    }

    public void SlotInput(int slotIndex)
    {
        if (slotIndex < _uiOfItems.Count)
            _uiOfItems[slotIndex]?.Equip();
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
    }

    private void CreateItemUI(InventoryItemWithAmount item)
    {
        ItemUI i = Instantiate(_itemUItoSpawn, _inventoryContent.position, _inventoryContent.rotation, _inventoryContent);
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


    public void SetMeleeWeapon(MeleeItem melee,ItemUI itemUi)
    {
        SetItemInSlot(melee.GetMoveSet, _playerCombatSystem.GetCurrentMeleeMoveSet,
                _playerCombatSystem.SetMeleeMoveSet, _meleeSlot,_playerEquipUI.GetMeleeSlot, itemUi);
    }

    public void SetRangeWeapon(RangeItem range, ItemUI itemUi)
    {
        SetItemInSlot(range.GetMoveSet, _playerCombatSystem.GetCurrentRangeMoveSet,
                _playerCombatSystem.SetRangeMoveSet, _rangeSlot, _playerEquipUI.GetRangeSlot, itemUi);
    }

    public void SetStaticUseable(UseableItem useable, ItemUI itemUi)
    {
        SetItemInSlot(useable.GetUseable, _playerCombatSystem.GetCurrentStaticUseable,
                _playerCombatSystem.SetStaticUseable, _staticSlot, _playerEquipUI.GetStaticSlot, itemUi);
    }

    public void SetDynamicUseable(UseableItem useable, ItemUI itemUi)
    {
        SetItemInSlot(useable.GetUseable, _playerCombatSystem.GetCurrentDynamicUseable,
                _playerCombatSystem.SetDynamicUseable, _dynamicSlot, _playerEquipUI.GetDynamicSlot, itemUi);
    }

    private void SetItemInSlot<T>(T newItem, T currentItem,
                                Action<T> setItemAction, InventoryItemSlot slot, EquipDisplayItemSlot eSlot, ItemUI itemUI) where T : Component
    {
        if (newItem != null && newItem.name != currentItem?.name)
        {
            setItemAction(newItem);
            slot.SetSlot(itemUI.GetInventoryItem.GetSprite,itemUI.GetAmount);
            eSlot.SetSlot(itemUI.GetInventoryItem.GetSprite, itemUI.GetAmount);
        }
        else 
        {
            setItemAction(default);
            slot.ClearSlot();
            eSlot.ClearSlot();
        }
    }

    private void MeleeSlotClick()
    {
        _playerCombatSystem.SetMeleeMoveSet(null);
        _meleeSlot.ClearSlot();
        _playerEquipUI.GetMeleeSlot.ClearSlot();
    }
    private void RangeSlotClick()
    {
        _playerCombatSystem.SetRangeMoveSet(null);
        _rangeSlot.ClearSlot();
        _playerEquipUI.GetRangeSlot.ClearSlot();
    }
    private void StaticSlotClick()
    {
        _playerCombatSystem.SetStaticUseable(null);
        _staticSlot.ClearSlot();
        _playerEquipUI.GetStaticSlot.ClearSlot();
    }
    private void DynamicSlotClick()
    {
        _playerCombatSystem.SetDynamicUseable(null);
        _dynamicSlot.ClearSlot();
        _playerEquipUI.GetDynamicSlot.ClearSlot();
    }

    public void RemoveOneItem(ItemUI itemUI)
    {
        InventoryItemWithAmount itemToRemove = FindInventoryItem(itemUI);

        if (itemToRemove != null)
        {
            itemToRemove.Amount--;
            if (itemToRemove.Amount <= 0)
            {
                RemoveWholeItem(itemToRemove);
            }
            else
            {
                itemUI.RemoveAmountFromItem();
            }
        }
    }

    public void RemoveItemStack(ItemUI itemUI)
    {
        InventoryItemWithAmount itemToRemove = FindInventoryItem(itemUI);

        if (itemToRemove != null)
        {
            RemoveWholeItem(itemToRemove);
        }
    }

    private void RemoveWholeItem(InventoryItemWithAmount itemToRemove)
    {
        if (IsSelectingThat(itemToRemove.UiOfItem.gameObject))
        {
            MoveSelectionAway();
        }
        _inventoryItems.Remove(itemToRemove);
        _uiOfItems.Remove(itemToRemove.UiOfItem);
        Destroy(itemToRemove.UiOfItem.gameObject);
    }

    private InventoryItemWithAmount FindInventoryItem(ItemUI itemUI)
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
        
    }

    private void SetSelected(GameObject newSelected)
    {
        _selected = newSelected;
        _multiplayerEventSystem.SetSelectedGameObject(newSelected);
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
    }



    [Serializable]
    public class InventoryItemWithAmount
    {
        public InventoryItem Item;
        public int Amount = 1;
        [HideInInspector] public ItemUI UiOfItem;
    }
}
