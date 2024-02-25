using UnityEngine;

public class GameManager : MonoBehaviour
{

    private int _playerCount;

    [SerializeField] private LayerMask[] _playerCinemachineCameraLayers = new LayerMask[4];

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
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
}
