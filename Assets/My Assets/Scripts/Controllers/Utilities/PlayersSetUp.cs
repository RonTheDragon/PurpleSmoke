using UnityEngine;

public class PlayersSetUp : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _cinemachine;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    private void Start()
    {
        _gm = GameManager.Instance;
        _cinemachine.layer = LayerMask.NameToLayer("Player"+ _gm.PlayerCount);
        _camera.cullingMask = _gm.GetLayerMaskForCinemachine();
        ChangeFinsMaterial();
    }

    private void ChangeFinsMaterial()
    {
        Material[] mats = _skinnedMeshRenderer.materials;
        mats[1] = _gm.GetColorForPlayer();
        _skinnedMeshRenderer.materials = mats;
    }
}
