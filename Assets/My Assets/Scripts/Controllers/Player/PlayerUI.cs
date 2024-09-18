using System;
using System.Collections.Generic;
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

    private PlayerHealth _playerHealth;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerKnockout _playerKnockout;
    private PlayerDeath _playerDeath;
    private PlayerAcidation _playerAcidation;

    private GameObject _selected;

    [SerializeField] private List<InventoryItem> _inventoryItems;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerHealth = playerComponents.GetPlayerHealth;
        _playerCombatSystem = playerComponents.GetPlayerCombatSystem;
        _playerKnockout = playerComponents.GetPlayerKnockout;
        _playerDeath = playerComponents.GetPlayerDeath;
        _playerAcidation = playerComponents.GetPlayerAcidation;

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
        ColorBar(_healthBar,amount,Color.green,Color.yellow,Color.red);
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
        foreach (InventoryItem item in _inventoryItems) 
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
}
