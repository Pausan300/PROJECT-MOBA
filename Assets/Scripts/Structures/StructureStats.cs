using Unity.Netcode;
using UnityEngine;

public class StructureStats : MonoBehaviour
{
    public CharacterBaseStatsBlock m_StructureBaseStats;

    [Header("TEAM")]
    public CharacterStats.TeamType m_TeamType;

    float m_MaxHealth;
    float m_CurrentHealth;
    float m_AttackDamage; 
    float m_AbilityPower;
    float m_AttackSpeed;
    float m_AttackRange;
    float m_Armor;
    float m_MagicResistance;
    float m_HealthRegen;

    int m_CurrentLevel;


    void Awake()
    {
        SetInitStats();
    }
    private void Update()
    {
        ResourceRestoring();
    }

    public void SetInitStats()
    {
        m_MaxHealth = m_StructureBaseStats.m_BaseHealth;
        m_AttackDamage = m_StructureBaseStats.m_BaseAttackDamage;
        m_AttackSpeed = m_StructureBaseStats.m_BaseAttackSpeed;
        m_AttackRange = m_StructureBaseStats.m_AttackRange;
        m_Armor = m_StructureBaseStats.m_BaseArmor;
        m_MagicResistance = m_StructureBaseStats.m_BaseMagicResist;
        m_HealthRegen = m_StructureBaseStats.m_BaseHealthRegen;
        m_CurrentLevel=1;

        m_CurrentHealth = m_MaxHealth;
    }

    public void ResourceRestoring()
    {
        if(m_HealthRegen>0.0f && m_CurrentHealth<m_MaxHealth)
        {
            m_CurrentHealth += m_HealthRegen * Time.deltaTime;
            if (m_CurrentHealth > m_MaxHealth)
                m_CurrentHealth = m_MaxHealth;
        }
    }

    //GETTERS & SETTERS
    public Sprite GetCharacterIcon()
    {
        return m_StructureBaseStats.m_CharacterIcon;
    } 
    public int GetCurrentLevel()
    {
        return m_CurrentLevel;
    }
    public void SetCurrentLevel(int Level) 
    {
        m_CurrentLevel=Level;
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
