
using UnityEngine;

public interface ITakeDamage
{
	void TakeDamage(DamageInstance Instance);
	CharacterStats GetCharacterStats();
	TowerController GetNearTower();
	void SetNearTower(TowerController Tower);
}
public interface ITakeDamageStructure 
{
	void TakeDamage(CharacterStats Stats);
	StructureStats GetStructureStats();
	bool GetIsDestroyed();
}

public class DamageInstance
{
    public enum DamageType 
    {
        PHYSICAL,
        MAGIC
    }
    public float m_PhysDamage;
    public float m_MagicDamage;
    public bool m_IgnoreResistances;
    public string m_Id;
    public GameObject m_SourceObject;
    public float m_Timer;

    public DamageInstance(float PhysDamage, float MagicDamage, string Id, GameObject SourceObject)
    {
        m_PhysDamage = PhysDamage;
        m_MagicDamage = MagicDamage;
        m_Id = Id;
        m_SourceObject=SourceObject;
        m_Timer = 0.0f;
    }
    public void AddDamage(float PhysDamage, float MagicDamage)
    {
        m_PhysDamage += PhysDamage;
        m_MagicDamage += MagicDamage;
    }
    public void AddToBiggestDamage(float Damage) 
    {
        if(m_PhysDamage>m_MagicDamage)
            m_PhysDamage+=Damage;
        else
            m_MagicDamage+=Damage;
    }
    public void ChangeDamage(float PhysDamage, float MagicDamage) 
    {
        m_PhysDamage = PhysDamage;
        m_MagicDamage = MagicDamage;
    }
    public void ChangeDamageType(DamageType Type) 
    {
        if(Type==DamageType.PHYSICAL) 
        {
            m_PhysDamage+=m_MagicDamage;
            m_MagicDamage=0.0f;
        }
        else 
        {
            m_MagicDamage+=m_PhysDamage;
            m_PhysDamage=0.0f;
        }
    }
}
public class HealthInstance
{
    public float m_Health;
    public float m_Timer;

    public HealthInstance(float HelthToAdd)
    {
        m_Health = HelthToAdd;
        m_Timer = 0.0f;
    }
}
