using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [Header("SKILLS")]
    public Image m_QSkillCdImage;
    public Image m_WSkillCdImage;
    public Image m_ESkillCdImage;
    public Image m_RSkillCdImage;
    public Image m_SumSpell1CdImage;
    public Image m_SumSpell2CdImage; 
    public TextMeshProUGUI m_QSkillCdText;
    public TextMeshProUGUI m_WSkillCdText;
    public TextMeshProUGUI m_ESkillCdText;
    public TextMeshProUGUI m_RSkillCdText;
    public TextMeshProUGUI m_SumSpell1CdText;
    public TextMeshProUGUI m_SumSpell2CdText;
    public Transform m_QLevelUpButton;
    public Transform m_WLevelUpButton;
    public Transform m_ELevelUpButton;
    public Transform m_RLevelUpButton;
    public Slider m_QLevelPoints;
    public Slider m_WLevelPoints;
    public Slider m_ELevelPoints;
    public Slider m_RLevelPoints;

    [Header("EXPERIENCE")]
    public Slider m_ExpBar;
    public TextMeshProUGUI m_LevelText;

    [Header("HEALTH MANA BARS")]
    public Slider m_HealthBar;
    public Slider m_ManaBar;
    public TextMeshProUGUI m_HealthText;
    public TextMeshProUGUI m_ManaText;

    [Header("STATS")]
    public RectTransform m_PrimStatsPanel;
    public RectTransform m_SeconStatsPanel;
    public TextMeshProUGUI m_AttackDamageText;
    public TextMeshProUGUI m_ArmorText;
    public TextMeshProUGUI m_AttackSpeedText;
    public TextMeshProUGUI m_CriticalChanceText;
    public TextMeshProUGUI m_AbilityPowerText;
    public TextMeshProUGUI m_MagicResistanceText;
    public TextMeshProUGUI m_CooldownReductionText;
    public TextMeshProUGUI m_MovementSpeedText;

    public TextMeshProUGUI m_HealthManaRegenText;
    public TextMeshProUGUI m_ArmorPenText;
    public TextMeshProUGUI m_LifestealText;
    public TextMeshProUGUI m_AttackRangeText;
    public TextMeshProUGUI m_HealsShieldsPowerText;
    public TextMeshProUGUI m_MagicPenText;
    public TextMeshProUGUI m_OmnidrainText;
    public TextMeshProUGUI m_TenacityText;

    [Header("RECALL UI")]
    public RectTransform m_RecallUI;
    public Slider m_RecallBar;
    public TextMeshProUGUI m_RecallTimeText;

    public CharacterMaster m_Character;

    void Start()
    {
        HideSeconStatsPanel();
        HideRecallUI();
        HideCdTexts();
        HideLevelUpSkillButtons();
        ResetSkillLevelPoints();
        m_Character=transform.GetComponent<CharacterMaster>();
    }
    public void UpdateHealthManaBars(float Health, float MaxHealth, float Mana, float MaxMana)
    {
        float l_HealthRounded=Mathf.Round(Health);
        float l_ManaRounded=Mathf.Round(Mana);
        m_HealthBar.value=l_HealthRounded/MaxHealth;
        m_ManaBar.value=l_ManaRounded/MaxMana;
        m_HealthText.text=l_HealthRounded+"/"+MaxHealth;
        m_ManaText.text=l_ManaRounded+"/"+MaxMana;
    }
    public void UpdatePrimStats(float AtkDmg, float Armor, float AtkSpd, float CritChance, float AbPower, float MagResist, float Cdr, float MovSpeed)
    {
        m_AttackDamageText.text=AtkDmg.ToString();
        m_ArmorText.text=Armor.ToString();
        m_AttackSpeedText.text=AtkSpd.ToString();
        m_CriticalChanceText.text=CritChance.ToString();
        m_AbilityPowerText.text=AbPower.ToString();
        m_MagicResistanceText.text=MagResist.ToString();
        m_CooldownReductionText.text=Cdr.ToString();
        m_MovementSpeedText.text=MovSpeed.ToString();
    }
    public void UpdateSeconStats(float HealthRegen, float ArmorPenFix, float ArmorPenPct, float Lifesteal, float AttackRange, float ManaRegen, 
        float MagicPenFix, float MagicPenPct, float Omnidrain, float Tenacity, float HealsShieldsPower)
    {
        m_HealthManaRegenText.text=HealthRegen.ToString()+"|"+ManaRegen.ToString();
        m_ArmorPenText.text=ArmorPenFix.ToString()+"|"+ArmorPenPct.ToString()+"%";
        m_LifestealText.text=Lifesteal.ToString()+"%";
        m_AttackRangeText.text=AttackRange.ToString();
        m_HealsShieldsPowerText.text=HealsShieldsPower.ToString()+"%";
        m_MagicPenText.text=MagicPenFix.ToString()+"|"+MagicPenPct.ToString()+"%";
        m_OmnidrainText.text=Omnidrain.ToString()+"%";
        m_TenacityText.text=Tenacity.ToString()+"%";
    }
    public void UpdateExpBar(float CurrentExp, float NeededExp)
    {
        m_ExpBar.value=CurrentExp/NeededExp;
    }
    public void UpdateCharacterLevel(int Level)
    {
        m_LevelText.text=Level.ToString();
    }
    public void UpdateRecallUI(float CurrentRecallTime, float MaxRecallTime)
    {
        m_RecallBar.value=CurrentRecallTime/MaxRecallTime;
        m_RecallTimeText.text=CurrentRecallTime.ToString("f1");
    }
    public void LevelUpQSkill()
    {
        m_QLevelPoints.value+=1;
        m_Character.m_SkillPoints-=1;
        if(m_Character.m_SkillPoints<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpWSkill()
    {
        m_WLevelPoints.value+=1;
        m_Character.m_SkillPoints-=1;
        if(m_Character.m_SkillPoints<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpESkill()
    {
        m_ELevelPoints.value+=1;
        m_Character.m_SkillPoints-=1;
        if(m_Character.m_SkillPoints<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpRSkill()
    {
        m_RLevelPoints.value+=1;
        m_Character.m_SkillPoints-=1;
        if(m_Character.m_SkillPoints<=0)
            HideLevelUpSkillButtons();
    }
    public void ResetSkillLevelPoints()
    {
        m_QLevelPoints.value=0;
        m_WLevelPoints.value=0;
        m_ELevelPoints.value=0;
        m_RLevelPoints.value=0;
    }

    //SHOW & HIDE METHODS
    public void ShowSeconStatsPanel()
    {
        m_SeconStatsPanel.gameObject.SetActive(true);
    }
    public void HideSeconStatsPanel()
    {
        m_SeconStatsPanel.gameObject.SetActive(false);
    }
    public void ShowRecallUI()
    {
        m_RecallUI.gameObject.SetActive(true);
    }
    public void HideRecallUI()
    {
        m_RecallUI.gameObject.SetActive(false);
    }
    public void HideCdTexts()
    {
        m_QSkillCdText.enabled=false;
        m_WSkillCdText.enabled=false;
        m_ESkillCdText.enabled=false;
        m_RSkillCdText.enabled=false;
        m_SumSpell1CdText.enabled=false;
        m_SumSpell2CdText.enabled=false;
    }
    public void ShowLevelUpSkillButtons()
    {
        m_QLevelUpButton.gameObject.SetActive(true);
        m_WLevelUpButton.gameObject.SetActive(true);
        m_ELevelUpButton.gameObject.SetActive(true);
        m_RLevelUpButton.gameObject.SetActive(true);
    }
    public void HideLevelUpSkillButtons()
    {
        m_QLevelUpButton.gameObject.SetActive(false);
        m_WLevelUpButton.gameObject.SetActive(false);
        m_ELevelUpButton.gameObject.SetActive(false);
        m_RLevelUpButton.gameObject.SetActive(false);
    }
}
