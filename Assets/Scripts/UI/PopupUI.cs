using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using ColorUtility = UnityEngine.ColorUtility;

public class PopupUI : MonoBehaviour
{
    RectTransform m_Rect;
    Animation m_Animation;
    CharacterMaster m_Player;

    public GameObject m_TopSeparationImage;
    public GameObject m_BotSeparationImage;
    public TextMeshProUGUI m_MainDescription;
    public TextMeshProUGUI m_ExtraSpecificationsText;
    [Header("POWER INFO")]
    public GameObject m_TopPowerInfoHolder;
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
    [Header("POSITION")]
    public float m_BottomYPos;
    [Header("TEXT COLORS")]
    public Color m_SelectedTextColor = new Color32(255, 255, 255, 255);
    public Color m_TotalMagicDamageTextColor = new Color32(173, 216, 230, 255);
    public Color m_BonusMagicDamageTextColor = new Color32(203, 195, 227, 255);
    public Color m_TotalFisicDamageTextColor = new Color32(255, 165, 0, 255);
    public Color m_BonusFisicDamageTextColor = new Color32(255, 0, 0, 255);



    private void Awake()
    {
        m_Rect=GetComponent<RectTransform>();
        m_Animation=GetComponent<Animation>();
    }

    public void UpdatePowerPopupInfo(Skill _Skill, string Key, int SkillLV, bool LevelingUp)
    {
        m_TopPowerInfoHolder.SetActive(true);
        m_TopSeparationImage.SetActive(true);
        m_StatName.gameObject.SetActive(false);
        m_MainDescription.text = _Skill.m_Description;
        m_PowerName.text = _Skill.m_PowerName;
        m_PowerKey.text = "[" + Key + "]";
        if (_Skill.GetCd().ToString() != null && _Skill.m_PowerType!=Power.PowerType.PASSIVESKILL)
            m_PowerCd.text = _Skill.GetCd().ToString() + " s";
        else
            m_PowerCd.text = "";
        if (_Skill.GetMana(SkillLV) != 0.0f)
            m_SkillMana.text = _Skill.GetMana(SkillLV).ToString() + " Mana";
        else
            m_SkillMana.text = "";
        m_PowerImage.sprite = _Skill.m_Sprite;

        foreach (GameObject Obj in m_OldSkillsSpecificationsList)
            if (Obj != null)
                Destroy(Obj);
        m_OldSkillsSpecificationsList.Clear();

        if(!LevelingUp || (LevelingUp && SkillLV>0))
        {
            foreach (SkillAttribute Attribute in _Skill.m_AttributeList)
            {
                if (Attribute.m_LevelScaling != null && Attribute.m_LevelScaling.Count > 0 && Attribute.m_ShowScalingInPopup)
                {
                    SkillsSpecificationsPrefabUI l_SkillsSpecificationsPrefabUI = Instantiate(m_SkillsSpecificationsPrefab, m_SkillsSpecificationsContent.transform).GetComponent<SkillsSpecificationsPrefabUI>();
                    m_OldSkillsSpecificationsList.Add(l_SkillsSpecificationsPrefabUI.gameObject);
                    string SkillStatsLV = "";

                    if(!LevelingUp) 
                    {
                        int i = 0;
                        foreach (float Stat in Attribute.m_LevelScaling)
                        {
                            i++;
                            string l_Stat = "";
                            if (i == SkillLV || (SkillLV == 0 && i == 1))
                            {
                                l_Stat = $"<b><color=#{ColorUtility.ToHtmlStringRGB(m_SelectedTextColor)}>" + Stat.ToString() + "</color></b>";
                            }
                            else
                                l_Stat = Stat.ToString();

                            if (SkillStatsLV == "")
                                SkillStatsLV = l_Stat;
                            else
                                SkillStatsLV = SkillStatsLV + " / " + l_Stat;
                        }
                        SkillStatsLV = "[ " + SkillStatsLV + " ]";
                    }
                    else 
                        SkillStatsLV = Attribute.m_LevelScaling[SkillLV-1].ToString() + " -> " + Attribute.m_LevelScaling[SkillLV].ToString();
                
                    l_SkillsSpecificationsPrefabUI.SetSkillsSpecificationsPrefabUI(Attribute.m_AttributeId, SkillStatsLV);
                }
            }
        }

        float l_BaseDamage=_Skill.GetAttribute("Daño base", SkillLV);
        foreach(SkillDescriptionDamage DamageDescription in _Skill.m_DescriptionDamageList) 
        {
            string l_BaseDamageText= $"<b><color=#{ColorUtility.ToHtmlStringRGB(m_SelectedTextColor)}>" + (l_BaseDamage*DamageDescription.m_BaseDamageMultiplier) + "</color></b>";

            string l_BonusDamageText;
            float l_BonusDamage;
            string  l_TotalDamageColorText;
            string l_DamageTypeText;
            if(DamageDescription.m_IsMagicDamage) 
            {
                l_BonusDamage=m_Player.GetCharacterStats().GetAbilityPower()*(DamageDescription.m_BonusDamagePct/100.0f);
                l_BonusDamageText=$"<b><color=#{ColorUtility.ToHtmlStringRGB(m_BonusMagicDamageTextColor)}> +" + DamageDescription.m_BonusDamagePct + "% del daño magico adicional</color></b>";
                l_TotalDamageColorText=$"<b><color=#{ColorUtility.ToHtmlStringRGB(m_TotalMagicDamageTextColor)}>";
                l_DamageTypeText=") de daño mágico</color></b>";
            }
            else 
            {
                l_BonusDamage=m_Player.GetCharacterStats().GetBonusAttackDamage()*(DamageDescription.m_BonusDamagePct/100.0f);
                l_BonusDamageText=$"<b><color=#{ColorUtility.ToHtmlStringRGB(m_BonusFisicDamageTextColor)}> +" + DamageDescription.m_BonusDamagePct + "% del daño de ataque adicional</color></b>"; 
                l_TotalDamageColorText=$"<b><color=#{ColorUtility.ToHtmlStringRGB(m_TotalFisicDamageTextColor)}>";
                l_DamageTypeText=") de daño físico</color></b>";
            }

            float l_TotalDamage=l_BaseDamage*DamageDescription.m_BaseDamageMultiplier+l_BonusDamage;
            string l_TotalDamageText=l_TotalDamageColorText + l_TotalDamage;
            
            string l_DescriptionText=l_TotalDamageText + " = (</color></b>" + l_BaseDamageText + l_BonusDamageText + l_TotalDamageColorText + l_DamageTypeText;

            m_MainDescription.text = m_MainDescription.text.Replace(DamageDescription.m_DescriptionId, l_DescriptionText);
        }

        if(_Skill.m_ExtraSpecifications.Length>0) 
        {
            m_ExtraSpecificationsText.text = "";
            m_ExtraSpecificationsText.gameObject.SetActive(true);
            foreach(string Specification in _Skill.m_ExtraSpecifications) 
            {
                m_ExtraSpecificationsText.text = m_ExtraSpecificationsText.text + Specification + "\n";
            }
        }
        else
            m_ExtraSpecificationsText.gameObject.SetActive(false);

        if(m_OldSkillsSpecificationsList.Count<=0 && _Skill.m_ExtraSpecifications.Length<=0)
            m_BotSeparationImage.SetActive(false);
        else
            m_BotSeparationImage.SetActive(true);
    }

