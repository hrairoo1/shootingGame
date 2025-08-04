using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverOnSelect : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Input System ���g�p���ă{�^����I��
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
