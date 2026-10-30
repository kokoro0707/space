
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 20f;
    public float boostSpeed = 40f;

    [Header("マウス回転速度")]
    public float rotationSpeed = 120f;

    [Header("機体モデル")]
    public Transform visualModel;

    [Header("ロール設定")]
    public float maxRollAngle = 45f;
    public float rollSmooth = 5f;

    [Header("緊急回避")]
    public float dodgeSpeed = 85f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 2f;

    [Header("マウス設定")]
    public bool lockCursor = true;

    public float DodgeCooldownRemaining =>
        cooldownTimer;

    private float currentRoll;
    private Quaternion initialVisualRotation;

    private float dodgeTimer;
    private float cooldownTimer;

    private Vector3 dodgeDirection;
    private bool isDodging;

    void Start()
    {
        if (visualModel != null)
        {
            initialVisualRotation =
                visualModel.localRotation;
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        // マウス操作
        UpdateRotation();

        // 移動入力
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float upDown = 0f;

        if (Input.GetKey(KeyCode.E))
            upDown = 1f;

        if (Input.GetKey(KeyCode.Q))
            upDown = -1f;

        Vector3 moveInput = new Vector3(
            h, upDown, v
        );

        moveInput = Vector3.ClampMagnitude(
            moveInput, 1f
        );

        // 回避クールタイム
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(0f, cooldownTimer);
        }

        // 緊急回避開始
        if (!isDodging &&
            cooldownTimer <= 0f &&
            Input.GetKeyDown(KeyCode.F))
        {
            StartDodge(moveInput);
        }

        if (isDodging)
        {
            UpdateDodge();
        }
        else
        {
            MoveNormally(moveInput);
        }

        // 機体の見た目だけを傾ける
        UpdateVisualRoll(h);
    }

    // =================================
    // マウス視点操作（元の方式）
    // =================================
    void UpdateRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 newRotation = transform.rotation.eulerAngles;

        newRotation.x -= mouseY * rotationSpeed * Time.deltaTime;
        newRotation.y += mouseX * rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(
            newRotation.x,
            newRotation.y,
            0f
        );
    }

    // =================================
    // 通常移動
    // =================================
    void MoveNormally(Vector3 moveInput)
    {
        float speed =
            Input.GetKey(KeyCode.LeftShift)
            ? boostSpeed
            : moveSpeed;

        Vector3 move =
            transform.TransformDirection(moveInput);

        transform.position +=
            move * speed * Time.deltaTime;
    }

    // =================================
    // 緊急回避開始
    // =================================
    void StartDodge(Vector3 moveInput)
    {
        if (moveInput.sqrMagnitude < 0.01f)
        {
            moveInput = Vector3.forward;
        }

        dodgeDirection =
            transform.TransformDirection(
                moveInput.normalized
            );

        isDodging = true;
        dodgeTimer = dodgeDuration;
        cooldownTimer = dodgeCooldown;
    }

    // =================================
    // 緊急回避中
    // =================================
    void UpdateDodge()
    {
        transform.position +=
            dodgeDirection *
            dodgeSpeed *
            Time.deltaTime;

        dodgeTimer -= Time.deltaTime;

        if (dodgeTimer <= 0f)
        {
            isDodging = false;
        }
    }

    // =================================
    // 機体モデルのロール
    // =================================
    void UpdateVisualRoll(float h)
    {
        float targetRoll = -h * maxRollAngle;

        currentRoll = Mathf.Lerp(
            currentRoll,
            targetRoll,
            Time.deltaTime * rollSmooth
        );

        // プレイヤー本体にロールを適用
        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            currentRoll
        );
    }
}
