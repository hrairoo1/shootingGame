using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public float cartridgeLifeTime = 60f;
    public GameObject Explosive1;

    public List<WeaponListEntry> weaponEntries;
    public List<ArmorListEntry> armorEntries;
    public Color _defaltMainColor1 = new Color(0.3686f, 0.3686f, 0.3686f, 1.0f);
    public Color _defaltMainColor2 = new Color(0.4784f, 0.4784f, 0.4784f, 1.0f);
    public Color _defaltMainColor3 = new Color(0.5882f, 0.5882f, 0.5882f, 1.0f);
    public Color _defaltSubColor1 = new Color(0.2275f, 0.2275f, 0.2275f, 1.0f);
    public Color _defaltSubColor2 = new Color(0.2275f, 0.2275f, 0.2275f, 1.0f);
    public Color _defaltEmiColor1 = new Color(0.0f, 1.0f, 0.5647f, 1.0f);
    public Color _defaltEmiColor2 = new Color(0.0f, 0.4667f, 1.0f, 1.0f);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ゲームのシーンが変わっても残す
        }
        else
        {
            Destroy(gameObject); // すでにインスタンスがある場合は新しく作らない
        }
    }
}
