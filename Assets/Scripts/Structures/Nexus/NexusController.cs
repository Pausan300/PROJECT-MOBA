using System.Collections.Generic;
using UnityEngine;

public class NexusController : MonoBehaviour, ITakeDamageStructure
{
    Animator m_NexusAnimator;

    [Header("STATS")]
    public StructureStats m_NexusStats;
    bool m_Destroyed;
    bool m_Untargetable;

    [Header("UI")]
    public IngameStructureUI m_IngameUI;


    void Start() 
    {
        //m_NexusAnimator=GetComponent<Animator>();
        SetIsUntargetable(true);
        GameManager.m_GameManagerInstance.m_AssignCameras+=m_IngameUI.SetCameraController;
    }

    void Update()
    {
        if(!m_Untargetable) 
        {
            m_IngameUI.UpdateHealthBar(m_NexusStats.GetCurrentHealth(), m_NexusStats.GetMaxHealth());
        }
    }

    public void TakeDamage(CharacterStats Stats)
    {
        if(m_Destroyed || m_Untargetable) 
            return;

        float l_Damage=Stats.GetAttackDamage()+Stats.GetBonusAttackDamage()+(Stats.GetAbilityPower()*0.6f);
        DamageInstance l_AfterResistancesInstance;
        if(Stats.GetBonusAttackDamage()<(Stats.GetAbilityPower()*0.6f)) 
        {
            l_Damage /= (1.0f + m_NexusStats.GetMagicRes() / 100.0f);
            l_AfterResistancesInstance=new DamageInstance(0.0f, l_Damage, Stats.GetPlayerName(), Stats.gameObject);
        }
        else 
        { 
            l_Damage /= (1.0f + m_NexusStats.GetArmor() / 100.0f);
            l_AfterResistancesInstance=new DamageInstance(l_Damage, 0.0f, Stats.GetPlayerName(), Stats.gameObject);
        }
        m_NexusStats.SetCurrentHealthRpc(m_NexusStats.GetCurrentHealth() - l_Damage);
        m_IngameUI.AddDamageInstance(l_AfterResistancesInstance);

        if(m_NexusStats.GetCurrentHealth()<=0.0f) 
        {
            DestroyNexus();
        }
    }
    void DestroyNexus() 
    {
        m_Destroyed=true;
        SetIsUntargetable(true);
        GameManager.m_GameManagerInstance.EndGame(false);
    }

    //GETTERS & SETTERS
    public StructureStats GetStructureStats()
    {
        return m_NexusStats;
    } 
    public void SetAnimatorBool(string Name, bool True)
    {
        m_NexusAnimator.SetBool(Name, True);
    }
    public void SetAnimatorTrigger(string Name)
    {
        m_NexusAnimator.SetTrigger(Name);
    }
    public void SetAnimatorFloat(string Name, float Num) 
    {
        m_NexusAnimator.SetFloat(Name, Num);
    }
    public bool GetIsDestroyed()
    {
        return m_Destroyed;
    }
    public void SetIsDestroyed(bool Destroyed) 
    {
        m_Destroyed=Destroyed;
    }
    public bool GetIsUntargetable()
    {
        return m_Untargetable;
    }
    public void SetIsUntargetable(bool Untargetable) 
    {
        m_Untargetable=Untargetable;
        if(m_Untargetable)
            m_IngameUI.m_IngameHealthBar.gameObject.SetActive(false);
        else
            m_IngameUI.m_IngameHealthBar.gameObject.SetActive(true);
    }
}