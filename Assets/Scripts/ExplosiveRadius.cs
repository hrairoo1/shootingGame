using Game.Interfaces;
using UnityEngine;

public class ExplosiveRadius : MonoBehaviour
{
    private float ammoDamage = 0f;

    public void Initialize(float damage, float ExplosiveSize)
    {
        transform.localScale = new Vector3(ExplosiveSize, ExplosiveSize, ExplosiveSize);
        ammoDamage = damage;

        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerEnter(Collider other) // `OnCollisionEnter` Å® `OnTriggerEnter`
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(ammoDamage);
        }
    }
}
