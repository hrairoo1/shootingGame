using UnityEngine;
using UnityEngine.UI;

public class ScrollToSelected : MonoBehaviour
{
    [SerializeField] EquipSelectWindow selectionUI;
    public ScrollRect scrollRect;  // スクロールビュー
    public RectTransform scrollView;  // スクロールビュー
    public RectTransform content;  // コンテンツ（ボタンが入っている親）
    public Vector2 contentPos;  // コンテンツ（ボタンが入っている親）
    private float contentUp;
    private float contentDown;
    private float contentLeft;
    private float contentRight;
    private Vector2 targetPos = new Vector2(0, 0);
    public float scroolSpeed = 5f;

    private void Start()
    {
        contentPos = content.position;
    }
    private void Update()
    {
        if (!selectionUI.isMouseVisible) scrollRect.content.anchoredPosition = Vector2.Lerp(scrollRect.content.anchoredPosition, targetPos, scroolSpeed * Time.deltaTime);
    }
    public void ScrollToButton(RectTransform selectedButton)
    {
        // Viewport の RectTransform を取得
        RectTransform viewport = scrollRect.viewport;
        contentUp = -content.position.y + contentPos.y + scrollView.rect.height;
        contentDown = - content.position.y + contentPos.y;
        contentRight = scrollView.rect.width / 2;
        contentLeft = -content.position.x + contentPos.x;
        
        // ボタンのローカル座標を取得
        Vector2 localPos = content.InverseTransformPoint(selectedButton.position);
        if (selectionUI.isMouseVisible) return;
        // ボタンがスクロールビュー内に収まっていない場合
        // 上方向にスクロール: ボタンのY位置がビューの上1/4を越えた場合
        if (localPos.y > contentUp - selectedButton.rect.height * 3 / 2)
        {
            // 上方向にスクロールするための位置計算
            float newY = localPos.y + viewport.rect.height / 4 - selectedButton.rect.height / 2;
            // スクロール位置を計算して更新
            targetPos.y = Mathf.Max(-newY, 0);  // 上方向にスクロールしすぎないように制限
        }
        else if (localPos.y < contentDown + selectedButton.rect.height * 5 / 2)
        {
            // 下方向にスクロールするための位置計算
            float newY = localPos.y + viewport.rect.height - viewport.rect.height * 1 / 4 + selectedButton.rect.height / 2 ;
            // スクロール位置を計算して更新
            targetPos.y = Mathf.Min(-newY, scrollRect.content.rect.height - viewport.rect.height);  // 下方向にスクロールしすぎないように制限
        }

        // 横方向
        // 左方向にスクロール
        if (localPos.x > contentLeft - selectedButton.rect.height / 2)
        {
            float newX = localPos.x + viewport.rect.width / 4 - selectedButton.rect.width / 2;
            targetPos.x = Mathf.Max(-newX, 0);
        }/*
        // 右方向にスクロール
        else if (localPos.x - selectedButton.rect.width / 2 < viewport.rect.width / 2 && false)
        {
            float newX = localPos.x + viewport.rect.width / 2 - selectedButton.rect.width / 2;
            scrollRect.content.anchoredPosition = new Vector2(-newX, scrollRect.content.anchoredPosition.y);
        }*/

    }
}
