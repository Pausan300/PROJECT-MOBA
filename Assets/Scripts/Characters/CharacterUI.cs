using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    CharacterMaster m_Character;

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

    [Header("INGAME PLAYER INFO")]
    public Slider m_IngameHealthBar;
    public Slider m_IngameManaBar;
    public TextMeshProUGUI m_IngameLevelText;
    public TextMeshProUGUI m_PlayerNameText;

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

    [Header("CASTING UI")]
    public RectTransform m_CastingUI;
    public Slider m_CastingBar;
    public TextMeshProUGUI m_CastingAbilityText;
    public TextMeshProUGUI m_CastingTimeText;

    void Start()
    {
        HideSeconStatsPanel();
        HideCastingUI();
        HideCdTexts();
    }
    public void UpdateHealthManaBars(float Health, float MaxHealth, float Mana, float MaxMana)
    {
        float l_HealthRounded=Mathf.Round(Health);
        float l_ManaRounded=Mathf.Round(Mana);
        m_HealthBar.value=l_HealthRounded/MaxHealth;
        m_ManaBar.value=l_ManaRounded/MaxMana;
        m_HealthText.text=l_HealthRounded+"/"+Mathf.Round(MaxHealth);
        m_ManaText.text=l_ManaRounded+"/"+Mathf.Round(MaxMana);
        m_IngameHealthBar.value=l_HealthRounded/MaxHealth;
        m_IngameManaBar.value=l_ManaRounded/MaxMana;
    }
    public void UpdatePrimStats(float AtkDmg, float Armor, float AtkSpd, float CritChance, float AbPower, float MagResist, float Cdr, float MovSpeed)
    {
        m_AttackDamageText.text=Mathf.Round(AtkDmg).ToString();
        m_ArmorText.text=Mathf.Round(Armor).ToString();
        m_AttackSpeedText.text=AtkSpd.ToString("f2");
        m_CriticalChanceText.text=CritChance.ToString();
        m_AbilityPowerText.text=AbPower.ToString();
        m_MagicResistanceText.text=Mathf.Round(MagResist).ToString();
        m_CooldownReductionText.text=Cdr.ToString();
        m_MovementSpeedText.text=MovSpeed.ToString();
    }
    public void UpdateSeconStats(float HealthRegen, float ArmorPenFix, float ArmorPenPct, float Lifesteal, float AttackRange, float ManaRegen, 
        float MagicPenFix, float MagicPenPct, float Omnidrain, float Tenacity, float HealsShieldsPower)
    {
        m_HealthManaRegenText.text=Mathf.Round(HealthRegen).ToString()+"|"+Mathf.Round(ManaRegen).ToString();
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
        m_IngameLevelText.text=Level.ToString();
    }
    public void UpdateCastingUI(float CurrentRecallTime, float MaxRecallTime)
    {
        m_CastingBar.value=CurrentRecallTime/MaxRecallTime;
        m_CastingTimeText.text=CurrentRecallTime.ToString("f1");
    }
    public void LevelUpQSkill()
    {
        m_QLevelPoints.value+=1;
        m_Character.m_SkillPoints-=1;
        m_Character.m_QSkillLevel++;
        if(m_Character.m_QSkillLevel>=5)
            m_QLevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_SkillPoints<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpWSkill()
    {
        m_WLevelPoints.value+=1;
        m_Character.m_SkillPoints-=1;
        m_Character.m_WSkillLevel++;
        if(m_Character.m_WSkillLevel>=5)
            m_WLevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_SkillPoints<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpESkill()
    {
        m_ELevelPoints.value+=1;
        m_Character.m_SkillPoints-=1;
        m_Character.m_ESkillLevel++;
        if(m_Character.m_ESkillLevel>=5)
            m_ELevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_SkillPoints<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpRSkill()
    {
        m_RLevelPoints.value+=1;
        m_Character.m_SkillPoints-=1;
        m_Character.m_RSkillLevel++;
        if(m_Character.m_RSkillLevel>=3 || (m_Character.m_CurrentLevel<11 && m_Character.m_RSkillLevel>=1) || (m_Character.m_CurrentLevel<16 && m_Character.m_RSkillLevel>=2))
            m_RLevelUpButton.gameObject.SetActive(false);
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
    public void SetPlayerName(string Name)
    {
        m_PlayerNameText.text=Name;
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
    public void ShowCastingUI()
    {
        m_CastingUI.gameObject.SetActive(true);
    }
    public void HideCastingUI()
    {
        m_CastingUI.gameObject.SetActive(false);
    }
    public void ShowCastingTime()
    {
        m_CastingTimeText.gameObject.SetActive(true);
    }
    public void HideCastingTime()
    {
        m_CastingTimeText.gameObject.SetActive(false);
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
        if(m_Character.m_QSkillLevel<5)
            m_QLevelUpButton.gameObject.SetActive(true);
        if(m_Character.m_WSkillLevel<5)
            m_WLevelUpButton.gameObject.SetActive(true);
        if(m_Character.m_ESkillLevel<5)
            m_ELevelUpButton.gameObject.SetActive(true);
        if((m_Character.m_CurrentLevel>=6 && m_Character.m_RSkillLevel<1) || (m_Character.m_CurrentLevel>=11 && m_Character.m_RSkillLevel<2) || 
            (m_Character.m_CurrentLevel>=16 && m_Character.m_RSkillLevel<3))    
            m_RLevelUpButton.gameObject.SetActive(true);
    }
    public void HideLevelUpSkillButtons()
    {
        m_QLevelUpButton.gameObject.SetActive(false);
        m_WLevelUpButton.gameObject.SetActive(false);
        m_ELevelUpButton.gameObject.SetActive(false);
        m_RLevelUpButton.gameObject.SetActive(false);
    }

    //GETTERS & SETTERS
    public void SetPlayer(CharacterMaster Player)
    {
        m_Character=Player;
    }
    public void SetCastingUIAbilityText(string Text)
    {
        m_CastingAbilityText.text=Text;
    }
}
