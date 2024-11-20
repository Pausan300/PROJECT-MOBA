using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterBaseStatsBlock m_CharacterBaseStats;

    float m_MaxHealth;
    float m_MaxMana;
    float m_CurrentHealth;
    float m_CurrentMana;
    float m_AttackDamage;
    float m_AbilityPower;
    float m_AttackSpeed;
    float m_CooldownReduction;
    float m_CriticalChance;
    float m_CriticalDamage;
    float m_Armor;
    float m_MagicResistance;
    float m_Tenacity;
    float m_ArmorPenetrationFixed;
    float m_ArmorPenetrationPct;
    float m_MagicPenetrationFixed;
    float m_MagicPenetrationPct;
    float m_HealthRegen;
    float m_ManaRegen;
    float m_LifeSteal;
    float m_OmniDrain;
    float m_ShieldsAndHealsPower;
    float m_MovementSpeed;
    
    float m_HealthBonus;
    float m_ManaBonus;
    float m_AttackDamageBonus;
    float m_AttackSpeedBonus;
    float m_ArmorBonus;
    float m_MagicResistBonus;
    float m_HealthRegenBonus;
    float m_ManaRegenBonus;
    float m_MoveSpeedBonusFlat;
    float m_MoveSpeedBonusAddi;
    float m_MoveSpeedBonusMulti;
    Dictionary<string, float> m_MoveSpeedBonusMultiBuffs=new Dictionary<string, float>();

    int m_CurrentLevel;
    int m_SkillPoints;
    float m_CurrentExp;

    private void Awake()
    {
        SetInitStats();
    }
	private void Update()
	{
		UpdateMovement();
        ResourceRestoring();
	}
	public void UpdateMovement()
    {
        m_MovementSpeed=m_CharacterBaseStats.m_BaseMovementSpeed+m_MoveSpeedBonusFlat;
        m_MovementSpeed*=1.0f+(m_MoveSpeedBonusAddi/100.0f);
        if(m_MoveSpeedBonusMulti!=0.0f)
            m_MovementSpeed*=m_MoveSpeedBonusMulti;
    }
    public void SetInitStats()
    {
        m_MaxHealth=m_CharacterBaseStats.m_BaseHealth;
        m_MaxMana=m_CharacterBaseStats.m_BaseMana;
        m_AttackDamage=m_CharacterBaseStats.m_BaseAttackDamage;
        m_AttackSpeed=m_CharacterBaseStats.m_BaseAttackSpeed;
        m_Armor=m_CharacterBaseStats.m_BaseArmor;
        m_MagicResistance=m_CharacterBaseStats.m_BaseMagicResist;
        m_HealthRegen=m_CharacterBaseStats.m_BaseHealthRegen;
        m_ManaRegen=m_CharacterBaseStats.m_BaseManaRegen;
        m_MovementSpeed=m_CharacterBaseStats.m_BaseMovementSpeed;

        m_HealthBonus=0.0f;
        m_ManaBonus=0.0f;
        m_AttackDamageBonus=0.0f;
        m_AttackSpeedBonus=0.0f;
        m_ArmorBonus=0.0f;
        m_MagicResistBonus=0.0f;
        m_ManaRegenBonus=0.0f;
        m_HealthRegenBonus=0.0f;
        m_MoveSpeedBonusFlat=0.0f;
        m_MoveSpeedBonusAddi=0.0f;
        m_MoveSpeedBonusMulti=0.0f;

        m_CurrentHealth=m_MaxHealth;
        m_CurrentMana=m_MaxMana;
        m_CurrentLevel=1;
        m_CurrentExp=0.0f;
        m_SkillPoints=1;
    }
    public void LevelUp()
    {
        m_CurrentExp-=m_CharacterBaseStats.m_ExpPerLevel[m_CurrentLevel];
        if(m_CurrentExp<0.0f)
            m_CurrentExp=0.0f;
        m_CurrentLevel++;
        m_SkillPoints++;
        RecalculateStat(m_MaxHealth, out m_MaxHealth, m_CurrentHealth, out m_CurrentHealth, m_CharacterBaseStats.m_BaseHealth, m_CharacterBaseStats.m_HealthPerLevel, m_HealthBonus);
        RecalculateStat(m_MaxMana, out m_MaxMana, m_CurrentMana, out m_CurrentMana, m_CharacterBaseStats.m_BaseMana, m_CharacterBaseStats.m_ManaPerLevel, m_ManaBonus);
        RecalculateStat(out m_AttackDamage, m_CharacterBaseStats.m_BaseAttackDamage, m_CharacterBaseStats.m_AttackDamagePerLevel, m_AttackDamageBonus);
        RecalculateStat(out m_AttackSpeed, m_CharacterBaseStats.m_BaseAttackSpeed, m_CharacterBaseStats.m_AttackSpeedPerLevel, m_AttackSpeedBonus);
        RecalculateStat(out m_Armor, m_CharacterBaseStats.m_BaseArmor, m_CharacterBaseStats.m_ArmorPerLevel, m_ArmorBonus);
        RecalculateStat(out m_MagicResistance, m_CharacterBaseStats.m_BaseMagicResist, m_CharacterBaseStats.m_MagicResistPerLevel, m_MagicResistBonus);
        RecalculateStat(out m_HealthRegen, m_CharacterBaseStats.m_BaseHealthRegen, m_CharacterBaseStats.m_HealthRegenPerLevel, m_HealthRegenBonus);
        RecalculateStat(out m_ManaRegen, m_CharacterBaseStats.m_BaseManaRegen, m_CharacterBaseStats.m_ManaRegenPerLevel, m_ManaRegenBonus);
    }
    void RecalculateStat(float Stat, out float StatRef, float Current, out float CurrentRef, float BaseStat, float LevelIncr, float Bonus)
    {
        float l_InitMaxStat=Stat;
        StatRef=BaseStat+Bonus+LevelIncr*(m_CurrentLevel-1.0f)*(0.7025f+0.0175f*(m_CurrentLevel-1.0f));
        float l_Difference=StatRef-l_InitMaxStat;
        CurrentRef=Current+l_Difference;
    }
    void RecalculateStat(out float StatRef, float BaseStat, float LevelIncr, float Bonus)
    {
        StatRef=BaseStat+Bonus+LevelIncr*(m_CurrentLevel-1.0f)*(0.7025f+0.0175f*(m_CurrentLevel-1.0f));
    }
    public void ResourceRestoring()
    {
        if(m_CurrentMana<m_MaxMana)
        {
            m_CurrentMana+=m_ManaRegen/5.0f*Time.deltaTime;
            if(m_CurrentMana>m_MaxMana)
                m_CurrentMana=m_MaxMana;
        }
        if(m_CurrentHealth<m_MaxHealth)
        {
            m_CurrentHealth+=m_HealthRegen/5.0f*Time.deltaTime;
            if(m_CurrentHealth>m_MaxHealth)
                m_CurrentHealth=m_MaxHealth;
        }
    }
    
    //GETTERS & SETTERS
    public string GetPlayerName()
    {
        return m_CharacterBaseStats.m_PlayerName;
    }
    public int GetCurrentLevel()
    {
        return m_CurrentLevel;
    }
    public float GetCurrentExp()
    {
        return m_CurrentExp;
    }
    public void SetCurrentExp(float Exp)
    {
        m_CurrentExp=Exp;
    }
    public int GetSkillPoints()
    {
        return m_SkillPoints;
    }
    public void SetSkillPoints(int Points)
    {
        m_SkillPoints=Points;
    }
    public float GetAttackDamage()
    {
        return m_AttackDamage;
    }
    public float GetBonusAttackDamage()
    {
        return m_AttackDamageBonus;
    }
    public float GetAbilityPower()
    {
        return m_AbilityPower;
    }
    public float GetArmorPenFixed()
    {
        return m_ArmorPenetrationFixed;
    }
    public float GetArmorPenPct()
    {
        return m_ArmorPenetrationPct;
    }
    public float GetMagicPenFixed()
    {
        return m_MagicPenetrationFixed;
    }
    public float GetMagicPenPct()
    {
        return m_MagicPenetrationPct;
    }
    public float GetAttackSpeed()
    {
        return m_AttackSpeed;
    }
    public float GetAttackRange()
    {
        return m_CharacterBaseStats.m_AttackRange;
    }
    public float GetLifeSteal()
    {
        return m_LifeSteal;
    }
    public float GetOmniDrain()
    {
        return m_OmniDrain;
    }
    public float GetCritChance()
    {
        return m_CriticalChance;
    }
    public float GetCritDamage()
    {
        return m_CriticalDamage;
    }
    public float GetCdr()
    {
        return m_CooldownReduction;
    }
    public float GetMaxHealth()
    {
        return m_MaxHealth;
    }
    public void SetMaxHealth(float Health)
    {
        m_MaxHealth=Health;
    }
    public float GetCurrentHealth()
    {
        return m_CurrentHealth;
    }
    public void SetCurrentHealth(float Health)
    {
        m_CurrentHealth=Health;
    }
    public float GetHealthRegen()
    {
        return m_HealthRegen;
    }
    public float GetMaxMana()
    {
        return m_MaxMana;
    }
    public float GetCurrentMana()
    {
        return m_CurrentMana;
    }
    public void SetCurrentMana(float Mana)
    {
        m_CurrentMana=Mana;
    }
    public float GetManaRegen()
    {
        return m_ManaRegen;
    }
    public float GetShieldsHealsPower()
    {
        return m_ShieldsAndHealsPower;
    }
    public float GetArmor()
    {
        return m_Armor;
    }
    public void SetArmor(float Armor)
    {
        m_Armor=Armor;
    }
    public float GetMagicRes()
    {
        return m_MagicResistance;
    }
    public void SetMagicRes(float MagicRes)
    {
        m_MagicResistance=MagicRes;
    }
    public float GetTenacity()
    {
        return m_Tenacity;
    }
    public float GetMovSpeed()
    {
        return m_MovementSpeed;
    }
    public float GetMovSpeedBonusFlat()
    {
        return m_MoveSpeedBonusFlat;
    }
    public void AddMovSpeedBonusFlat(float Bonus)
    {
        m_MoveSpeedBonusFlat=Bonus;
    }
    public float GetMovSpeedBonusAddi()
    {
        return m_MoveSpeedBonusAddi;
    }
    public void AddMovSpeedBonusAddi(float Bonus)
    {
        m_MoveSpeedBonusAddi=Bonus;
    }
    public void AddMovSpeedBonusMulti(string Name, float Bonus)
    {
        m_MoveSpeedBonusMultiBuffs.Add(Name, Bonus);
        m_MoveSpeedBonusMulti=0.0f;
        for(int i=0; i<m_MoveSpeedBonusMultiBuffs.Count; ++i)
        {
            if(i==0)
                m_MoveSpeedBonusMulti=1+m_MoveSpeedBonusMultiBuffs.Values.ToList()[i]/100.0f;
            else
                m_MoveSpeedBonusMulti*=1+m_MoveSpeedBonusMultiBuffs.Values.ToList()[i]/100.0f;
        }
    }
    public void RemoveMovSpeedBonusMulti(string Name)
    {
        m_MoveSpeedBonusMultiBuffs.Remove(Name);
        m_MoveSpeedBonusMulti=0.0f;
        for(int i=0; i<m_MoveSpeedBonusMultiBuffs.Count; ++i)
        {
            if(i==0)
                m_MoveSpeedBonusMulti=1+m_MoveSpeedBonusMultiBuffs.Values.ToList()[i]/100.0f;
            else
                m_MoveSpeedBonusMulti*=1+m_MoveSpeedBonusMultiBuffs.Values.ToList()[i]/100.0f;
        }
    }
}
