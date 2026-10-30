
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("ƒvƒŒƒCƒ„[")]
    public ShipHealth playerHealth;

    private int totalEnemies = 10;
    private int defeatedEnemies;

    public bool IsFinished { get; private set; }

    private bool isClear;

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
        GUIStyle hudStyle = new GUIStyle(GUI.skin.label);
        hudStyle.fontSize = 24;

        if (playerHealth != null)
        {
            GUI.Label(
                new Rect(20, 20, 400, 35),
                $"HP: {playerHealth.CurrentHP} / {playerHealth.MaxHP}",
                hudStyle
            );

            GUI.Label(
                new Rect(20, 55, 400, 35),
                $"SHIELD: {playerHealth.CurrentShield} / {playerHealth.MaxShield}",
                hudStyle
            );

            PlayerController controller =
                playerHealth.GetComponent<PlayerController>();

            if (controller != null)
            {
                string dodgeText =
                    controller.DodgeCooldownRemaining <= 0f
                    ? "DODGE: READY"
                    : $"DODGE: {controller.DodgeCooldownRemaining:F1}s";

                GUI.Label(
                    new Rect(20, 125, 400, 35),
                    dodgeText,
                    hudStyle
                );
            }
        }

        GUI.Label(
            new Rect(20, 90, 400, 35),
            $"DESTROYED: {defeatedEnemies} / {totalEnemies}",
            hudStyle
        );

        if (!IsFinished) return;

        GUIStyle centerStyle =
            new GUIStyle(GUI.skin.label);

        centerStyle.alignment = TextAnchor.MiddleCenter;
        centerStyle.fontSize = 48;

        GUI.Label(
            new Rect(
                0, Screen.height / 2f - 80,
                Screen.width, 100
            ),
            isClear ? "MISSION CLEAR" : "GAME OVER",
            centerStyle
        );

        centerStyle.fontSize = 24;

        GUI.Label(
            new Rect(
                0, Screen.height / 2f + 30,
                Screen.width, 60
            ),
            "Press R to Restart",
            centerStyle
        );
    }
}
