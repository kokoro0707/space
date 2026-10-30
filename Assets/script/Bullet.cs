
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("’e‚Ìİ’è")]
    public float speed = 40f;
    public float lifeTime = 5f;

    private int damage = 25;
    private ShipTeam ownerTeam;

    private bool initialized = false;
    private bool hasHit = false;

    // ”­Ë‚ÉŠ‘®‚ÆˆĞ—Í‚ğİ’è
    public void Initialize(ShipTeam team, int bulletDamage)
    {
        ownerTeam = team;
        damage = Mathf.Max(1, bulletDamage);
        initialized = true;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position +=
            transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!initialized || hasHit) return;

        ShipHealth target =
            other.GetComponentInParent<ShipHealth>();

        if (target == null) return;

        // –¡•û‚Ö‚ÌŒëË‚ğ–³Œø‰»
        if (target.Team == ownerTeam) return;

        hasHit = true;

        target.TakeDamage(damage);

        Destroy(gameObject);
    }
}
