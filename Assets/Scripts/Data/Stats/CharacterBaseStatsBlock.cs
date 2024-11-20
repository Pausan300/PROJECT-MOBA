using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="CharacterStats/BaseStatsBlock")]
public class CharacterBaseStatsBlock : ScriptableObject
{
    [Header("PLAYER INFO")]
    public string m_PlayerName;

    [Header("BASE STATS")]
    public float m_BaseHealth;
    public float m_BaseMana;
    public float m_BaseAttackDamage;
    public float m_BaseAttackSpeed;
    public float m_BaseArmor;
    public float m_BaseMagicResist;
    public float m_BaseHealthRegen;
    public float m_BaseManaRegen;
    public float m_AttackRange;
    public float m_BaseMovementSpeed;
    
    [Header("GROWTH STATS")]
    public float m_HealthPerLevel;
    public float m_ManaPerLevel;
    public float m_AttackDamagePerLevel;
    public float m_AttackSpeedPerLevel;
    public float m_ArmorPerLevel;
    public float m_MagicResistPerLevel;
    public float m_HealthRegenPerLevel;
    public float m_ManaRegenPerLevel;
    public List<float> m_ExpPerLevel;

    //GETTERS & SETTERS
    public string GetPlayerName()
    {
        return m_PlayerName;
    }
}
