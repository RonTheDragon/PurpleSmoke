using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _playerCount;
    [SerializeField] private LayerMask[] _playerCinemachineCameraLayers = new LayerMask[4];
    [SerializeField] private Material[] _playerColors = new Material[4];

    public static GameManager Instance;

    [SerializeField] private ProjectilePooler _projectilePooler;
    [SerializeField] private VEPooler _visualEffectsPooler;
    [SerializeField] private PickupPooler _pickupPooler;
    [SerializeField] private Color _meleeColor, _rangeColor, _staticColor,_dynamicColor, _consumableColor;
    public enum ItemColor { Melee, Range, Static , Dynamic, Consumable }

    public Action OnPlayerAmountChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateAllPools();
    }

    private void CreateAllPools()
    {
        _projectilePooler.SpawnAllPools();
        _visualEffectsPooler.SpawnAllPools();
        _pickupPooler.SpawnAllPools();
    }

    public void OnPlayerJoined()
    {
        _playerCount++;
        OnPlayerAmountChange?.Invoke();
    }

    public void OnPlayerLeave()
    {
        _playerCount--;
        OnPlayerAmountChange?.Invoke();
    }

    public int PlayerCount => _playerCount;

    public ProjectilePooler GetProjectilePooler => _projectilePooler;
    public VEPooler GetVEPooler => _visualEffectsPooler;
    public PickupPooler GetPickupPooler => _pickupPooler;

    public Color GetItemColor(ItemColor c)
    {
        switch (c)
        {
            case ItemColor.Melee:
                return _meleeColor;
                case ItemColor.Range: return _rangeColor;
                case ItemColor.Static: return _staticColor;
                case ItemColor.Dynamic: return _dynamicColor;
                case ItemColor.Consumable: return _consumableColor;
        }
        return Color.black;
    }

    public LayerMask GetLayerMaskForCinemachine()
    {
        return _playerCinemachineCameraLayers[_playerCount-1];
    }

    public Material GetColorForPlayer()
    {
        return _playerColors[_playerCount - 1];
    }
}
