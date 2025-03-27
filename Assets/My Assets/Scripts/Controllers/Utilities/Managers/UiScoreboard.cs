using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement; // Added for scene transitions
using System.Collections; // Added for coroutines

public class UiScoreboard : MonoBehaviour
{
    [SerializeField] private GameObject _scoreboardPanel;
    [SerializeField] private PlayerScoreUI[] scores = new PlayerScoreUI[4];
    [SerializeField] private GameObject[] Players = new GameObject[4];

    [System.Serializable]
    class PlayerScoreUI
    {
        public GameObject ScoreGameObject;
        public TMP_Text Kills, Deaths, Time;
        public GameObject TimeIcon;
    }

    public class ScoreBoardResults
    {
        public PlayerScore[] scores;
        public enum WinningReason { MostKills, LeastDeaths, TimeSurvived }
        public WinningReason WinReason;
        public bool IsShowingTime;
        public class PlayerScore
        {
            public int Kills, Deaths;
            public float Time;
        }
    }

    public void SetScoreBoard(ScoreBoardResults score)
    {
        // Validate inputs
        if (score == null || score.scores == null || score.scores.Length != 4 ||
            scores.Any(s => s == null || s.ScoreGameObject == null || s.Kills == null || s.Deaths == null || s.Time == null || s.TimeIcon == null) ||
            Players.Any(p => p == null))
        {
            Debug.LogWarning("Scoreboard setup is invalid!");
            return;
        }

        // Step 1: Count active players and hide missing ones
        int activePlayerCount = 0;
        for (int i = 0; i < 4; i++)
        {
            if (score.scores[i] != null)
            {
                activePlayerCount++;
                Players[i].SetActive(true);
            }
            else
            {
                Players[i].SetActive(false);
            }
        }

        // Step 2: Hide lower PlayerScoreUI spots if there are fewer active players
        for (int i = 0; i < 4; i++)
        {
            scores[i].ScoreGameObject.SetActive(i < activePlayerCount);
        }

        // Step 3: Rank active players based on WinReason
        var rankedScores = score.scores
            .Select((playerScore, index) => new { PlayerScore = playerScore, OriginalIndex = index })
            .Where(x => x.PlayerScore != null)
            .ToList();

        // Debug the initial list
        Debug.Log("Before sorting:");
        foreach (var item in rankedScores)
        {
            Debug.Log($"Player{item.OriginalIndex + 1}: Kills={item.PlayerScore.Kills}, Deaths={item.PlayerScore.Deaths}, Time={item.PlayerScore.Time}");
        }

        // Sort based on WinReason
        rankedScores.Sort((a, b) =>
        {
            switch (score.WinReason)
            {
                case ScoreBoardResults.WinningReason.MostKills:
                    return b.PlayerScore.Kills.CompareTo(a.PlayerScore.Kills); // Descending
                case ScoreBoardResults.WinningReason.LeastDeaths:
                    return a.PlayerScore.Deaths.CompareTo(b.PlayerScore.Deaths); // Ascending
                case ScoreBoardResults.WinningReason.TimeSurvived:
                    return b.PlayerScore.Time.CompareTo(a.PlayerScore.Time); // Descending
                default:
                    return 0;
            }
        });

        // Debug the sorted list
        Debug.Log("After sorting:");
        foreach (var item in rankedScores)
        {
            Debug.Log($"Player{item.OriginalIndex + 1}: Kills={item.PlayerScore.Kills}, Deaths={item.PlayerScore.Deaths}, Time={item.PlayerScore.Time}");
        }

        // Step 4: Update UI for each rank position
        for (int rank = 0; rank < rankedScores.Count; rank++)
        {
            var rankedPlayer = rankedScores[rank];
            int playerIndex = rankedPlayer.OriginalIndex;
            var playerScore = rankedPlayer.PlayerScore;

            //Debug.Log($"Rank {rank + 1}: Player{playerIndex + 1} (Kills={playerScore.Kills})");

            // Position the player GameObject at the ranked spot
            Vector3 scorePosition = scores[rank].ScoreGameObject.transform.position;
            Vector3 playerPosition = Players[playerIndex].transform.position;
            Players[playerIndex].transform.position = new Vector3(playerPosition.x, scorePosition.y, playerPosition.z);

            // Set Kills and Deaths (always shown)
            scores[rank].Kills.text = playerScore.Kills.ToString();
            scores[rank].Deaths.text = playerScore.Deaths.ToString();

            // Handle Time display based on IsShowingTime
            if (score.IsShowingTime)
            {
                scores[rank].TimeIcon.SetActive(true);
                scores[rank].Time.text = FormatTime(playerScore.Time);
            }
            else
            {
                scores[rank].TimeIcon.SetActive(false);
                scores[rank].Time.text = "";
            }
        }

        // Step 5: Show the scoreboard
        _scoreboardPanel.SetActive(true);

        // Step 6: Calculate delay and transition to scene 0
        float baseDelay = 5f; // Base delay for 1 player
        float extraDelayPerPlayer = 3f; // Additional delay per extra player
        float totalDelay = baseDelay + (activePlayerCount - 1) * extraDelayPerPlayer;
        Debug.Log($"Active players: {activePlayerCount}, Total delay: {totalDelay} seconds");
        StartCoroutine(TransitionToSceneAfterDelay(totalDelay));
    }

    private IEnumerator TransitionToSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Transitioning to scene 0");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0); // Load scene 0 (main menu or lobby)
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100) % 100);
        return $"{minutes:D2}:{seconds:D2}.{milliseconds:D2}";
    }
}