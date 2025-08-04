using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // カーソルを中央に固定
        Cursor.visible = false; // カーソルを非表示
    }

    void Update()
    {
        // ESCキーでロック解除（ゲームメニューなどを開くとき用）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None; // ロック解除
            Cursor.visible = true; // カーソルを表示
        }
    }

}
