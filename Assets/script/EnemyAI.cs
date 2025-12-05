using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("ターゲット")]
    public Transform player;

    [Header("移動速度")]
    public float moveSpeed = 15f;
    public float rushSpeed = 25f;        // 追加：突進速度
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
    public GameObject bulletPrefab;
    public Transform muzzle;

    private float fireTimer = 0f;

    enum State { Rush, Wander, Chase, Attack }
    State state = State.Rush;

    void Start()
    {
        SetRandomWanderPoint();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Rush → Chase or Wander or Attack に移行
        if (state == State.Rush)
        {
            if (distance < detectDistance)
                state = State.Chase;
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

    //===============================
    // ① スポーン直後：プレイヤーに突進
    //===============================
    void RushToPlayer()
    {
        RotateTowards(player.position);
        transform.position += transform.forward * rushSpeed * Time.deltaTime;
    }

    //===============================
    // ② ランダム移動
    //===============================
    void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer > wanderChangeInterval)
        {
            SetRandomWanderPoint();
        }

        MoveToTarget(wanderTarget);
    }

    void SetRandomWanderPoint()
    {
        wanderTimer = 0;
        wanderTarget = transform.position +
            new Vector3(
                Random.Range(-wanderRadius, wanderRadius),
                Random.Range(-wanderRadius, wanderRadius),
                Random.Range(-wanderRadius, wanderRadius)
            );
    }

    //===============================
    // ③ プレイヤー追尾
    //===============================
    void ChasePlayer()
    {
        MoveToTarget(player.position);
    }

    //===============================
    // ④ 攻撃
    //===============================
    void AttackPlayer()
    {
        RotateTowards(player.position);

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0;
            Shoot();
        }
    }

    //===============================
    // 共通処理
    //===============================
    void MoveToTarget(Vector3 target)
    {
        RotateTowards(target);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            Time.deltaTime * rotationSpeed
        );
    }

    void Shoot()
    {
        if (bulletPrefab == null || muzzle == null) return;

        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
    }
}