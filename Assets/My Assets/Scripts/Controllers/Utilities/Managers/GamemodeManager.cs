using System;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeManager : MonoBehaviour
{
    [SerializeField] private SOgamemodeSelected _gamemodeSelected;
    [SerializeField] private List<PlayerScoreData> _players = new List<PlayerScoreData>();
    [SerializeField] private UiScoreboard uiScoreboard;
    [SerializeField] private UiTimer UiTimer;

    private List<Action> _onKillActions = new List<Action>();
    private List<Action> _onDeathActions = new List<Action>();
    [SerializeField] private List<GameObject> Monsters = new List<GameObject>();

    private void Start()
    {
        // Validate essential components
        if (_gamemodeSelected == null)
        {
            Debug.LogError("SOgamemodeSelected is not assigned!");
            return;
        }
        if (UiTimer == null)
        {
            Debug.LogError("UiTimer is not assigned!");
            return;
        }
        if (uiScoreboard == null)
        {
            Debug.LogError("UiScoreboard is not assigned!");
            return;
        }

        Debug.Log($"Starting GamemodeManager: Mode={_gamemodeSelected.Mode}, Pvp={_gamemodeSelected.Pvp}, Amount={_gamemodeSelected.Amount}");

        if (_gamemodeSelected.Pvp == false) { foreach (GameObject a in Monsters) a.SetActive(true); }


        // Set up the timer based on the mode
        switch (_gamemodeSelected.Mode)
        {
            case 0: // Survival/Till Death
            case 1: // Kill Race
            case 3: // Infinity
                Debug.Log("Starting timer in count-up mode");
                UiTimer.StartTimer(0); // Count up
                break;
            case 2: // Count Down
                Debug.Log($"Starting timer in count-down mode with {_gamemodeSelected.Amount} seconds");
                UiTimer.StartTimer(_gamemodeSelected.Amount); // Count down from Amount
                UiTimer.OnTimesUp += EndGame; // End game when timer reaches 0
                break;
        }
    }

    public bool IsPvp()
    {
        return _gamemodeSelected.Pvp;
    }

    public (int playerIndex, Action onKillAction, Action onDeathAction) AddPlayer()
    {
        PlayerScoreData playerData = new PlayerScoreData();
        int playerIndex = _players.Count;

        // Set initial lives for Mode 0
        if (_gamemodeSelected.Mode == 0)
        {
            playerData.Lives = _gamemodeSelected.Amount;
            Debug.Log($"Player {playerIndex + 1} starts with {playerData.Lives} lives");
        }

        _players.Add(playerData);
        Debug.Log($"Added Player {playerIndex + 1} to GamemodeManager. Total players: {_players.Count}");

        // Define the kill action
        Action onKillAction = () =>
        {
            playerData.Kills++;
            Debug.Log($"Player {playerIndex + 1} now has {playerData.Kills} kills");

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
            Debug.Log($"Player {playerIndex + 1} died");
            playerData.Deaths++;
            Debug.Log($"Player {playerIndex + 1} now has {playerData.Deaths} deaths");
            if (_gamemodeSelected.Mode == 0) // Survival mode
            {
                playerData.Lives--;
                Debug.Log($"Player {playerIndex + 1} has {playerData.Lives} lives left");
                if (playerData.Lives <= 0 && playerData.TimeOfDeath == 0)
                {
                    playerData.TimeOfDeath = UiTimer.GetTime();
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
        Debug.Log($"OnPlayerKill called for Player {playerIndex + 1}. Total kill actions: {_onKillActions.Count}");
        if (playerIndex >= 0 && playerIndex < _onKillActions.Count)
        {
            _onKillActions[playerIndex]?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Invalid playerIndex {playerIndex} for OnPlayerKill. Valid range: 0 to {_onKillActions.Count - 1}");
        }
    }

    public void OnPlayerDeath(int playerIndex)
    {
        Debug.Log($"OnPlayerDeath called for Player {playerIndex + 1}. Total death actions: {_onDeathActions.Count}");
        if (playerIndex >= 0 && playerIndex < _onDeathActions.Count)
        {
            _onDeathActions[playerIndex]?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Invalid playerIndex {playerIndex} for OnPlayerDeath. Valid range: 0 to {_onDeathActions.Count - 1}");
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

                Debug.Log($"Players with lives remaining: {playersWithLives}");
                if (playersWithLives <= 1)
                {
                    // Add 1 second to the last player's time
                    if (lastPlayerIndex >= 0 && _players[lastPlayerIndex].TimeOfDeath == 0)
                    {
                        _players[lastPlayerIndex].TimeOfDeath = UiTimer.GetTime() + 1f;
                        Debug.Log($"Last player (Player {lastPlayerIndex + 1}) survived for {_players[lastPlayerIndex].TimeOfDeath} seconds");
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
                    Debug.Log("All players are out of lives in PvE!");
                    EndGame();
                }
            }
        }
    }

    private void EndGame()
    {
        Debug.Log("Ending game");
        UiTimer.StopTimer();

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
                    Time = _players[i].TimeOfDeath > 0 ? _players[i].TimeOfDeath : UiTimer.GetTime()
                };
                Debug.Log($"Player {i + 1} final stats: Kills={_players[i].Kills}, Deaths={_players[i].Deaths}, Time={results.scores[i].Time}");
            }
            else
            {
                results.scores[i] = null;
            }
        }

        uiScoreboard.SetScoreBoard(results);
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
        public int Lives; // Added for Mode 0
    }
}