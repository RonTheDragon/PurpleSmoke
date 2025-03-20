using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private SOdeviceId _devicesIds;
    [SerializeField] private GameObject[] _players; // UI slots, not actual players
    private int _playerCount = 0;
    private int _currentGameMode = 0;
    [SerializeField] private List<Gamemode> gamemodes;
    [SerializeField] private TMP_Text _gamemode_text, _mode_text, _amountOf_text, _amountText, _playButtonText;
    [SerializeField] private GameObject _amountPanel, _removeAllPlayersButton;
    [SerializeField] private Button PlayButton;
    private List<PlayerInput> _activePlayers = new List<PlayerInput>(); // Track actual PlayerInput instances

    void Awake()
    {
        _devicesIds.DeviceIds.Clear();
        _activePlayers.Clear();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        if (_playerCount < _players.Length)
        {
            InputDevice device = input.devices[0];
            _devicesIds.DeviceIds.Add(device.deviceId);
            _players[_playerCount].SetActive(true); // Activate UI slot
            _activePlayers.Add(input); // Store the PlayerInput instance
            _playerCount++;
            UpdatePlayButton();
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void ChangeGameMode()
    {
        if (gamemodes.Count == 0) return;

        _currentGameMode = (_currentGameMode + 1) % gamemodes.Count;
        UpdateUI();
    }

    public void ChangeMode()
    {
        if (gamemodes.Count == 0 || gamemodes[_currentGameMode].modes.Count == 0) return;

        Gamemode g = gamemodes[_currentGameMode];
        g.currentMode = (g.currentMode + 1) % g.modes.Count;
        UpdateUI();
    }

    public void ChangeAmount(bool increase)
    {
        if (gamemodes.Count == 0 || gamemodes[_currentGameMode].modes.Count == 0) return;

        Gamemode g = gamemodes[_currentGameMode];
        Mode mode = g.modes[g.currentMode];
        if (mode.Amount.Count == 0) return;

        mode.CurrentAmountSelected += increase ? 1 : -1;
        mode.CurrentAmountSelected = Mathf.Clamp(mode.CurrentAmountSelected, 0, mode.Amount.Count - 1);
        UpdateAmountText();
    }

    public void RemoveAllPlayers()
    {
        // Keep the first player, remove the rest
        for (int i = _activePlayers.Count - 1; i > 0; i--) // Start from end, keep index 0
        {
            PlayerInput player = _activePlayers[i];
            _activePlayers.RemoveAt(i);
            Destroy(player.gameObject); // Destroy the PlayerInput GameObject
        }

        // Set player count to 1
        _playerCount = 1;

        // Keep only the first device ID
        if (_devicesIds.DeviceIds.Count > 1)
        {
            int firstDeviceId = _devicesIds.DeviceIds[0];
            _devicesIds.DeviceIds.Clear();
            _devicesIds.DeviceIds.Add(firstDeviceId);
        }

        // Refresh UI
        UpdateUI();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void UpdateUI()
    {
        if (gamemodes.Count == 0) return;

        Gamemode g = gamemodes[_currentGameMode];

        if (_gamemode_text != null)
            _gamemode_text.text = g.name;
        else
            Debug.LogWarning("Gamemode text is missing!");

        if (g.modes.Count > 0)
        {
            Mode mode = g.modes[g.currentMode];

            if (_mode_text != null)
                _mode_text.text = mode.name;
            else
                Debug.LogWarning("Mode text is missing!");

            if (_amountOf_text != null)
                _amountOf_text.text = mode.AmountOf;
            else
                Debug.LogWarning("AmountOf text is missing!");

            if (_amountPanel != null)
                _amountPanel.SetActive(!string.IsNullOrEmpty(mode.AmountOf));
            else
                Debug.LogWarning("Amount panel is missing!");

            UpdateAmountText();
        }

        // Sync UI player slots to _playerCount
        for (int i = 0; i < _players.Length; i++)
        {
            _players[i].SetActive(i < _playerCount);
        }

        UpdatePlayButton();
    }

    private void UpdateAmountText()
    {
        if (gamemodes.Count == 0 || gamemodes[_currentGameMode].modes.Count == 0) return;

        Mode mode = gamemodes[_currentGameMode].modes[gamemodes[_currentGameMode].currentMode];
        if (mode.Amount.Count > 0 && _amountText != null)
        {
            mode.CurrentAmountSelected = Mathf.Clamp(mode.CurrentAmountSelected, 0, mode.Amount.Count - 1);
            _amountText.text = mode.Amount[mode.CurrentAmountSelected].ToString();
        }
        else if (_amountText == null)
        {
            Debug.LogWarning("Amount text is missing!");
        }
    }

    private void UpdatePlayButton()
    {
        if (PlayButton == null || _playButtonText == null)
        {
            Debug.LogWarning("PlayButton or PlayButtonText is missing!");
            return;
        }

        if (_removeAllPlayersButton == null)
        {
            Debug.LogWarning("RemoveAllPlayersButton is missing!");
            return;
        }

        Gamemode g = gamemodes[_currentGameMode];
        if (g.name == "PVP" && _playerCount <= 1)
        {
            PlayButton.interactable = false;
            _playButtonText.text = "Not enough Players";
        }
        else
        {
            PlayButton.interactable = true;
            _playButtonText.text = "Play";
        }

        // Show/hide RemoveAllPlayersButton based on player count
        _removeAllPlayersButton.SetActive(_playerCount > 1);
    }

    [System.Serializable]
    public class Gamemode
    {
        public string name;
        public List<Mode> modes;
        [HideInInspector] public int currentMode;
    }

    [System.Serializable]
    public class Mode
    {
        public string name;
        public string AmountOf;
        public List<int> Amount;
        public int CurrentAmountSelected;
    }
}