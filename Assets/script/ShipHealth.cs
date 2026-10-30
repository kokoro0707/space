
using UnityEngine;

public enum ShipTeam
{
    Player,
    Enemy
}

public class ShipHealth : MonoBehaviour
{
    [Header("Š‘®")]
    public ShipTeam team = ShipTeam.Enemy;

    [Header("HP")]
    [Min(1)]
    public int maxHP = 100;

    public int CurrentHP { get; private set; }
    public int MaxHP => maxHP;
    public ShipTeam Team => team;

    private bool isDead = false;

    void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        CurrentHP = Mathf.Max(0, CurrentHP - damage);

        Debug.Log(
            $"{gameObject.name} ”í’eI HP: {CurrentHP}/{maxHP}"
        );

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (team == ShipTeam.Player)
        {
            GameManager.Instance?.PlayerDefeated();
        }
        else
        {
            GameManager.Instance?.EnemyDefeated();
            Destroy(gameObject);
        }
    }
}
