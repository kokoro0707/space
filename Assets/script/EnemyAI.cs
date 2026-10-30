
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("ターゲット")]
    public Transform player;

    [Header("移動速度")]
    public float moveSpeed = 15f;
    public float rushSpeed = 25f;
    public float rotationSpeed = 5f;

    [Header("ランダム移動")]
    public float wanderRadius = 30f;
    public float wanderChangeInterval = 3f;

    private Vector3 wanderTarget;
    private float wanderTimer;

    [Header("戦闘設定")]
    public float detectDistance = 40f;
    public float attackDistance = 20f;
    public float fireInterval = 1f;
    public float fireAngle = 15f;
    public int bulletDamage = 15;

    [Header("攻撃中の移動")]
    [Range(0f, 1f)]
    public float attackMoveRatio = 0.35f;

    public GameObject bulletPrefab;
    public Transform muzzle;

    private float fireTimer;

    enum State
    {
        Rush,
        Wander,
        Chase,
        Attack
    }

    State state = State.Rush;

    void Start()
    {
        SetRandomWanderPoint();
    }

    // Spawnerからプレイヤーを設定
    public void SetPlayer(Transform target)
    {
        player = target;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (state == State.Rush)
        {
            if (distance < detectDistance)
            {
                state = State.Chase;
            }
        }
        else
        {
            if (distance < attackDistance)
                state = State.Attack;
            else if (distance < detectDistance)
                state = State.Chase;
            else
                state = State.Wander;
        }

        switch (state)
        {
            case State.Rush:
                RushToPlayer();
                break;

            case State.Wander:
                Wander();
                break;

            case State.Chase:
                ChasePlayer();
                break;

            case State.Attack:
                AttackPlayer();
                break;
        }
    }

    void RushToPlayer()
    {
        RotateTowards(player.position);

        transform.position +=
            transform.forward * rushSpeed * Time.deltaTime;
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderChangeInterval)
        {
            SetRandomWanderPoint();
        }

        MoveToTarget(wanderTarget);
    }

    void SetRandomWanderPoint()
    {
        wanderTimer = 0f;

        wanderTarget = transform.position +
            new Vector3(
                Random.Range(-wanderRadius, wanderRadius),
                Random.Range(-wanderRadius, wanderRadius),
                Random.Range(-wanderRadius, wanderRadius)
            );
    }

    void ChasePlayer()
    {
        MoveToTarget(player.position);
    }

    void AttackPlayer()
    {
        RotateTowards(player.position);

        // 攻撃中も前進する
        transform.position +=
            transform.forward *
            moveSpeed *
            attackMoveRatio *
            Time.deltaTime;

        fireTimer += Time.deltaTime;

        if (fireTimer < fireInterval) return;

        if (muzzle == null) return;

        Vector3 targetDirection =
            player.position - muzzle.position;

        float angle = Vector3.Angle(
            transform.forward,
            targetDirection
        );

        // 正面にターゲットがいる場合のみ攻撃
        if (angle <= fireAngle)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    void MoveToTarget(Vector3 target)
    {
        RotateTowards(target);

        transform.position +=
            transform.forward * moveSpeed * Time.deltaTime;
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    void Shoot()
    {
        if (bulletPrefab == null || muzzle == null)
            return;

        GameObject bulletObject = Instantiate(
            bulletPrefab,
            muzzle.position,
            muzzle.rotation
        );

        Bullet bullet =
            bulletObject.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(
                ShipTeam.Enemy,
                bulletDamage
            );
        }
    }
}
