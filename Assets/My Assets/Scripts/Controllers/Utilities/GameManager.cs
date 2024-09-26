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

    public LayerMask GetLayerMaskForCinemachine()
    {
        return _playerCinemachineCameraLayers[_playerCount-1];
    }

    public Material GetColorForPlayer()
    {
        return _playerColors[_playerCount - 1];
    }
}
