using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private int _playerCount = 0;
    [SerializeField] private LayerMask[] _playerCinemachineCameraLayers = new LayerMask[4];
    [SerializeField] private Material[] _playerColors = new Material[4];

    public static GameManager Instance;

    [SerializeField] private ProjectilePooler _projectilePooler;
    [SerializeField] private VEPooler _visualEffectsPooler;
    [SerializeField] private PickupPooler _pickupPooler;
    [SerializeField] private EnemyPooler _enemyPooler;
    [SerializeField] private Color _meleeColor, _rangeColor, _staticColor, _dynamicColor, _consumableColor;
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private SOdeviceId _devicesIds;
    [SerializeField] private GamemodeManager _gamemodeManager;

    public enum ItemColor { Melee, Range, Static, Dynamic, Consumable }

    public Action OnPlayerAmountChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateAllPools();
        CreateAllConnectedPlayers();
    }

    private void CreateAllPools()
    {
        _projectilePooler.SpawnAllPools();
        _visualEffectsPooler.SpawnAllPools();
        _pickupPooler.SpawnAllPools();
        _enemyPooler.SpawnAllPools();
    }

    private void CreateAllConnectedPlayers()
    {
        foreach (int i in _devicesIds.DeviceIds)
        {
            InputDevice d = InputSystem.GetDeviceById(i);
            _playerInputManager.JoinPlayer(-1, -1, null, d);
        }
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        InputDevice device = input.devices[0]; // Get the device the player used
        //_playerCount++;
        OnPlayerAmountChange?.Invoke();
        // Check if the device is already in the list to avoid duplicates
        if (!_devicesIds.DeviceIds.Contains(device.deviceId))
        {
            _devicesIds.DeviceIds.Add(device.deviceId); // Store the device ID for future use           
        }
    }


    public void OnPlayerLeave(PlayerInput i)
    {
       // InputDevice device = i.devices[0];
       // _devicesIds.DeviceIds.Remove(device.deviceId);
        _playerCount--;
        OnPlayerAmountChange?.Invoke();
    }


    public int PlayerCount => _playerCount;

    public ProjectilePooler GetProjectilePooler => _projectilePooler;
    public VEPooler GetVEPooler => _visualEffectsPooler;
    public PickupPooler GetPickupPooler => _pickupPooler;
    public EnemyPooler GetEnemyPooler => _enemyPooler;  
    public GamemodeManager GetGamemodeManager => _gamemodeManager;

    public Color GetItemColor(ItemColor c)
    {
        switch (c)
        {
            case ItemColor.Melee:
                return _meleeColor;
            case ItemColor.Range:
                return _rangeColor;
            case ItemColor.Static:
                return _staticColor;
            case ItemColor.Dynamic:
                return _dynamicColor;
            case ItemColor.Consumable:
                return _consumableColor;
        }
        return Color.black;
    }

    public LayerMask GetLayerMaskForCinemachine()
    {
        return _playerCinemachineCameraLayers[_playerCount - 1];
    }

    public Material GetColorForPlayer()
    {
        _playerCount++;
        return _playerColors[_playerCount-1];
    }
}
