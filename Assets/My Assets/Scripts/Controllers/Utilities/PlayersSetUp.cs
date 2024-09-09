using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayersSetUp : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _cinemachine;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    private UniversalAdditionalCameraData _cameraData;

    private void Start()
    {
        _gm = GameManager.Instance;
        _cinemachine.layer = LayerMask.NameToLayer("Player"+ _gm.PlayerCount);
        _camera.cullingMask = _gm.GetLayerMaskForCinemachine();
        ChangeFinsMaterial();
        FixCam();
        _gm.OnPlayerAmountChange += FixCam;
    }

    [ContextMenu("FixCamera")]
    public void FixCam()
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
        _gm.OnPlayerAmountChange -= FixCam;
    }
}
