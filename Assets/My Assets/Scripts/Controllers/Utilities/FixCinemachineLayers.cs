using UnityEngine;

public class FixCinemachineLayers : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _cinemachine;

    private void Start()
    {
        _gm = GameManager.Instance;
        _cinemachine.layer = LayerMask.NameToLayer("Player"+ _gm.PlayerCount);
        _camera.cullingMask = _gm.GetLayerMaskForCinemachine();
    }
}
