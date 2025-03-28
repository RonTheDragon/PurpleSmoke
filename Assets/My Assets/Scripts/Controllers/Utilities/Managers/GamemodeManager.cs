using System;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeManager : MonoBehaviour
{
    [SerializeField] private SOgamemodeSelected _gamemodeSelected;
    [SerializeField] private List<PlayerScoreData> _players = new List<PlayerScoreData>();
    [SerializeField] private UiScoreboard _uiScoreboard;
    [SerializeField] private UiTimer _uiTimer;

    private List<Action> _onKillActions = new List<Action>();
    private List<Action> _onDeathActions = new List<Action>(); // Reverted to Action
    [SerializeField] private List<GameObject> _monstersSpawners = new List<GameObject>();
    [SerializeField] private Sprite _killsIcon, _livesIcon;

    private void Start()
    {
        // Validate essential components
        if (_gamemodeSelected == null)
        {
            Debug.LogError("SOgamemodeSelected is not assigned!");
            return;
        }
        if (_uiTimer == null)
        {
            Debug.LogError("UiTimer is not assigned!");
            return;
        }
        if (_uiScoreboard == null)
        {
            Debug.LogError("UiScoreboard is not assigned!");
            return;
        }

        if (_gamemodeSelected.Pvp == false)
        {
            foreach (GameObject a in _monstersSpawners)
                a.SetActive(true);
        }

        // Set up the timer based on the mode
        switch (_gamemodeSelected.Mode)
        {
            case 0: // Survival/Till Death 
            case 1: // Kill Race
            case 3: // Infinity
                _uiTimer.StartTimer(0); // Count up
                break;
            case 2: // Count Down
                _uiTimer.StartTimer(_gamemodeSelected.Amount); // Count down from Amount
                _uiTimer.OnTimesUp += EndGame; // End game when timer reaches 0
                break;
        }
    }

    public bool IsPvp()
    {
        return _gamemodeSelected.Pvp;
    }

    public (int playerIndex, Action onKillAction, Action onDeathAction) AddPlayer(PlayerComponentsRefrences refs)
    {
        PlayerScoreData playerData = new PlayerScoreData();
        int playerIndex = _players.Count;

        // Set initial lives for Mode 0
        if (_gamemodeSelected.Mode == 0)
        {
            playerData.Lives = _gamemodeSelected.Amount;
        }

        playerData.Refs = refs; // Store the refs
        _players.Add(playerData);

        // Set the initial mission counter on the player's UI
        PlayerUI playerUI = refs.GetPlayerUI;
        if (_gamemodeSelected.Mode == 0)
        {
            // Survival mode: Show lives
            playerUI.SetMissionCounter(playerData.Lives.ToString(), _livesIcon);
        }
        else
        {
            // Other modes: Show kills
            playerUI.SetMissionCounter(playerData.Kills.ToString(), _killsIcon);
        }

        // Define the kill action
        Action onKillAction = () =>
        {
            playerData.Kills++;
            // Update the mission counter for kills (all modes except Survival show kills)
            if (_gamemodeSelected.Mode != 0)
            {
                playerUI.SetMissionCounter(playerData.Kills.ToString(), _killsIcon);
            }

            // Mode 1: Check if player reached the kill goal
            if (_gamemodeSelected.Mode == 1 && playerData.Kills >= _gamemodeSelected.Amount)
            {
                Debug.Log($"Player {playerIndex + 1} reached kill goal of {_gamemodeSelected.Amount}! Ending game.");
                Invoke(nameof(EndGame), 0.1f);
            }
        };

        // Define the death action
        Action onDeathAction = () =>
        {
            playerData.Deaths++;
            if (_gamemodeSelected.Mode == 0) // Survival mode
            {
                playerData.Lives--;
                // Update the mission counter for lives
                playerUI.SetMissionCounter(playerData.Lives.ToString(), _livesIcon);
                if (playerData.Lives <= 0 && playerData.TimeOfDeath == 0)
                {
                    playerData.TimeOfDeath = _uiTimer.GetTime();
                    refs.GetPlayerDeath.SetOutOfLives(); // Restored SetOutOfLives
                    Debug.Log($"Player {playerIndex + 1} is out of lives! Survived for {playerData.TimeOfDeath} seconds");
                }
            }

            // Check for game end conditions
            CheckForGameEnd();
        };

        // Store the actions for manual invocation if needed
        _onKillActions.Add(onKillAction);
        _onDeathActions.Add(onDeathAction);

        // Return the player index and the actions to subscribe to
        return (playerIndex, onKillAction, onDeathAction);
    }

    public void OnPlayerKill(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < _onKillActions.Count)
        {
            _onKillActions[playerIndex]?.Invoke();
        }
    }

    public void OnPlayerDeath(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < _onDeathActions.Count)
        {
            _onDeathActions[playerIndex]?.Invoke();
        }
    }

    private void CheckForGameEnd()
    {
        if (_gamemodeSelected.Mode == 0) // Survival/Till Death
        {
            if (IsPvp())
            {
                // PvP: End when only 1 player has lives left
                int playersWithLives = 0;
                int lastPlayerIndex = -1;
                for (int i = 0; i < _players.Count; i++)
                {
                    if (_players[i].Lives > 0)
                    {
                        playersWithLives++;
                        lastPlayerIndex = i;
                    }
                }

                if (playersWithLives <= 1)
                {
                    // Add 1 second to the last player's time
                    if (lastPlayerIndex >= 0 && _players[lastPlayerIndex].TimeOfDeath == 0)
                    {
                        _players[lastPlayerIndex].TimeOfDeath = _uiTimer.GetTime() + 1f;
                    }
                    EndGame();
                }
            }
            else
            {
                // PvE: End when all players are out of lives
                bool allOutOfLives = true;
                for (int i = 0; i < _players.Count; i++)
                {
                    if (_players[i].Lives > 0)
                    {
                        allOutOfLives = false;
                        break;
                    }
                }

                if (allOutOfLives)
                {
                    EndGame();
                }
            }
        }
    }

    private void EndGame()
    {
        _uiTimer.StopTimer();

        // Clear mission counters for all players
        foreach (var player in _players)
        {
            player.Refs.GetPlayerUI.SetMissionCounter("");
        }

        UiScoreboard.ScoreBoardResults results = new UiScoreboard.ScoreBoardResults
        {
            scores = new UiScoreboard.ScoreBoardResults.PlayerScore[4],
            WinReason = GetWinningReason(),
            IsShowingTime = ShouldShowTime()
        };

        for (int i = 0; i < 4; i++)
        {
            if (i < _players.Count && _players[i] != null)
            {
                results.scores[i] = new UiScoreboard.ScoreBoardResults.PlayerScore
                {
                    Kills = _players[i].Kills,
                    Deaths = _players[i].Deaths,
                    Time = _players[i].TimeOfDeath > 0 ? _players[i].TimeOfDeath : _uiTimer.GetTime()
                };
            }
            else
            {
                results.scores[i] = null;
            }
        }

        _uiScoreboard.SetScoreBoard(results);
    }

    private UiScoreboard.ScoreBoardResults.WinningReason GetWinningReason()
    {
        switch (_gamemodeSelected.Mode)
        {
            case 0: return UiScoreboard.ScoreBoardResults.WinningReason.TimeSurvived; // Survival: Rank by time
            case 1: return UiScoreboard.ScoreBoardResults.WinningReason.MostKills; // Kill Race: Rank by kills
            case 2: return UiScoreboard.ScoreBoardResults.WinningReason.MostKills; // Count Down: Rank by kills
            case 3: return UiScoreboard.ScoreBoardResults.WinningReason.MostKills; // Infinity: No end, but default to kills
            default: return UiScoreboard.ScoreBoardResults.WinningReason.MostKills;
        }
    }

    private bool ShouldShowTime()
    {
        return _gamemodeSelected.Mode == 0; // Only show time in Survival mode
    }

    class PlayerScoreData
    {
        public int Kills;
        public int Deaths;
        public float TimeOfDeath;
        public int Lives; // Used for Mode 0
        public PlayerComponentsRefrences Refs;
    }

    public bool IsForceRespawn => _gamemodeSelected.Mode == 0;
}