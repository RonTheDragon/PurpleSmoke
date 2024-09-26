using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour, IPlayerComponent
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _chargeBar;
    [SerializeField] private Image _acidationBar;
    [SerializeField] private GameObject _pressSpaceToGetUp;
    [SerializeField] private TMP_Text _respawnCountDown;
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private Button _inventoryFirstSelected;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Transform _inventoryContent;
    [SerializeField] private GameObject _acidCrosshair;

    [SerializeField] private MultiplayerEventSystem _multiplayerEventSystem;
    [SerializeField] private ItemUI _itemUItoSpawn;
    [SerializeField] private ItemSlot _meleeSlot, _rangeSlot, _dynamicSlot, _staticSlot;

    private PlayerComponentsRefrences _playerComponents;
    private Transform _playerBody;
    private PlayerHealth _playerHealth;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerKnockout _playerKnockout;
    private PlayerDeath _playerDeath;
    private PlayerAcidation _playerAcidation;

    private GameManager _gameManager;
    private PickupPooler _pickupPooler;

    private GameObject _selected;

    [SerializeField] private Vector2 _itemDropVelocity;
    [SerializeField] private float _itemDropProtectionTime, _itemDropHeight, _itemDropStackTime;


    private Color _green = Color.green * Color.gray;

    [SerializeField] private List<InventoryItemWithAmount> _inventoryItems;

    private bool _pressingDrop;
    private float _itemDropStackTimeLeft;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _gameManager = GameManager.Instance;
        _pickupPooler = _gameManager.GetPickupPooler;

        _playerComponents = playerComponents;

        _playerBody = _playerComponents.GetPlayerBody;
        _playerHealth = _playerComponents.GetPlayerHealth;
        _playerCombatSystem = _playerComponents.GetPlayerCombatSystem;
        _playerKnockout = _playerComponents.GetPlayerKnockout;
        _playerDeath = _playerComponents.GetPlayerDeath;
        _playerAcidation = _playerComponents.GetPlayerAcidation;

        _playerHealth.OnPlayerHealthChange += UpdateHealthUI;
        _playerCombatSystem.OnChargeChange += UpdateChargeUI;
        _playerKnockout.OnCanGetUp += PressSpaceToGetUp;
        _playerDeath.OnRespawnCountdown += UpdateRespawnTime;
        _playerAcidation.OnAcidationChange += UpdateAcidUI;

        _meleeSlot.GetButton.onClick.AddListener(MeleeSlotClick);
        _rangeSlot.GetButton.onClick.AddListener(RangeSlotClick);
        _staticSlot.GetButton.onClick.AddListener(StaticSlotClick);
        _dynamicSlot.GetButton.onClick.AddListener(DynamicSlotClick);
    }

    private void UpdateHealthUI(float amount)
    {
        _healthBar.fillAmount = amount;
        ColorBar(_healthBar,amount, _green * Color.gray,Color.yellow,Color.red);
    }

    private void UpdateChargeUI(float amount)
    {
        _chargeBar.fillAmount = amount;
        ColorBar(_chargeBar, amount, Color.red, Color.yellow, Color.white);
    }

    private void UpdateAcidUI(float amount)
    {
        _acidationBar.fillAmount = amount;
    }

    private void ColorBar(Image bar, float amount, Color full, Color mid, Color empty)
    {
        if (amount > 0.5f)
        {
            bar.color = Color.Lerp(mid, full, (amount - 0.5f) * 2);
        }
        else
        {
            bar.color = Color.Lerp(empty, mid, amount * 2);
        }
    }

    private void PressSpaceToGetUp(bool show)
    {
        _pressSpaceToGetUp.SetActive(show);
    }

    private void UpdateRespawnTime(int time)
    {
        switch (time)
        {
            case 0:
                _respawnCountDown.text = "Press [Jump] To Respawn";
                break;
            case -1:
                _respawnCountDown.text = string.Empty;
                break;
            default:
                _respawnCountDown.text = time.ToString();
                break;
        }

        PressSpaceToGetUp(false);
    }

    public void PressDropItem()
    {
        _pressingDrop = true;
        if (!_playerComponents.OnUpdate.GetInvocationList().Contains((Action)ReleaseStackTimer))
        {
            _playerComponents.OnUpdate += ReleaseStackTimer;
        }
        _itemDropStackTimeLeft = _itemDropStackTime;
    }

    public void ReleaseDropItem()
    {
        if (_pressingDrop)
        {
            DropOne();
        }
        _pressingDrop = false;
        _playerComponents.OnUpdate -= ReleaseStackTimer;
    }

    private void ReleaseStackTimer()
    {
        if (_itemDropStackTimeLeft > 0)
        {
            _itemDropStackTimeLeft -= Time.deltaTime;
        }
        else
        {
            DropStack();
            _playerComponents.OnUpdate -= ReleaseStackTimer;
        }   
    }

    private void DropOne()
    {
        if (_selected)
        {
            ItemUI selectedItem = _selected.GetComponent<ItemUI>();
            DropItem(selectedItem, 1, false); // Drop a single item
        }
    }

    private void DropStack()
    {
        if (_selected)
        {
            ItemUI selectedItem = _selected.GetComponent<ItemUI>();
            DropItem(selectedItem, selectedItem.GetAmount, true); // Drop the entire stack
        }
    }

    private void DropItem(ItemUI itemUI, int amount, bool isStack)
    {
        if (itemUI != null)
        {
            InventoryItem item = itemUI.GetInventoryItem;
            if (item.CanBeDropped)
            {
                if (isStack)
                {
                    RemoveItemStack(itemUI); // Remove the entire stack
                }
                else
                {
                    RemoveItem(itemUI); // Remove one item
                }

                // Spawn the item pickup from the pool
                ItemPickUp pickUp = (ItemPickUp)_pickupPooler.CreateOrSpawnFromPool(
                    item.GetPickUpTag,
                    _playerBody.position + Vector3.up * _itemDropHeight,
                    Quaternion.identity
                );

                // Set the item amount (1 for single item drop, or stack amount)
                pickUp.SetAmount(amount);

                // Apply item drop protection and add force
                pickUp.Spawn(_itemDropProtectionTime);
                pickUp.GetRigidbody.AddForce(
                    _playerBody.forward * _itemDropVelocity.x + Vector3.up * _itemDropVelocity.y
                );
            }
        }
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

    private void OpenInventory()
    {
        _inventoryUI.SetActive(true);
        _multiplayerEventSystem.SetSelectedGameObject(null);
        SetUpInventoryContent();
    }

    public void CloseInventory()
    {
        _inventoryUI.SetActive(false);
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
        ClearContent();

        bool first = true;
        foreach (InventoryItemWithAmount item in _inventoryItems) 
        {
            ItemUI i = Instantiate(_itemUItoSpawn, _inventoryContent.position, _inventoryContent.rotation, _inventoryContent);
            i.SetUpItemUI(item,this);
            if (first)
            {
                first=false;
                _multiplayerEventSystem.SetSelectedGameObject(i.gameObject);
                _selected = i.gameObject;
            }
        }

        FixNothingSelected();
    }

    private void FixNothingSelected()
    {
        if (_selected == null)
        {
            _multiplayerEventSystem.SetSelectedGameObject(_inventoryFirstSelected.gameObject);
            _selected = _inventoryFirstSelected.gameObject;
        }
    }

    private void ClearContent()
    {
        foreach (Transform child in _inventoryContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetMeleeWeapon(MeleeItem melee)
    {
        SetItem(melee.GetMoveSet, _playerCombatSystem.GetCurrentMeleeMoveSet,
                _playerCombatSystem.SetMeleeMoveSet, _meleeSlot, melee.GetSprite);
    }

    public void SetRangeWeapon(RangeItem range)
    {
        SetItem(range.GetMoveSet, _playerCombatSystem.GetCurrentRangeMoveSet,
                _playerCombatSystem.SetRangeMoveSet, _rangeSlot, range.GetSprite);
    }

    public void SetStaticUseable(UseableItem useable)
    {
        SetItem(useable.GetUseable, _playerCombatSystem.GetCurrentStaticUseable,
                _playerCombatSystem.SetStaticUseable, _staticSlot, useable.GetSprite);
    }

    public void SetDynamicUseable(UseableItem useable)
    {
        SetItem(useable.GetUseable, _playerCombatSystem.GetCurrentDynamicUseable,
                _playerCombatSystem.SetDynamicUseable, _dynamicSlot, useable.GetSprite);
    }

    public bool SpendConsumable(ConsumableItem consumable) 
    {
        return _playerCombatSystem.ConsumeConsumable(consumable.GetConsumable);
    }

    public void SetAcidCrosshair(bool b) => _acidCrosshair.SetActive(b);

    private void SetItem<T>(T newItem, T currentItem,
                            Action<T> setItemAction, ItemSlot slot, Sprite newSprite) where T : Component
    {
        if (newItem != null && newItem.name != currentItem?.name)
        {
            setItemAction(newItem);
            slot.SetImage(newSprite);
        }
        else
        {
            setItemAction(default);
            slot.SetImage(null);
        }
    }

    public void RemoveItem(ItemUI itemUI)
    {
        InventoryItemWithAmount itemToRemove = FindInventoryItem(itemUI);

        if (itemToRemove != null)
        {
            itemToRemove.Amount--;
            if (itemToRemove.Amount <= 0)
            {
                _inventoryItems.Remove(itemToRemove);
                SetUpInventoryContent();
            }
            else
            {
                itemUI.RemoveOneItem();
            }
        }
    }

    public void RemoveItemStack(ItemUI itemUI)
    {
        InventoryItemWithAmount itemToRemove = FindInventoryItem(itemUI);

        if (itemToRemove != null)
        {
            _inventoryItems.Remove(itemToRemove);
            SetUpInventoryContent();
        }
    }

    private InventoryItemWithAmount FindInventoryItem(ItemUI itemUI)
    {
        return _inventoryItems.Find(item => item.Item == itemUI.GetInventoryItem);
    }


    private void MeleeSlotClick()
    {
        _playerCombatSystem.SetMeleeMoveSet(null);
        _meleeSlot.SetImage(null);
    }
    private void RangeSlotClick()
    {
        _playerCombatSystem.SetRangeMoveSet(null);
        _rangeSlot.SetImage(null);
    }
    private void StaticSlotClick()
    {
        _playerCombatSystem.SetStaticUseable(null);
        _staticSlot.SetImage(null);
    }
    private void DynamicSlotClick()
    {
        _playerCombatSystem.SetDynamicUseable(null);
        _dynamicSlot.SetImage(null);
    }
    
    public void AddInventoryItem(InventoryItem item, int amount = 1)
    {
        foreach (InventoryItemWithAmount i in _inventoryItems)
        {
            if (i.Item == item)
            {
                i.Amount += amount;
                if (_inventoryUI.activeSelf)
                {
                    SetUpInventoryContent();
                }
                return;
            }
        }
        _inventoryItems.Add(new InventoryItemWithAmount() {Item = item, Amount=amount  });
        if (_inventoryUI.activeSelf)
        {
            SetUpInventoryContent();
        }
    }
}

[Serializable]
public class InventoryItemWithAmount
{
     public InventoryItem Item;
     public int Amount; 
}
