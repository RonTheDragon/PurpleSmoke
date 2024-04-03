using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _playerCount;
    [SerializeField] private LayerMask[] _playerCinemachineCameraLayers = new LayerMask[4];
    [SerializeField] private Material[] _playerColors = new Material[4];

    public static GameManager Instance;

    public ProjectilePooler ProjectilePooler;

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
        ProjectilePooler.CreateAllPools();
    }

    public void OnPlayerJoined()
    {
        _playerCount++;
    }

    public int PlayerCount => _playerCount;

    public LayerMask GetLayerMaskForCinemachine()
    {
        return _playerCinemachineCameraLayers[_playerCount-1];
    }

    public Material GetColorForPlayer()
    {
        return _playerColors[_playerCount - 1];
    }
}
