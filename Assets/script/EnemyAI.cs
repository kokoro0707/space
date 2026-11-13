
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("ƒ^[ƒQƒbƒg")]
    public Transform player;

    [Header("Šî–{ˆÚ“®")]
    public float moveSpeed = 15f;
    public float rushSpeed = 25f;
    public float rotationSpeed = 5f;

    [Header("õ“GEUŒ‚‹——£")]
    public float detectDistance = 70f;
    public float attackDistance = 35f;

    [Header("ù‰ñUŒ‚")]
    public float orbitSpeed = 14f;
    public float preferredOrbitDistance = 25f;
    public float orbitCorrectionStrength = 1.2f;

    [Header("ù‰ñŠÔ")]
    public float orbitDuration = 3f;
    public float orbitExitMargin = 5f;

    [Header("ã‰ºˆÚ“®")]
    public float verticalOrbitAmount = 0.3f;
    public float verticalOrbitFrequency = 1.5f;

    [Header("’Êí‚Ì—£’E")]
    public float breakAwayDuration = 2f;
    public float breakAwaySpeed = 28f;
    public float minChaseTime = 0.8f;

    [Header("‹ß‹——£‚Ì‹Ù‹}—£’E")]
    public float retreatDistance = 12f;
    public float retreatExitDistance = 20f;
    public float retreatSpeed = 23f;

    [Header("ƒ‰ƒ“ƒ_ƒ€ˆÚ“®")]
    public float wanderRadius = 30f;
    public float wanderChangeInterval = 3f;

    [Header("ËŒ‚")]
    public GameObject bulletPrefab;
    public Transform muzzle;
    public float fireInterval = 1f;
    public int bulletDamage = 15;
    public float fireAngle = 20f;
    public bool shootWhileRetreating = true;

    private float fireTimer;
    private float stateTimer;
    private float wanderTimer;

    private Vector3 wanderTarget;
    private Vector3 breakAwayDirection;

    private int orbitDirection;
    private float orbitPhase;

    private enum State
    {
        Rush,
        Wander,
        Chase,
        OrbitAttack,
        Retreat,
        BreakAway
    }

    [SerializeField]
    private State state = State.Rush;

    void Start()
    {
        state = State.Rush;
        stateTimer = 0f;

        orbitDirection =
            Random.value < 0.5f ? -1 : 1;

        orbitPhase =
            Random.Range(0f, Mathf.PI * 2f);

        fireTimer = Random.Range(
            0f,
            Mathf.Max(0.01f, fireInterval)
        );

        SetRandomWanderPoint();
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    void Update()
    {
        if (player == null) return;

        stateTimer += Time.deltaTime;
        fireTimer += Time.deltaTime;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        UpdateState(distance);

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

            case State.OrbitAttack:
                OrbitAttack(distance);
                break;

            case State.Retreat:
                RetreatFromPlayer();
                break;

            case State.BreakAway:
                BreakAway();
                break;
        }

        if (state == State.OrbitAttack ||
            (state == State.Retreat &&
             shootWhileRetreating))
        {
            TryShoot();
        }
    }

    // =====================================
    // ó‘Ô‘JˆÚ
    // =====================================
    void UpdateState(float distance)
    {
        switch (state)
        {
            case State.Rush:
                if (distance <= detectDistance)
                    ChangeState(State.Chase);
                return;

            case State.BreakAway:
                // w’èŠÔ‚ªŒo‰ß‚·‚é‚Ü‚Å—£’E‚ğ‘±‚¯‚é
                if (stateTimer >= breakAwayDuration)
                    ChangeState(State.Chase);
                return;

            case State.Retreat:
                // ‹ß‚·‚¬‚éê‡‚Í\•ª‚É‹——£‚ğæ‚é
                if (distance >= retreatExitDistance)
                    ChangeState(State.BreakAway);
                return;

            case State.OrbitAttack:
                if (distance <= retreatDistance)
                {
                    ChangeState(State.Retreat);
                }
                else if (distance >
                    attackDistance + orbitExitMargin)
                {
                    ChangeState(State.Chase);
                }
                else if (stateTimer >= orbitDuration)
                {
                    // ù‰ñŠÔ‚ªI‚í‚Á‚½‚ç—£’E
                    ChangeState(State.BreakAway);
                }
                return;
        }

        // Wander‚©‚ç•œ‹A
        if (state == State.Wander &&
            distance <= detectDistance)
        {
            ChangeState(State.Chase);
            return;
        }

        if (distance <= retreatDistance)
        {
            ChangeState(State.Retreat);
        }
        else if (distance <= attackDistance &&
                 stateTimer >= minChaseTime)
        {
            ChangeState(State.OrbitAttack);
        }
        else if (distance <= detectDistance)
        {
            ChangeState(State.Chase);
        }
        else
        {
            ChangeState(State.Wander);
        }
    }

    // =====================================
    // ó‘Ô•ÏX‚Ì‰Šú‰»
    // =====================================
    void ChangeState(State nextState)
    {
        if (state == nextState) return;

        state = nextState;
        stateTimer = 0f;

        if (state == State.Wander)
        {
            SetRandomWanderPoint();
        }

        if (state == State.BreakAway)
        {
            SetBreakAwayDirection();
        }
    }

    // =====================================
    // ‹}Ú‹ß
    // =====================================
    void RushToPlayer()
    {
        RotateTowards(player.position);

        transform.position +=
            transform.forward *
            rushSpeed *
            Time.deltaTime;
    }

    // =====================================
    // ’ÇÕ
    // =====================================
    void ChasePlayer()
    {
        RotateTowards(player.position);

        transform.position +=
            transform.forward *
            moveSpeed *
            Time.deltaTime;
    }

    // =====================================
    // ù‰ñUŒ‚
    // =====================================
    void OrbitAttack(float distance)
    {
        RotateTowards(player.position);

        Vector3 toPlayer =
            (player.position - transform.position).normalized;

        Vector3 tangent =
            GetOrbitTangent(toPlayer);

        float distanceError =
            distance - preferredOrbitDistance;

        float correction = Mathf.Clamp(
            distanceError /
            Mathf.Max(1f, preferredOrbitDistance * 0.4f),
            -1f,
            1f
        );

        Vector3 moveDirection =
            tangent +
            toPlayer * correction *
            orbitCorrectionStrength;

        float verticalMove = Mathf.Sin(
            Time.time * verticalOrbitFrequency + orbitPhase
        ) * verticalOrbitAmount;

        moveDirection +=
            Vector3.up * verticalMove;

        MoveWorld(moveDirection, orbitSpeed);
    }

    // =====================================
    // ’Êí—£’EFˆê’è•ûŒü‚Ö”ò‚Ñ‹‚é
    // =====================================
    void SetBreakAwayDirection()
    {
        Vector3 away =
            transform.position - player.position;

        if (away.sqrMagnitude < 0.001f)
            away = -transform.forward;

        away.Normalize();

        Vector3 tangent =
            GetOrbitTangent(-away);

        breakAwayDirection =
            (away + tangent * 0.4f).normalized;
    }

    void BreakAway()
    {
        // —£’E‚·‚é•ûŒü‚Ö‹@ñ‚ğŒü‚¯‚é
        RotateTowards(
            transform.position + breakAwayDirection
        );

        MoveWorld(
            breakAwayDirection,
            breakAwaySpeed
        );
    }

    // =====================================
    // ‹ß‹——£‚Ì‹Ù‹}—£’E
    // =====================================
    void RetreatFromPlayer()
    {
        RotateTowards(player.position);

        Vector3 away =
            transform.position - player.position;

        if (away.sqrMagnitude < 0.001f)
            away = -transform.forward;

        away.Normalize();

        Vector3 tangent =
            GetOrbitTangent(-away);

        Vector3 moveDirection =
            away + tangent * 0.6f;

        MoveWorld(moveDirection, retreatSpeed);
    }

    // =====================================
    // ƒ‰ƒ“ƒ_ƒ€ˆÚ“®
    // =====================================
    void Wander()
    {
        wanderTimer += Time.deltaTime;

        float distance = Vector3.Distance(
            transform.position,
            wanderTarget
        );

        if (wanderTimer >= wanderChangeInterval ||
            distance < 3f)
        {
            SetRandomWanderPoint();
        }

        RotateTowards(wanderTarget);

        transform.position +=
            transform.forward *
            moveSpeed *
            Time.deltaTime;
    }

    void SetRandomWanderPoint()
    {
        wanderTimer = 0f;

        Vector3 center =
            player != null
            ? player.position
            : transform.position;

        wanderTarget =
            center +
            Random.insideUnitSphere * wanderRadius;
    }

    // =====================================
    // ù‰ñ•ûŒü
    // =====================================
    Vector3 GetOrbitTangent(Vector3 toPlayer)
    {
        Vector3 axis =
            Mathf.Abs(Vector3.Dot(
                toPlayer,
                Vector3.up
            )) > 0.95f
            ? Vector3.forward
            : Vector3.up;

        Vector3 tangent =
            Vector3.Cross(axis, toPlayer);

        if (tangent.sqrMagnitude < 0.0001f)
            tangent = transform.right;

        return tangent.normalized * orbitDirection;
    }

    // =====================================
    // ˆÚ“®
    // =====================================
    void MoveWorld(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return;

        transform.position +=
            direction.normalized *
            speed *
            Time.deltaTime;
    }

    // =====================================
    // ‰ñ“]
    // =====================================
    void RotateTowards(Vector3 target)
    {
        Vector3 direction =
            target - transform.position;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction.normalized
            );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    // =====================================
    // ËŒ‚
    // =====================================
    void TryShoot()
    {
        if (bulletPrefab == null || muzzle == null)
            return;

        if (fireTimer < Mathf.Max(0.01f, fireInterval))
            return;

        Vector3 targetDirection =
            player.position - muzzle.position;

        if (targetDirection.sqrMagnitude < 0.0001f)
            return;

        float angle = Vector3.Angle(
            muzzle.forward,
            targetDirection
        );

        if (angle > fireAngle)
            return;

        fireTimer = 0f;

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

    // =====================================
    // ”ÍˆÍ‚Ì‰Â‹‰»
    // =====================================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            detectDistance
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            transform.position,
            attackDistance
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            retreatDistance
        );
    }
}
