using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public Transform aimTarget; // �J�����̃G�C���^�[�Q�b�g
    public PlayerWeaponManager weaponManager;

    private Vector3 defaultAimPosition;
    private Vector3 recoilOffset;

    void Start()
    {
        defaultAimPosition = aimTarget.localPosition;
    }

    void Update()
    {
        // ���ݑ������̕���̔������擾
        Weapon weaponR = weaponManager.GetWeapon("MainR");
        Weapon weaponL = weaponManager.GetWeapon("MainL");

        Vector3 recoilR = weaponR != null ? weaponR.GetRecoilOffset() : Vector3.zero;
        Vector3 recoilL = weaponL != null ? weaponL.GetRecoilOffset() : Vector3.zero;

        // ���蕐��̉e���𕽋ω�
        recoilOffset = (recoilR + recoilL) * 0.5f;

        // �G�C���ʒu�𒲐�
        aimTarget.localPosition = defaultAimPosition + recoilOffset;
    }
}
