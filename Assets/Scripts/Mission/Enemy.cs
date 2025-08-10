using System;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    public enum Faction
    {
        Alley1,
        Alley2,
        Enemy1,
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
