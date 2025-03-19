using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private SOdeviceId _devicesIds;
    [SerializeField] private GameObject[] _players;
    private int _playerCount = 0;
    void Awake()
    {
        _devicesIds.DeviceIds.Clear();
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        InputDevice device = input.devices[0]; // Get the device the player used
        _devicesIds.DeviceIds.Add(device.deviceId); // Store the device ID for future use
        _players[_playerCount].SetActive(true);
        _playerCount++;
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
             Application.Quit();
#endif
    }
}
