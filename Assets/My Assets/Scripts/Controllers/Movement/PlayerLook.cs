using UnityEngine;

public class PlayerLook : PlayerComponent
{
    [SerializeField] private Vector2 _mouseSpeed;
    [SerializeField] private Vector2 _mouseClampY = new Vector2(-70, 70);
    [SerializeField] private float _smoothTime = 0.3f; // Smoothing time

    private Transform _cameraHolder;
    private Vector2 _currentRotation = Vector2.zero;

    public override void SetPlayerComponents(PlayerComponentsRefrences controller)
    {
        base.SetPlayerComponents(controller);
        _cameraHolder = controller.GetCameraHolder();
    }

    public void Look(Vector2 direction)
    {
        Vector2 calculatedDirection = direction * Time.deltaTime;
        UpdateRotation(calculatedDirection);
        ClampRotation();
        ApplySmoothRotation();
    }

    private void UpdateRotation(Vector2 direction)
    {
        _currentRotation.x -= direction.y * _mouseSpeed.y;
        _currentRotation.y += direction.x * _mouseSpeed.x;
    }

    private void ClampRotation()
    {
        _currentRotation.x = Mathf.Clamp(_currentRotation.x, _mouseClampY.x, _mouseClampY.y);
    }

    private void ApplySmoothRotation()
    {
        // Define a target rotation based on _currentRotation
        Quaternion targetRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);

        // Smoothly move the camera's rotation towards the target rotation
        _cameraHolder.rotation = Quaternion.Slerp(_cameraHolder.rotation, targetRotation, _smoothTime);
    }
}
