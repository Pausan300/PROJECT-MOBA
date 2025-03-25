using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
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
    public GameObject m_SkillsSpecificationsPrefab;
    public GameObject m_SkillsSpecificationsContent;
    List<GameObject> m_OldSkillsSpecificationsList = new List<GameObject>();
    [Header("STAT INFO")]
    public TextMeshProUGUI m_StatName;

    public void UpdatePowerPopupInfo(Skill _Skill, string Key, string Mana)
    {
        m_TopPowerInfoHolder.SetActive(true);
        m_BotPowerInfoHolder.SetActive(true);
        m_TopSeparationImage.SetActive(true);
        m_BotSeparationImage.SetActive(true);
        m_StatName.gameObject.SetActive(false);
        m_MainDescription.text = _Skill.m_Description.Replace("X", "HOLA");
        m_PowerName.text = _Skill.m_PowerName;
        m_PowerKey.text = "[" + Key + "]";
        if (_Skill.GetCd().ToString() != null)
            m_PowerCd.text = _Skill.GetCd().ToString() + " s";
        else
            m_PowerCd.text = "";
        if (Mana != null)
            m_SkillMana.text = Mana + " Mana";
        else
            m_SkillMana.text = "";
        m_PowerImage.sprite = _Skill.m_Sprite;

        foreach (GameObject Obj in m_OldSkillsSpecificationsList)
        {
            if (Obj != null)
            {
                Destroy(Obj);
            }
        }

        m_OldSkillsSpecificationsList.Clear();

        foreach (SkillAttribute Attribute in _Skill.m_AttributeList)
        {
            SkillsSpecificationsPrefabUI l_SkillsSpecificationsPrefabUI = Instantiate(m_SkillsSpecificationsPrefab, m_SkillsSpecificationsContent.transform).GetComponent<SkillsSpecificationsPrefabUI>();


            l_SkillsSpecificationsPrefabUI.SetSkillsSpecificationsPrefabUI(Attribute.m_AttributeId, "");
        }
    }
    
    public void UpdatePowerPopupInfo(Summoner _Summoner, string Key)
    {
        m_TopPowerInfoHolder.SetActive(true);
        m_BotPowerInfoHolder.SetActive(true);
        m_TopSeparationImage.SetActive(true);
        m_BotSeparationImage.SetActive(true);
        m_StatName.gameObject.SetActive(false);
        m_MainDescription.text = _Summoner.m_Description.Replace("X", "HOLA");
        m_PowerName.text = _Summoner.m_PowerName;
        m_PowerKey.text = "[" + Key + "]";
        if (_Summoner.GetCd().ToString() != null)
            m_PowerCd.text = _Summoner.GetCd().ToString() + " s";
        else
            m_PowerCd.text = "";

        m_SkillMana.text = "";
        m_PowerImage.sprite = _Summoner.m_Sprite;
    }

    public void UpdateStatPopupInfo(string Description, string Name)
    {
        m_TopPowerInfoHolder.SetActive(false);
        m_BotPowerInfoHolder.SetActive(false);
        m_TopSeparationImage.SetActive(true);
        m_BotSeparationImage.SetActive(false);
        m_StatName.gameObject.SetActive(true);
        m_MainDescription.text = Description;
        m_StatName.text = Name;
    }
}
