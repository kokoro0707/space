using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 10f;

    void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}