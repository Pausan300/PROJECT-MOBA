using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    CharacterMaster m_Character;

    [Header("HEALTH MANA BARS")]
    public Slider m_HealthBar;
    public Slider m_ManaBar;
    public TextMeshProUGUI m_HealthText;
    public TextMeshProUGUI m_ManaText;

    [Header("SKILLS")]
    public Image m_QSkillImage;
    public Image m_WSkillImage;
    public Image m_ESkillImage;
    public Image m_RSkillImage;
    public Image m_SumSpell1Image;
    public Image m_SumSpell2Image;

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

    [Header("BUFFS/DEBUFFS")]
    public GameObject m_BuffUIPrefab;
    public RectTransform m_BuffsDebuffsParent;
    public List<BuffDebuffObjectUI> m_BuffDebuffUIList=new List<BuffDebuffObjectUI>();

    [Header("CASTING UI")]
    public RectTransform m_CastingUI;
    public Slider m_CastingBar;
    public TextMeshProUGUI m_CastingAbilityText;
    public TextMeshProUGUI m_CastingTimeText;

    [Header("TARGET INFO UI")]
    public GameObject m_TargetInfoUI;
    CharacterStats m_TargetStats;
    public TextMeshProUGUI m_TargetAttackDamageText;
    public TextMeshProUGUI m_TargetArmorText;
    public TextMeshProUGUI m_TargetAttackSpeedText;
    public TextMeshProUGUI m_TargetCriticalChanceText;
    public TextMeshProUGUI m_TargetAbilityPowerText;
    public TextMeshProUGUI m_TargetMagicResistanceText;
    public TextMeshProUGUI m_TargetCooldownReductionText;
    public TextMeshProUGUI m_TargetMovementSpeedText;
    public TextMeshProUGUI m_TargetLevelText;
    public TextMeshProUGUI m_TargetHealthText;
    public TextMeshProUGUI m_TargetManaText;
    public Slider m_TargetHealthBar;
    public Slider m_TargetManaBar;
    public RectTransform m_TargetBuffsDebuffsParent;

    [Header("SKILL POPUP")]
    public PopupUI m_PopupUI;

    private void Start()
    {
        HideSeconStatsPanel();
        HideCastingUI();
        HideCdTexts();
        HideTargetInfoUI();
        HidePopup();
    }
	private void Update()
	{
        if(Input.GetKey(KeyCode.C))
            ShowSeconStatsPanel();
        else if(Input.GetKeyUp(KeyCode.C))
            HideSeconStatsPanel();

        if(m_TargetInfoUI.activeSelf)
            UpdateTargetInfoUI(m_TargetStats);
	}

	public void UpdateHealthManaBars(float Health, float MaxHealth, float Mana, float MaxMana)
    {
        float l_HealthRounded=Mathf.Round(Health);
        float l_ManaRounded=Mathf.Round(Mana);
        m_HealthBar.value=l_HealthRounded/MaxHealth;
        m_ManaBar.value=l_ManaRounded/MaxMana;
        m_HealthText.text=l_HealthRounded+"/"+Mathf.Round(MaxHealth);
        m_ManaText.text=l_ManaRounded+"/"+Mathf.Round(MaxMana);
        m_Character.m_IngameCharacterUI.UpdateHealthManaBars(l_HealthRounded, MaxHealth, l_ManaRounded, MaxMana);
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
    }
    public void UpdateCastingUI(float CurrentRecallTime, float MaxRecallTime)
    {
        m_CastingBar.value=CurrentRecallTime/MaxRecallTime;
        m_CastingTimeText.text=CurrentRecallTime.ToString("f1");
    }
    public void UpdateTargetInfoUI(CharacterStats Stats)
    {
        if(Stats!=null)
        {
            m_TargetAttackDamageText.text=Mathf.Round(Stats.GetAttackDamage()).ToString();
            m_TargetArmorText.text=Mathf.Round(Stats.GetArmor()).ToString();
            m_TargetAttackSpeedText.text=Stats.GetAttackSpeed().ToString("f2");
            m_TargetCriticalChanceText.text=Stats.GetCritChance().ToString();
            m_TargetAbilityPowerText.text=Stats.GetAbilityPower().ToString();
            m_TargetMagicResistanceText.text=Mathf.Round(Stats.GetMagicRes()).ToString();
            m_TargetCooldownReductionText.text=Stats.GetCdr().ToString();
            m_TargetMovementSpeedText.text=Stats.GetMovSpeed().ToString();
            float l_HealthRounded=Mathf.Round(Stats.GetCurrentHealth());
            float l_ManaRounded=Mathf.Round(Stats.GetCurrentMana());
            m_TargetHealthBar.value=l_HealthRounded/Stats.GetMaxHealth();
            m_TargetManaBar.value=l_ManaRounded/Stats.GetMaxMana();
            m_TargetHealthText.text=l_HealthRounded+"/"+Mathf.Round(Stats.GetMaxHealth());
            m_TargetManaText.text=l_ManaRounded+"/"+Mathf.Round(Stats.GetMaxMana());
            m_TargetLevelText.text=Stats.GetCurrentLevel().ToString();
        }
    }
    public void UpdatePowerUI(Power.PowerType Type, float PowerTimer, float PowerCd, bool ZeroCd)
    {
        TextMeshProUGUI l_PowerCdText=null;
        Image l_PowerCdImage=null;
        switch(Type)
        {
            case Power.PowerType.QSKILL:
                l_PowerCdText=m_QSkillCdText;
                l_PowerCdImage=m_QSkillCdImage;
                break;
            case Power.PowerType.WSKILL:
                l_PowerCdText=m_WSkillCdText;
                l_PowerCdImage=m_WSkillCdImage;
                break;
            case Power.PowerType.ESKILL:
                l_PowerCdText=m_ESkillCdText;
                l_PowerCdImage=m_ESkillCdImage;
                break;
            case Power.PowerType.RSKILL:
                l_PowerCdText=m_RSkillCdText;
                l_PowerCdImage=m_RSkillCdImage;
                break;
            case Power.PowerType.SUMMONER1:
                l_PowerCdText=m_SumSpell1CdText;
                l_PowerCdImage=m_SumSpell1CdImage;
                break;
            case Power.PowerType.SUMMONER2:
                l_PowerCdText=m_SumSpell2CdText;
                l_PowerCdImage=m_SumSpell2CdImage;
                break;
        }
        if(PowerTimer>=1.0f)
			l_PowerCdText.text=PowerTimer.ToString("f0");
        else if(PowerTimer<=0.0f)
			l_PowerCdText.text="";
		else
			l_PowerCdText.text=PowerTimer.ToString("f1");
		l_PowerCdImage.fillAmount=PowerTimer/PowerCd;
	}
    public void LevelUpQSkill()
    {
        m_QLevelPoints.value+=1;
        m_Character.m_CharacterStats.SetSkillPoints(m_Character.m_CharacterStats.GetSkillPoints()-1);   
        m_Character.SetQSkillLevelRpc();
        if(m_Character.GetQSkillLevel()>=5)
            m_QLevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_CharacterStats.GetSkillPoints()<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpWSkill()
    {
        m_WLevelPoints.value+=1;
        m_Character.m_CharacterStats.SetSkillPoints(m_Character.m_CharacterStats.GetSkillPoints()-1);
        m_Character.SetWSkillLevelRpc();
        if(m_Character.GetWSkillLevel()>=5)
            m_WLevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_CharacterStats.GetSkillPoints()<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpESkill()
    {
        m_ELevelPoints.value+=1;
        m_Character.m_CharacterStats.SetSkillPoints(m_Character.m_CharacterStats.GetSkillPoints()-1);
        m_Character.SetESkillLevelRpc();
        if(m_Character.GetESkillLevel()>=5)
            m_ELevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_CharacterStats.GetSkillPoints()<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpRSkill()
    {
        m_RLevelPoints.value+=1;
        m_Character.m_CharacterStats.SetSkillPoints(m_Character.m_CharacterStats.GetSkillPoints()-1);
        m_Character.SetRSkillLevelRpc();
        if(m_Character.GetRSkillLevel()>=3 || (m_Character.m_CharacterStats.GetCurrentLevel()<11 && m_Character.GetRSkillLevel()>=1) || 
            (m_Character.m_CharacterStats.GetCurrentLevel()<16 && m_Character.GetRSkillLevel()>=2))
            m_RLevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_CharacterStats.GetSkillPoints()<=0)
            HideLevelUpSkillButtons();
    }
    public void ResetSkillLevelPoints()
    {
        m_QLevelPoints.value=0;
        m_WLevelPoints.value=0;
        m_ELevelPoints.value=0;
        m_RLevelPoints.value=0;
    }
    public void CreateBuffObject(TimedBuff TimedBuff)
    {
        GameObject l_BuffObject=Instantiate(m_BuffUIPrefab, m_BuffsDebuffsParent);
        BuffDebuffObjectUI l_BuffObjectUI=l_BuffObject.GetComponent<BuffDebuffObjectUI>();
        l_BuffObjectUI.m_BuffImage.sprite=TimedBuff.m_Buff.m_BuffSprite;
        l_BuffObjectUI.m_BuffDurationImage.sprite=TimedBuff.m_Buff.m_BuffSprite;
        l_BuffObjectUI.m_BuffDurationImage.fillAmount=1.0f;
        l_BuffObjectUI.m_TimedBuff=TimedBuff;
        //m_BuffDebuffUIList.Add(l_BuffObjectUI);
    }
    //public void DeleteBuffObject(TimedBuff TimedBuff)
    //{
    //    foreach(BuffDebuffObjectUI BuffDebuffUI in m_BuffDebuffUIList)
    //    {
    //        if(BuffDebuffUI.m_TimedBuff==TimedBuff)
    //        {
    //            m_BuffDebuffUIList.Remove(BuffDebuffUI);
    //            Destroy(BuffDebuffUI.gameObject);
    //            break;
    //        }
    //    }
    //}
    public void SetPopupType(InspectableElementUI.PopupType PopupElement, string Description, string Name)
    {
		switch(PopupElement)
		{
            case InspectableElementUI.PopupType.PASSIVESKILL:
                m_PopupUI.UpdatePowerPopupInfo(m_Character.m_PassiveSkill.m_Description, m_Character.m_PassiveSkill.m_PowerName, "P",
                    null, null, m_Character.m_PassiveSkill.m_Sprite);
                break;
            case InspectableElementUI.PopupType.QSKILL:
                m_PopupUI.UpdatePowerPopupInfo(m_Character.m_QSkill.m_Description, m_Character.m_QSkill.m_PowerName, m_Character.m_QSkillKey.ToString(), 
                    m_Character.m_QSkill.GetCd().ToString(), m_Character.m_QSkill.GetMana(m_Character.GetQSkillLevel()).ToString(), m_Character.m_QSkill.m_Sprite);
			    break;
			case InspectableElementUI.PopupType.WSKILL:
                m_PopupUI.UpdatePowerPopupInfo(m_Character.m_WSkill.m_Description, m_Character.m_WSkill.m_PowerName, m_Character.m_WSkillKey.ToString(), 
                    m_Character.m_WSkill.GetCd().ToString(), m_Character.m_WSkill.GetMana(m_Character.GetWSkillLevel()).ToString(), m_Character.m_WSkill.m_Sprite);
			    break;
			case InspectableElementUI.PopupType.ESKILL:
                m_PopupUI.UpdatePowerPopupInfo(m_Character.m_ESkill.m_Description, m_Character.m_ESkill.m_PowerName, m_Character.m_ESkillKey.ToString(), 
                    m_Character.m_ESkill.GetCd().ToString(), m_Character.m_ESkill.GetMana(m_Character.GetESkillLevel()).ToString(), m_Character.m_ESkill.m_Sprite);
			    break;
			case InspectableElementUI.PopupType.RSKILL:
                m_PopupUI.UpdatePowerPopupInfo(m_Character.m_RSkill.m_Description, m_Character.m_RSkill.m_PowerName, m_Character.m_RSkillKey.ToString(), 
                    m_Character.m_RSkill.GetCd().ToString(), m_Character.m_RSkill.GetMana(m_Character.GetRSkillLevel()).ToString(), m_Character.m_RSkill.m_Sprite);
			    break;
			case InspectableElementUI.PopupType.SUMMONER1:
                m_PopupUI.UpdatePowerPopupInfo(m_Character.m_SummSpell1.m_Description, m_Character.m_SummSpell1.m_PowerName, m_Character.m_SummSpell1Key.ToString(), 
                    m_Character.m_SummSpell1.GetCd().ToString(), null, m_Character.m_SummSpell1.m_Sprite);
			    break;
			case InspectableElementUI.PopupType.SUMMONER2:
                m_PopupUI.UpdatePowerPopupInfo(m_Character.m_SummSpell2.m_Description, m_Character.m_SummSpell2.m_PowerName, m_Character.m_SummSpell2Key.ToString(), 
                    m_Character.m_SummSpell2.GetCd().ToString(), null, m_Character.m_SummSpell2.m_Sprite);
			    break;
            case InspectableElementUI.PopupType.STAT:
                m_PopupUI.UpdateStatPopupInfo(Description, Name);
                break;
		}
        ShowPopup();
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
    public void ShowTargetInfoUI(CharacterStats Stats)
    {
        m_TargetStats=Stats;
        m_TargetInfoUI.gameObject.SetActive(true);
    }
    public void HideTargetInfoUI()
    {
        m_TargetInfoUI.gameObject.SetActive(false);
        m_TargetStats=null;
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
        if(m_Character.GetQSkillLevel()<5)
            m_QLevelUpButton.gameObject.SetActive(true);
        if(m_Character.GetWSkillLevel()<5)
            m_WLevelUpButton.gameObject.SetActive(true);
        if(m_Character.GetESkillLevel()<5)
            m_ELevelUpButton.gameObject.SetActive(true);
        if((m_Character.m_CharacterStats.GetCurrentLevel()>=6 && m_Character.GetRSkillLevel()<1) || (m_Character.m_CharacterStats.GetCurrentLevel()>=11 && m_Character.GetRSkillLevel()<2) || 
            (m_Character.m_CharacterStats.GetCurrentLevel()>=16 && m_Character.GetRSkillLevel()<3))    
            m_RLevelUpButton.gameObject.SetActive(true);
    }
    public void HideLevelUpSkillButtons()
    {
        m_QLevelUpButton.gameObject.SetActive(false);
        m_WLevelUpButton.gameObject.SetActive(false);
        m_ELevelUpButton.gameObject.SetActive(false);
        m_RLevelUpButton.gameObject.SetActive(false);
    }
    public void ShowPopup()
    {
        m_PopupUI.gameObject.SetActive(true);
    }
    public void HidePopup()
    {
        m_PopupUI.gameObject.SetActive(false);
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
    public void SetPowersImages(Sprite QSprite, Sprite WSprite, Sprite ESprite, Sprite RSprite, Sprite Summ1Sprite, Sprite Summ2Sprite)
    {
        m_QSkillImage.sprite=QSprite;
        m_QSkillCdImage.sprite=QSprite;
        m_WSkillImage.sprite=WSprite;
        m_WSkillCdImage.sprite=WSprite;
        m_ESkillImage.sprite=ESprite;
        m_ESkillCdImage.sprite=ESprite;
        m_RSkillImage.sprite=RSprite;
        m_RSkillCdImage.sprite=RSprite;
        m_SumSpell1Image.sprite=Summ1Sprite;
        m_SumSpell1CdImage.sprite=Summ1Sprite;
        m_SumSpell2Image.sprite=Summ2Sprite;
        m_SumSpell2CdImage.sprite=Summ2Sprite;
    }
}
