using UnityEngine;
using UnityEngine.UI;

public class ScrollToSelected : MonoBehaviour
{
    [SerializeField] EquipSelectWindow selectionUI;
    public ScrollRect scrollRect;  // �X�N���[���r���[
    public RectTransform scrollView;  // �X�N���[���r���[
    public RectTransform content;  // �R���e���c�i�{�^���������Ă���e�j
    public Vector2 contentPos;  // �R���e���c�i�{�^���������Ă���e�j
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
        // Viewport �� RectTransform ���擾
        RectTransform viewport = scrollRect.viewport;
        contentUp = -content.position.y + contentPos.y + scrollView.rect.height;
        contentDown = - content.position.y + contentPos.y;
        contentRight = scrollView.rect.width / 2;
        contentLeft = -content.position.x + contentPos.x;
        
        // �{�^���̃��[�J�����W���擾
        Vector2 localPos = content.InverseTransformPoint(selectedButton.position);
        if (selectionUI.isMouseVisible) return;
        // �{�^�����X�N���[���r���[���Ɏ��܂��Ă��Ȃ��ꍇ
        // ������ɃX�N���[��: �{�^����Y�ʒu���r���[�̏�1/4���z�����ꍇ
        if (localPos.y > contentUp - selectedButton.rect.height * 3 / 2)
        {
            // ������ɃX�N���[�����邽�߂̈ʒu�v�Z
            float newY = localPos.y + viewport.rect.height / 4 - selectedButton.rect.height / 2;
            // �X�N���[���ʒu���v�Z���čX�V
            targetPos.y = Mathf.Max(-newY, 0);  // ������ɃX�N���[���������Ȃ��悤�ɐ���
        }
        else if (localPos.y < contentDown + selectedButton.rect.height * 5 / 2)
        {
            // �������ɃX�N���[�����邽�߂̈ʒu�v�Z
            float newY = localPos.y + viewport.rect.height - viewport.rect.height * 1 / 4 + selectedButton.rect.height / 2 ;
            // �X�N���[���ʒu���v�Z���čX�V
            targetPos.y = Mathf.Min(-newY, scrollRect.content.rect.height - viewport.rect.height);  // �������ɃX�N���[���������Ȃ��悤�ɐ���
        }

        // ������
        // �������ɃX�N���[��
        if (localPos.x > contentLeft - selectedButton.rect.height / 2)
        {
            float newX = localPos.x + viewport.rect.width / 4 - selectedButton.rect.width / 2;
            targetPos.x = Mathf.Max(-newX, 0);
        }/*
        // �E�����ɃX�N���[��
        else if (localPos.x - selectedButton.rect.width / 2 < viewport.rect.width / 2 && false)
        {
            float newX = localPos.x + viewport.rect.width / 2 - selectedButton.rect.width / 2;
            scrollRect.content.anchoredPosition = new Vector2(-newX, scrollRect.content.anchoredPosition.y);
        }*/

    }
}
