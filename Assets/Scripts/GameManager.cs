using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // �J�[�\���𒆉��ɌŒ�
        Cursor.visible = false; // �J�[�\�����\��
    }

    void Update()
    {
        // ESC�L�[�Ń��b�N�����i�Q�[�����j���[�Ȃǂ��J���Ƃ��p�j
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None; // ���b�N����
            Cursor.visible = true; // �J�[�\����\��
        }
    }

}
