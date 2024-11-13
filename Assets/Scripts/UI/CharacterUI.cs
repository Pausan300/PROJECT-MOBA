using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    CharacterMaster m_Character;

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

    [Header("HEALTH MANA BARS")]
    public Slider m_HealthBar;
    public Slider m_ManaBar;
    public TextMeshProUGUI m_HealthText;
    public TextMeshProUGUI m_ManaText;

    [Header("INGAME PLAYER INFO")]
    public GameObject m_WorldCanvas;
    public Slider m_IngameHealthBar;
    public Slider m_IngameManaBar;
    public TextMeshProUGUI m_IngameLevelText;
    public TextMeshProUGUI m_PlayerNameText;
    public GameObject m_DamageNumbers;
    public Vector3 m_DamageNumbersPosOffset;
    public TMP_FontAsset m_PhysDamageFont;
    public TMP_FontAsset m_MagicDamageFont;

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

    [Header("BUFFS/DEBUFFS")]
    public GameObject m_BuffUIPrefab;
    public RectTransform m_BuffsDebuffsParent;
    public List<BuffDebuffObjectUI> m_BuffDebuffUIList=new List<BuffDebuffObjectUI>();

    private void Start()
    {
        HideSeconStatsPanel();
        HideCastingUI();
        HideCdTexts();
    }
	private void Update()
	{
		UpdatePowerUI(m_Character.m_QSkill.GetTimer(), m_Character.m_QSkill.GetCd(), m_QSkillCdImage, m_QSkillCdText, m_Character.m_QSkill.GetZeroCooldown());
		UpdatePowerUI(m_Character.m_WSkill.GetTimer(), m_Character.m_WSkill.GetCd(), m_WSkillCdImage, m_WSkillCdText, m_Character.m_WSkill.GetZeroCooldown());
		UpdatePowerUI(m_Character.m_ESkill.GetTimer(), m_Character.m_ESkill.GetCd(), m_ESkillCdImage, m_ESkillCdText, m_Character.m_ESkill.GetZeroCooldown());
		UpdatePowerUI(m_Character.m_RSkill.GetTimer(), m_Character.m_RSkill.GetCd(), m_RSkillCdImage, m_RSkillCdText, m_Character.m_RSkill.GetZeroCooldown());
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
    public void UpdatePowerUI(float PowerTimer, float PowerCd, Image PowerImage, TextMeshProUGUI PowerCdText, bool ZeroCd)
    {
        if(ZeroCd)
        {
			PowerCdText.text="";
		    PowerImage.fillAmount=0.0f;
        }
        else
        {
		    if(PowerTimer>=1.0f)
			    PowerCdText.text=PowerTimer.ToString("f0");
            else if(PowerTimer<=0.0f)
			    PowerCdText.text="";
		    else
			    PowerCdText.text=PowerTimer.ToString("f1");
		    PowerImage.fillAmount=PowerTimer/PowerCd;
        }
	}
    public void LevelUpQSkill()
    {
        m_QLevelPoints.value+=1;
        m_Character.m_CharacterStats.SetSkillPoints(m_Character.m_CharacterStats.GetSkillPoints()-1);   
        m_Character.m_QSkill.LevelUp();
        if(m_Character.m_QSkill.GetLevel()>=5)
            m_QLevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_CharacterStats.GetSkillPoints()<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpWSkill()
    {
        m_WLevelPoints.value+=1;
        m_Character.m_CharacterStats.SetSkillPoints(m_Character.m_CharacterStats.GetSkillPoints()-1);
        m_Character.m_WSkill.LevelUp();
        if(m_Character.m_WSkill.GetLevel()>=5)
            m_WLevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_CharacterStats.GetSkillPoints()<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpESkill()
    {
        m_ELevelPoints.value+=1;
        m_Character.m_CharacterStats.SetSkillPoints(m_Character.m_CharacterStats.GetSkillPoints()-1);
        m_Character.m_ESkill.LevelUp();
        if(m_Character.m_ESkill.GetLevel()>=5)
            m_ELevelUpButton.gameObject.SetActive(false);
        if(m_Character.m_CharacterStats.GetSkillPoints()<=0)
            HideLevelUpSkillButtons();
    }
    public void LevelUpRSkill()
    {
        m_RLevelPoints.value+=1;
        m_Character.m_CharacterStats.SetSkillPoints(m_Character.m_CharacterStats.GetSkillPoints()-1);
        m_Character.m_RSkill.LevelUp();
        if(m_Character.m_RSkill.GetLevel()>=3 || (m_Character.m_CharacterStats.GetCurrentLevel()<11 && m_Character.m_RSkill.GetLevel()>=1) || 
            (m_Character.m_CharacterStats.GetCurrentLevel()<16 && m_Character.m_RSkill.GetLevel()>=2))
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
    public void SetPlayerName(string Name)
    {
        m_PlayerNameText.text=Name;
    }
    public void SpawnDamageNumbers(float PhysDamage, float MagicDamage)
    {
        Vector3 l_PosOffset=m_DamageNumbersPosOffset;
        if(PhysDamage>0.0f)
        {
            GameObject l_PhysDamageText=Instantiate(m_DamageNumbers, m_WorldCanvas.transform);
            l_PhysDamageText.GetComponent<RectTransform>().localPosition=Vector3.zero+l_PosOffset;
            l_PosOffset.y-=0.5f;
            TextMeshProUGUI l_TextMesh=l_PhysDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font=m_PhysDamageFont;
            l_TextMesh.text=PhysDamage.ToString("f0");
        }
        if(MagicDamage>0.0f)
        {
            GameObject l_MagicDamageText=Instantiate(m_DamageNumbers, m_WorldCanvas.transform);
            l_MagicDamageText.GetComponent<RectTransform>().localPosition=Vector3.zero+l_PosOffset;
            TextMeshProUGUI l_TextMesh=l_MagicDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font=m_MagicDamageFont;
            l_TextMesh.text=MagicDamage.ToString("f0");
        }
    }
    public void CreateBuffObject(TimedBuff TimedBuff)
    {
        GameObject l_BuffObject=Instantiate(m_BuffUIPrefab, m_BuffsDebuffsParent);
        BuffDebuffObjectUI l_BuffObjectUI=l_BuffObject.GetComponent<BuffDebuffObjectUI>();
        l_BuffObjectUI.m_BuffImage.sprite=TimedBuff.m_Buff.m_BuffSprite;
        l_BuffObjectUI.m_BuffDurationImage.sprite=TimedBuff.m_Buff.m_BuffSprite;
        l_BuffObjectUI.m_BuffDurationImage.fillAmount=1.0f;
        l_BuffObjectUI.m_TimedBuff=TimedBuff;
        m_BuffDebuffUIList.Add(l_BuffObjectUI);
    }
    public void DeleteBuffObject(TimedBuff TimedBuff)
    {
        foreach(BuffDebuffObjectUI BuffDebuffUI in m_BuffDebuffUIList)
        {
            if(BuffDebuffUI.m_TimedBuff==TimedBuff)
            {
                m_BuffDebuffUIList.Remove(BuffDebuffUI);
                Destroy(BuffDebuffUI.gameObject);
                break;
            }
        }
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
        if(m_Character.m_QSkill.GetLevel()<5)
            m_QLevelUpButton.gameObject.SetActive(true);
        if(m_Character.m_WSkill.GetLevel()<5)
            m_WLevelUpButton.gameObject.SetActive(true);
        if(m_Character.m_ESkill.GetLevel()<5)
            m_ELevelUpButton.gameObject.SetActive(true);
        if((m_Character.m_CharacterStats.GetCurrentLevel()>=6 && m_Character.m_RSkill.GetLevel()<1) || (m_Character.m_CharacterStats.GetCurrentLevel()>=11 && m_Character.m_RSkill.GetLevel()<2) || 
            (m_Character.m_CharacterStats.GetCurrentLevel()>=16 && m_Character.m_RSkill.GetLevel()<3))    
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
