using Game.Interfaces;
using System;
using UnityEngine;

public class Unit : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public float currentHealth = 0f;
    public enum Faction
    {
        Neutral,
        Ally,
        Ally2,
        Enemy,
        Enemy2
    }

    [Flags]
    public enum Attributes
    {
        None = 0,
        human = 1 << 0,      // 1
        vehicle = 1 << 1,   // 2
        Debris = 1 << 2,    // 4
        Plane = 1 << 3    // 8
        // �K�v�ɉ����Ēǉ�
    }

    [Header("Faction")]
    public Faction faction; // ����

    [Header("Attributes")]
    public Attributes attributes = Attributes.None; // ����

    public string TagId { get; private set; }
    private MissionManager missionManager;
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
        missionManager.OnEnemyKilled(gameObject, TagId);
        Debug.Log(gameObject.name + " died.");
        Destroy(gameObject);
    }
    public void InitTag(string tagId, MissionManager manager)
    {
        TagId = tagId;
        missionManager = manager;
    }
    // �w�肳�ꂽ�����������Ă��邩�m�F����w���p�[���\�b�h
    public bool HasFaction(Faction getFaction)
    {
        return faction == getFaction;
    }
    // �w�肳�ꂽ�����������Ă��邩�m�F����w���p�[���\�b�h
    public bool HasAttribute(Attributes attribute)
    {
        return (attributes & attribute) != 0; // �����ꂩ�̃r�b�g����v���Ă���� true
    }
}
