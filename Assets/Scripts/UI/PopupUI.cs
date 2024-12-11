using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public GameObject m_TopSeparationImage;
    public GameObject m_BotSeparationImage;
    public TextMeshProUGUI m_MainDescription;
    [Header("POWER INFO")]
    public GameObject m_TopPowerInfoHolder;
    public GameObject m_BotPowerInfoHolder;
    public Image m_PowerImage;
    public TextMeshProUGUI m_PowerName;
    public TextMeshProUGUI m_PowerKey;
    public TextMeshProUGUI m_PowerCd;
    public TextMeshProUGUI m_SkillMana;
    [Header("STAT INFO")]
    public TextMeshProUGUI m_StatName;

    public void UpdatePowerPopupInfo(string Description, string Name, string Key, string Cd, string Mana, Sprite Icon)
    {
        m_TopPowerInfoHolder.SetActive(true);
        m_BotPowerInfoHolder.SetActive(true);
        m_TopSeparationImage.SetActive(true);
        m_BotSeparationImage.SetActive(true);
        m_StatName.gameObject.SetActive(false);
        m_MainDescription.text=Description.Replace("X", "HOLA");
        m_PowerName.text=Name;
        m_PowerKey.text="["+Key+"]";
        if(Cd!=null)
            m_PowerCd.text=Cd+" s";
        else
            m_PowerCd.text="";
        if(Mana!=null)
            m_SkillMana.text=Mana+" Mana";
        else
            m_SkillMana.text="";
        m_PowerImage.sprite=Icon;
    }
    public void UpdateStatPopupInfo(string Description, string Name) 
    {
        m_TopPowerInfoHolder.SetActive(false);
        m_BotPowerInfoHolder.SetActive(false);
        m_TopSeparationImage.SetActive(true);
        m_BotSeparationImage.SetActive(false);
        m_StatName.gameObject.SetActive(true);
        m_MainDescription.text=Description;
        m_StatName.text=Name;
    }
}
