using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
    public CharacterBaseStatsBlock m_TowerBaseStats;

    float m_MaxHealth;
    float m_CurrentHealth;
    float m_AttackDamage;
    float m_AbilityPower;
    float m_AttackSpeed;
    float m_AttackRange;
    float m_Armor;
    float m_MagicResistance;

    void Awake()
    {
        SetInitStats();
    }
    void Update()
    {

    }
    public void SetInitStats()
    {
        m_MaxHealth = m_TowerBaseStats.m_BaseHealth;
        m_AttackDamage = m_TowerBaseStats.m_BaseAttackDamage;
        m_AttackSpeed = m_TowerBaseStats.m_BaseAttackSpeed;
        m_AttackRange = m_TowerBaseStats.m_AttackRange;
        m_Armor = m_TowerBaseStats.m_BaseArmor;
        m_MagicResistance = m_TowerBaseStats.m_BaseMagicResist;

        m_CurrentHealth = m_MaxHealth;
    }

    //GETTERS & SETTERS
    public Sprite GetCharacterIcon()
    {
        return m_TowerBaseStats.m_CharacterIcon;
    }

    public float GetAttackDamage()
    {
        return m_AttackDamage;
    }
    public void AddAttackDamage(float Damage)
    {
        m_AttackDamage += Damage;
    }
    public float GetAbilityPower()
    {
        return m_AbilityPower;
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
}
