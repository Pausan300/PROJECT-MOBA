using System.Collections.Generic;
using System.Net.Sockets;
using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EmoteUI : MonoBehaviour
{
    CharacterMaster m_Character;

    public List<Button> m_Buttons;
    List<Image> m_EquipedEmotes=new List<Image>();
    public RectTransform m_Selector;
    public float m_MaxEmoteDuration;

    Vector2 m_ScreenPosition;
    int m_SelectedEmote;

    void Start()
    {
        HideEmoteWheel();
        for(int i=0; i<m_Buttons.Count; ++i)
            m_EquipedEmotes.Add(m_Buttons[i].gameObject.GetComponent<Image>());
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
            HideEmoteWheel();

        if(Input.GetMouseButtonDown(0) || Input.GetKeyUp(KeyCode.T)) 
        {
            UseSelectedEmote();
            HideEmoteWheel();
        }
        
        Vector2 l_DirectionToCursor=Input.mousePosition-m_Selector.position;
        if(l_DirectionToCursor.magnitude<50.0f)
        {
            m_Selector.gameObject.SetActive(false);
            m_SelectedEmote=8;
            m_Buttons[m_SelectedEmote].Select();
        }
        else
        {
            m_Selector.gameObject.SetActive(true);
            float l_Angle=Mathf.Atan2(l_DirectionToCursor.y, l_DirectionToCursor.x)*Mathf.Rad2Deg;
            m_Selector.localRotation=Quaternion.Euler(0, 0, l_Angle);
            m_SelectedEmote=Mathf.RoundToInt((l_Angle+180.0f)/45.0f);
            if(m_SelectedEmote==8)
                m_SelectedEmote=0;
            m_Buttons[m_SelectedEmote].Select();
        }
        //Debug.Log(l_DirectionToCursor.magnitude);
    }

    public void UseSelectedEmote() 
    {
        m_Character.m_IngameCharacterUI.ShowEmote(m_EquipedEmotes[m_SelectedEmote], m_MaxEmoteDuration);
    }

    //SHOW & HIDE METHODS
    public void ShowEmoteWheel() 
    {
        gameObject.SetActive(true);
        m_ScreenPosition=Input.mousePosition;
        RectTransform l_RectTransform=gameObject.GetComponent<RectTransform>();
        l_RectTransform.position=m_ScreenPosition;
    }
    public void HideEmoteWheel() 
    {
        gameObject.SetActive(false);
    }

    //GETTERS & SETTERS
    public void SetPlayer(CharacterMaster Player)
    {
        m_Character=Player;
    }
}
