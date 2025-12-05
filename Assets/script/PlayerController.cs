using UnityEngine;

public class PlayerControllerFree : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 20f;
    public float boostSpeed = 40f;

    [Header("回転速度")]
    public float rotationSpeed = 120f;

    [Header("傾き（ロール）設定")]
    public float maxRollAngle = 45f;
    public float rollSmooth = 5f;

    private float currentRoll = 0f;

    void Update()
    {
        // 移動入力（WASD）
        float h = Input.GetAxis("Horizontal");   // A D → 左右移動
        float v = Input.GetAxis("Vertical");     // W S → 前後移動
        float upDown = 0f;

        if (Input.GetKey(KeyCode.E)) upDown = 1f;     // 上昇
        if (Input.GetKey(KeyCode.Q)) upDown = -1f;    // 下降

        // ブースト
        float speed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : moveSpeed;

        Vector3 move = (transform.forward * v + transform.right * h + transform.up * upDown) * speed * Time.deltaTime;

        transform.position += move;


        // マウスで視点回転（Yaw + Pitch）

        float mouseX = Input.GetAxis("Mouse X");   // 左右回転（Yaw）
        float mouseY = Input.GetAxis("Mouse Y");   // 上下回転（Pitch）

        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.x -= mouseY * rotationSpeed * Time.deltaTime;
        newRotation.y += mouseX * rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(newRotation);


        // 左右移動時のロール（傾き）
        float targetRoll = -h * maxRollAngle;
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * rollSmooth);

        // Yaw・Pitch はそのまま、Roll だけ追加する
        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            currentRoll
        );
    }
}
