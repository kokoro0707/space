
using UnityEngine;

public enum ShipTeam
{
    Player,
    Enemy
}

public class ShipHealth : MonoBehaviour
{
    [Header("所属")]
    public ShipTeam team = ShipTeam.Enemy;

    [Header("機体HP")]
    [Min(1)]
    public int maxHP = 100;

    [Header("シールド")]
    [Min(0)]
    public int maxShield = 0;

    [Header("シールド回復設定")]
    public float shieldRegenDelay = 3f;
    public float shieldRegenPerSecond = 15f;

    public int CurrentHP { get; private set; }
    public int MaxHP => maxHP;

    public int CurrentShield { get; private set; }
    public int MaxShield => maxShield;

    public ShipTeam Team => team;

    private float lastDamageTime;
    private float regenAccumulator;

    private bool isDead;

    void Awake()
    {
        CurrentHP = maxHP;
        CurrentShield = maxShield;

        lastDamageTime = Time.time;
        regenAccumulator = 0f;
    }

    void Update()
    {
        if (isDead) return;

        RecoverShield();
    }

    // シールド回復
    void RecoverShield()
    {
        if (maxShield <= 0) return;

        if (CurrentShield >= maxShield)
        {
            regenAccumulator = 0f;
            return;
        }

        if (Time.time - lastDamageTime < shieldRegenDelay)
            return;

        regenAccumulator +=
            Mathf.Max(0f, shieldRegenPerSecond) * Time.deltaTime;

        int recoverAmount =
            Mathf.FloorToInt(regenAccumulator);

        if (recoverAmount <= 0) return;

        regenAccumulator -= recoverAmount;

        CurrentShield = Mathf.Min(
            maxShield,
            CurrentShield + recoverAmount
        );
    }

    // ダメージ処理
    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        lastDamageTime = Time.time;
        regenAccumulator = 0f;

        // シールドが優先してダメージを受ける
        if (CurrentShield > 0)
        {
            int shieldDamage =
                Mathf.Min(CurrentShield, damage);

            CurrentShield -= shieldDamage;
            damage -= shieldDamage;
        }

        // 余ったダメージを機体HPに適用
        if (damage > 0)
        {
            CurrentHP = Mathf.Max(
                0,
                CurrentHP - damage
            );
        }

        Debug.Log(
            $"{name} 被弾 " +
            $"HP:{CurrentHP}/{maxHP} " +
            $"Shield:{CurrentShield}/{maxShield}"
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
