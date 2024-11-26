using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectableElementUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public CharacterUI.PopupType m_PopupType;
	public CharacterUI m_CharacterUI;

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_CharacterUI.SetPopupType(m_PopupType);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		m_CharacterUI.HidePopup();
	}
}
