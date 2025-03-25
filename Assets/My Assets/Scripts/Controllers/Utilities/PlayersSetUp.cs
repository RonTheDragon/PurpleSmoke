using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayersSetUp : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _cinemachine;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private PlayerTeleporter _teleporter;
    [SerializeField] private CombatRules _combatRules;
    [SerializeField] private PlayerDeath _playerDeath;
    private UniversalAdditionalCameraData _cameraData;
    [SerializeField] private float _spawnRadius = 3;
    private int _playerIndex;
    private GamemodeManager _ggm;

    private void Start()
    {
        _gm = GameManager.Instance;
        _ggm = _gm.GetGamemodeManager;

        // Validate references
        if (_combatRules == null)
        {
            Debug.LogError($"CombatRules is null for Player {gameObject.name}!");
        }
        if (_playerDeath == null)
        {
            Debug.LogError($"PlayerDeath is null for Player {gameObject.name}!");
        }
        if (_ggm == null)
        {
            Debug.LogError("GamemodeManager is null in PlayersSetUp!");
            return;
        }

        // Get the player index and actions from GamemodeManager
        var (playerIndex, onKillAction, onDeathAction) = _ggm.AddPlayer();
        _playerIndex = playerIndex;
        Debug.Log($"Player {_playerIndex + 1} assigned index {_playerIndex} from GamemodeManager");
        _combatRules.SetTeam(_ggm.IsPvp() ? "" : "Player");

        // Subscribe the actions to the events
        if (_combatRules != null)
        {
            _combatRules.OnKill += onKillAction;
            Debug.Log($"Subscribed OnKill for Player {_playerIndex + 1}");
        }
        if (_playerDeath != null)
        {
            _playerDeath.OnDeath += onDeathAction;
            Debug.Log($"Subscribed OnDeath for Player {_playerIndex + 1}");
        }

        ChangeFinsMaterial();
        MoveFromSpawn();
        _cinemachine.layer = LayerMask.NameToLayer("Player" + _gm.PlayerCount);
        _camera.cullingMask = _gm.GetLayerMaskForCinemachine();
        FixCanvas();
        _gm.OnPlayerAmountChange += FixCanvas;
    }

    public void MoveFromSpawn()
    {
        Vector3 newPos = transform.position + new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0, Random.Range(-_spawnRadius, _spawnRadius));
        _teleporter.Teleport(newPos);
    }

    public void FixCanvas()
    {
        if (_cameraData == null)
        {
            _cameraData = _camera.GetUniversalAdditionalCameraData();
        }
        foreach (Camera cam in _cameraData.cameraStack)
        {
            cam.rect = _camera.rect;
        }
    }

    private void ChangeFinsMaterial()
    {
        Material[] mats = _skinnedMeshRenderer.materials;
        mats[1] = _gm.GetColorForPlayer();
        _skinnedMeshRenderer.materials = mats;
    }

    private void OnDestroy()
    {
        _gm.OnPlayerAmountChange -= FixCanvas;
    }
}