    public void UpdatePowerPopupInfo(Summoner _Summoner, string Key)
    {
        m_TopPowerInfoHolder.SetActive(true);
        m_TopSeparationImage.SetActive(true);
        m_BotSeparationImage.SetActive(false);
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

        foreach (GameObject Obj in m_OldSkillsSpecificationsList)
            if (Obj != null)
                Destroy(Obj);
        m_OldSkillsSpecificationsList.Clear();
    }

    public void UpdateStatPopupInfo(string Description, string Name)
    {
        m_TopPowerInfoHolder.SetActive(false);
        m_TopSeparationImage.SetActive(true);
        m_BotSeparationImage.SetActive(false);
        m_StatName.gameObject.SetActive(true);
        m_MainDescription.text = Description;
        m_StatName.text = Name;
    }
    
    public void ChangePopupPos() 
    {
        float l_Height=m_Rect.sizeDelta.y;
        float l_NewPos=m_BottomYPos+(l_Height/2.0f);
        m_Rect.anchoredPosition=new Vector2(m_Rect.anchoredPosition.x, l_NewPos);
    }

    public void StopAnimation() 
    {
        m_Animation.Stop();
    }
    //LLAMADA POR EVENTO EN LA ANIMACION DE SHOWPOPUP
    public void PlayShowAnimation() 
    {
        m_Animation.Play();
    }

    //GETTERS & SETTERS
    public void SetPlayer(CharacterMaster Player) 
    {
        m_Player=Player;
    }
}