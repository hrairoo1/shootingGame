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
        // 必要に応じて追加
    }

    [Header("Faction")]
    public Faction faction; // 所属

    [Header("Attributes")]
    public Attributes attributes = Attributes.None; // 属性

    public string TagId { get; private set; }
    private MissionManager missionManager;

    public void InitTag(string tagId, MissionManager manager)
    {
        TagId = tagId;
        missionManager = manager;
    }
    // 指定された所属を持っているか確認するヘルパーメソッド
    public bool HasFaction(Faction getFaction)
    {
        return faction == getFaction;
    }
    // 指定された属性を持っているか確認するヘルパーメソッド
    public bool HasAttribute(Attributes attribute)
    {
        return (attributes & attribute) != 0; // いずれかのビットが一致していれば true
    }
}
