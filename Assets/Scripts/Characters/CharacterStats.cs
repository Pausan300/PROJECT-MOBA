using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CharacterStats : NetworkBehaviour
{
    public CharacterBaseStatsBlock m_CharacterBaseStats;

    float m_MaxHealth;
    float m_MaxMana;
    float m_CurrentHealth;
    float m_CurrentMana;
    float m_AttackDamage;
    float m_AbilityPower;
    float m_AttackSpeed;
    float m_AttackRange;
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
    Dictionary<string, float> m_MoveSpeedBonusMultiBuffs = new Dictionary<string, float>();

    float m_CorruptedHealth;
    float m_CorruptedHealthDamage;

    int m_CurrentLevel;
    int m_SkillPoints;
    float m_CurrentExp;
    int m_Gold;
    int m_Crystals;

    [Header("TEAM")]
    public TeamType m_TeamType;
    public enum TeamType 
    {
        ENEMY,
        ALLY
    }

    [Header("Enemys")]
    public EnemyType m_EnemyType;
    public enum EnemyType
    {
        LARGEMONSTER,
        MINION,
        LIGHTLESS,
        LEGENDARY,
        PLAYER
    }

    [Header("Buffs")]
    bool m_ImmuneCC = false;
    bool m_Stuned = false;
    bool m_Immobilized = false;
    bool m_Scared = false;
    Vector3 m_FearPosition;
    bool m_CanSoulTheft = false;
    WilldurrCharacterController m_Willdurr; // Willdur esta aqui para el buff de SoulThedtBuff. Es necessario que sea una variable.
    float m_Omnivamp = 0;

    void Awake()
    {
        SetInitStats();
    }
    void Update()
    {
        UpdateMovementRpc();
        ResourceRestoringRpc();
    }
    public void SetInitStats()
    {
        m_MaxHealth = m_CharacterBaseStats.m_BaseHealth;
        m_MaxMana = m_CharacterBaseStats.m_BaseMana;
        m_AttackDamage = m_CharacterBaseStats.m_BaseAttackDamage;
        m_AttackSpeed = m_CharacterBaseStats.m_BaseAttackSpeed;
        m_AttackRange = m_CharacterBaseStats.m_AttackRange;
        m_Armor = m_CharacterBaseStats.m_BaseArmor;
        m_MagicResistance = m_CharacterBaseStats.m_BaseMagicResist;
        m_HealthRegen = m_CharacterBaseStats.m_BaseHealthRegen;
        m_ManaRegen = m_CharacterBaseStats.m_BaseManaRegen;
        m_MovementSpeed = m_CharacterBaseStats.m_BaseMovementSpeed;

        m_HealthBonus = 0.0f;
        m_ManaBonus = 0.0f;
        m_AttackDamageBonus = 0.0f;
        m_AttackSpeedBonus = 0.0f;
        m_ArmorBonus = 0.0f;
        m_MagicResistBonus = 0.0f;
        m_ManaRegenBonus = 0.0f;
        m_HealthRegenBonus = 0.0f;
        m_MoveSpeedBonusFlat = 0.0f;
        m_MoveSpeedBonusAddi = 0.0f;
        m_MoveSpeedBonusMulti = 0.0f;

        m_CurrentHealth = m_MaxHealth;
        m_CurrentMana = m_MaxMana;
        m_CurrentLevel = 1;
        m_CurrentExp = 0.0f;
        m_SkillPoints = 1;

        m_Gold=0;
        m_Crystals=0;
    }
    public void LevelUp()
    {
        m_CurrentExp -= m_CharacterBaseStats.m_ExpPerLevel[m_CurrentLevel];
        if (m_CurrentExp < 0.0f)
            m_CurrentExp = 0.0f;
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
        float l_InitMaxStat = Stat;
        StatRef = BaseStat + Bonus + LevelIncr * (m_CurrentLevel - 1.0f) * (0.7025f + 0.0175f * (m_CurrentLevel - 1.0f));
        float l_Difference = StatRef - l_InitMaxStat;
        CurrentRef = Current + l_Difference;
    }
    void RecalculateStat(out float StatRef, float BaseStat, float LevelIncr, float Bonus)
    {
        StatRef = BaseStat + Bonus + LevelIncr * (m_CurrentLevel - 1.0f) * (0.7025f + 0.0175f * (m_CurrentLevel - 1.0f));
    }
    [Rpc(SendTo.Everyone)]
    public void UpdateMovementRpc()
    {
        m_MovementSpeed = m_CharacterBaseStats.m_BaseMovementSpeed + m_MoveSpeedBonusFlat;
        m_MovementSpeed *= 1.0f + (m_MoveSpeedBonusAddi / 100.0f);
        if (m_MoveSpeedBonusMulti != 0.0f)
            m_MovementSpeed *= m_MoveSpeedBonusMulti;
    }
    [Rpc(SendTo.Everyone)]
    public void ResourceRestoringRpc()
    {
        if (m_CurrentMana < m_MaxMana)
        {
            m_CurrentMana += m_ManaRegen / 5.0f * Time.deltaTime;
            if (m_CurrentMana > m_MaxMana)
                m_CurrentMana = m_MaxMana;
        }
        if (m_CurrentHealth < m_MaxHealth)
        {
            m_CurrentHealth += m_HealthRegen / 5.0f * Time.deltaTime;
            if (m_CurrentHealth > m_MaxHealth)
                m_CurrentHealth = m_MaxHealth;
        }
        if(m_CorruptedHealth > 0.0f) 
        {
            m_CorruptedHealth-=m_CorruptedHealthDamage*Time.deltaTime;
            if(m_CorruptedHealth<0.0f)
                m_CorruptedHealth=0.0f;
        }
    }

    //GETTERS & SETTERS
    public string GetPlayerName()
    {
        return m_CharacterBaseStats.m_PlayerName;
    }
    public Sprite GetCharacterIcon()
    {
        return m_CharacterBaseStats.m_CharacterIcon;
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
        m_CurrentExp = Exp;
    }
    public int GetGold() 
    {
        return m_Gold;
    }
    public void SetGold(int Gold) 
    {
        m_Gold=Gold;
    }
    public int GetCrystals() 
    {
        return m_Crystals;
    }
    public void SetCrystals(int Crystals) 
    {
        m_Crystals=Crystals;
    }
    public int GetSkillPoints()
    {
        return m_SkillPoints;
    }
    public void SetSkillPoints(int Points)
    {
        m_SkillPoints = Points;
    }
    public float GetAttackDamage()
    {
        return m_AttackDamage;
    }
    public void AddAttackDamage(float Damage)
    {
        m_AttackDamage += Damage;
    }
    public float GetBonusAttackDamage()
    {
        return m_AttackDamageBonus;
    }
    public void SetBonusAttackDamage(float Bonus) 
    {
        m_AttackDamageBonus=Bonus;
        RecalculateStat(out m_AttackDamage, m_CharacterBaseStats.m_BaseAttackDamage, m_CharacterBaseStats.m_AttackDamagePerLevel, m_AttackDamageBonus);
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
        return m_AttackRange;
    }
    public void SetAttackRange(float Range)
    {
        m_AttackRange = Range;
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
        m_MaxHealth = Health;
    }
    public float GetCurrentHealth()
    {
        return m_CurrentHealth;
    }
    [Rpc(SendTo.Everyone)]
    public void SetCurrentHealthRpc(float Health)
    {
        m_CurrentHealth = Health;
    }
    public float GetCorruptedHealth() 
    {
        return m_CorruptedHealth;
    }
    public void SetCorruptedHealth(float Health) 
    {
        m_CorruptedHealth=Health;
    }
    public void SetCorruptedHealthDamage(float Damage) 
    {
        m_CorruptedHealthDamage=Damage;
    }
    public float GetHealthRegen()
    {
        return m_HealthRegen;
    }
    public float GetBonusHealth()
    {
        return m_HealthBonus;
    }
    public float GetMaxMana()
    {
        return m_MaxMana;
    }
    public float GetCurrentMana()
    {
        return m_CurrentMana;
    }
    [Rpc(SendTo.Everyone)]
    public void SetCurrentManaRpc(float Mana)
    {
        m_CurrentMana = Mana;
        if(m_CurrentMana>m_MaxMana)
            m_CurrentMana=m_MaxMana;
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
        m_Armor = Armor;
    }
    public float GetMagicRes()
    {
        return m_MagicResistance;
    }
    public void SetMagicRes(float MagicRes)
    {
        m_MagicResistance = MagicRes;
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
    public void SetMovSpeedBonusFlat(float Bonus)
    {
        m_MoveSpeedBonusFlat = Bonus;
    }
    public float GetMovSpeedBonusAddi()
    {
        return m_MoveSpeedBonusAddi;
    }
    public void SetMovSpeedBonusAddi(float Bonus)
    {
        m_MoveSpeedBonusAddi = Bonus;
    }
    public void AddMovSpeedBonusMulti(string Name, float Bonus)
    {
        if(m_MoveSpeedBonusMultiBuffs.ContainsKey(Name)) 
            m_MoveSpeedBonusMultiBuffs[Name]+=Bonus;
        else
            m_MoveSpeedBonusMultiBuffs.Add(Name, Bonus);

        m_MoveSpeedBonusMulti = 0.0f;
        for (int i = 0; i < m_MoveSpeedBonusMultiBuffs.Count; ++i)
        {
            if (i == 0)
                m_MoveSpeedBonusMulti = 1 + m_MoveSpeedBonusMultiBuffs.Values.ToList()[i] / 100.0f;
            else
                m_MoveSpeedBonusMulti *= 1 + m_MoveSpeedBonusMultiBuffs.Values.ToList()[i] / 100.0f;
        }
    }
    public void RemoveMovSpeedBonusMulti(string Name)
    {
        m_MoveSpeedBonusMultiBuffs.Remove(Name);
        m_MoveSpeedBonusMulti = 0.0f;
        for (int i = 0; i < m_MoveSpeedBonusMultiBuffs.Count; ++i)
        {
            if (i == 0)
                m_MoveSpeedBonusMulti = 1 + m_MoveSpeedBonusMultiBuffs.Values.ToList()[i] / 100.0f;
            else
                m_MoveSpeedBonusMulti *= 1 + m_MoveSpeedBonusMultiBuffs.Values.ToList()[i] / 100.0f;
        }
    }
    public bool IsMovSpeedBonusMultiApplied(string Name) 
    {
        return m_MoveSpeedBonusMultiBuffs.ContainsKey(Name);
    }

    public EnemyType GetEnemyType()
    {
        return m_EnemyType;
    }
    public void SetEnemyType(EnemyType EnemyType)
    {
        m_EnemyType = EnemyType;
    }
    public bool GetStuned()
    {
        return m_Stuned;
    }
    public void SetStuned(bool Stuned)
    {
        m_Stuned = Stuned;
    }
    public bool GetImmobilized()
    {
        return m_Immobilized;
    }
    public void SetImmobilized(bool Immobilized)
    {
        m_Immobilized = Immobilized;
    }

    public bool GetScared()
    {
        return m_Scared;
    }
    public Vector3 GetFearPos()
    {
        return m_FearPosition;
    }
    public void SetScared(bool Scared, Vector3 FearPosition)
    {
        m_FearPosition = FearPosition;
        m_Scared = Scared;
    }
    public bool GetCanSoulTheft()
    {
        return m_CanSoulTheft;
    }
    public WilldurrCharacterController GetWilldurrCharacterController()
    {
        return m_Willdurr;
    }
    public void SetCanSoulTheft(bool CanSoulTheft, WilldurrCharacterController Willdurr)
    {
        m_CanSoulTheft = CanSoulTheft;
        m_Willdurr = Willdurr;
    }
    public float GetOmnivamp()
    {
        return m_Omnivamp;
    }
    public void SetOmnivamp(float Omnivamp)
    {
        m_Omnivamp = Omnivamp;
    }
    public void AddOmnivamp(float Omnivamp)
    {
        m_Omnivamp += Omnivamp;
    }
    public bool GetImmuneCC()
    {
        return m_ImmuneCC;
    }
    public void SetImmuneCC(bool ImmuneCC)
    {
        m_ImmuneCC = ImmuneCC;
    }
}
