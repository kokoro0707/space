
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("”­ŽËÝ’è")]
    public GameObject bulletPrefab;
    public Transform muzzle;

    [Header("•Ší«”\")]
    public float fireInterval = 0.15f;
    public int bulletDamage = 25;

    private float fireTimer;

    void Start()
    {
        fireTimer = fireInterval;
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) &&
            fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            Shoot();
        }
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
                ShipTeam.Player,
                bulletDamage
            );
        }
    }
}
