using Game.Interfaces;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public float currentHealth = 0f;

    void Awake()
    {
        currentHealth = health;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took damage: " + damage + " HP left: " + health);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died.");
        Destroy(gameObject);
    }
}
