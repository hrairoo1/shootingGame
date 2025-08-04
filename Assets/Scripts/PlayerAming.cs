using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public Transform aimTarget; // カメラのエイムターゲット
    public PlayerWeaponManager weaponManager;

    private Vector3 defaultAimPosition;
    private Vector3 recoilOffset;

    void Start()
    {
        defaultAimPosition = aimTarget.localPosition;
    }

    void Update()
    {
        // 現在装備中の武器の反動を取得
        Weapon weaponR = weaponManager.GetWeapon("MainR");
        Weapon weaponL = weaponManager.GetWeapon("MainL");

        Vector3 recoilR = weaponR != null ? weaponR.GetRecoilOffset() : Vector3.zero;
        Vector3 recoilL = weaponL != null ? weaponL.GetRecoilOffset() : Vector3.zero;

        // 両手武器の影響を平均化
        recoilOffset = (recoilR + recoilL) * 0.5f;

        // エイム位置を調整
        aimTarget.localPosition = defaultAimPosition + recoilOffset;
    }
}
