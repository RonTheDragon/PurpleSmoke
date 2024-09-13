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

    public void OpenInventory()
    {
        _inventoryUI.SetActive(!_inventoryUI.activeSelf);
        _multiplayerEventSystem.SetSelectedGameObject(null);
        SetUpInventoryContent();
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
        if (melee.GetMoveSet.name != _playerCombatSystem.GetCurrentMeleeMoveSet.name)
        {
            _playerCombatSystem.SetMeleeMoveSet(melee.GetMoveSet);
            _meleeSlot.SetImage(melee.GetSprite);
        }
        else
        {
            _playerCombatSystem.SetMeleeMoveSet(null);
            _meleeSlot.SetImage(null);
        }
    }

    public void SetRangeWeapon(RangeItem ramge)
    {
        if (ramge.GetMoveSet.name != _playerCombatSystem.GetCurrentRangeMoveSet.name)
        {
            _playerCombatSystem.SetRangeMoveSet(ramge.GetMoveSet);
            _rangeSlot.SetImage(ramge.GetSprite);
        }
        else
        {
            _playerCombatSystem.SetRangeMoveSet(null);
            _rangeSlot.SetImage(null);
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
}
