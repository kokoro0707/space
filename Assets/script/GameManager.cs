
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("ÉvÉåÉCÉÑÅ[HP")]
    public ShipHealth playerHealth;

    private int totalEnemies = 10;
    private int defeatedEnemies = 0;

    public bool IsFinished { get; private set; }

    private bool isClear = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ConfigureMission(int enemyCount)
    {
        totalEnemies = Mathf.Max(1, enemyCount);
    }

    public void EnemyDefeated()
    {
        if (IsFinished) return;

        defeatedEnemies++;

        if (defeatedEnemies >= totalEnemies)
        {
            FinishGame(true);
        }
    }

    public void PlayerDefeated()
    {
        if (IsFinished) return;

        FinishGame(false);
    }

    void FinishGame(bool clear)
    {
        IsFinished = true;
        isClear = clear;

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (IsFinished && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 24;

        if (playerHealth != null)
        {
            GUI.Label(
                new Rect(20, 20, 400, 40),
                $"HP: {playerHealth.CurrentHP} / {playerHealth.MaxHP}"
            );
        }

        GUI.Label(
            new Rect(20, 60, 400, 40),
            $"åÇíƒêî: {defeatedEnemies} / {totalEnemies}"
        );

        if (!IsFinished) return;

        GUIStyle centerStyle =
            new GUIStyle(GUI.skin.label);

        centerStyle.alignment =
            TextAnchor.MiddleCenter;

        centerStyle.fontSize = 48;

        string message =
            isClear ? "MISSION CLEAR" : "GAME OVER";

        GUI.Label(
            new Rect(
                0,
                Screen.height / 2f - 80,
                Screen.width,
                100
            ),
            message,
            centerStyle
        );

        centerStyle.fontSize = 24;

        GUI.Label(
            new Rect(
                0,
                Screen.height / 2f + 30,
                Screen.width,
                60
            ),
            "Press R to Restart",
            centerStyle
        );
    }
}
