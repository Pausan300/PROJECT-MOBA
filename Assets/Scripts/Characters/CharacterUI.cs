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

    void Start()
    {
        HideSeconStatsPanel();
    }
    void Update()
    {
        
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
        m_LifestealText.text=Lifesteal.ToString();
        m_AttackRangeText.text=AttackRange.ToString();
        m_HealsShieldsPowerText.text=HealsShieldsPower.ToString();
        m_MagicPenText.text=MagicPenFix.ToString()+"|"+MagicPenPct.ToString()+"%";
        m_OmnidrainText.text=Omnidrain.ToString();
        m_TenacityText.text=Tenacity.ToString();
    }
    public void HideSeconStatsPanel()
    {
        m_SeconStatsPanel.gameObject.SetActive(false);
    }
    public void ShowSeconStatsPanel()
    {
        m_SeconStatsPanel.gameObject.SetActive(true);
    }
}
