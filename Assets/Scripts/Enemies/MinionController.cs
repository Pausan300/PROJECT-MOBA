using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class MinionController : MonoBehaviour, ITakeDamage
{
    CharacterStats m_MinionStats;
    TowerController m_EnemyTower;
    public IngameCharacterUI m_IngameUI;
    EnemyMovement m_MinionMovement;

    [Header("TOWER MODIFIER")]
    [Range(0.0f, 1.0f)]
    public float m_TowerDamagePct;

    void Start()
    {
        m_MinionStats=GetComponent<CharacterStats>();
        m_MinionMovement=GetComponent<EnemyMovement>();
        SetIngameUICamera(GameManager.m_GameManagerInstance.GetPlayersList()[0].GetCameraController());
    }

    void Update()
    {
        if (m_MinionStats.GetCurrentHealth() <= 0.0f && m_MinionStats.GetCorruptedHealth() <= 0.0f) 
            OnDeath();
        
        UpdateHealthBar();
        UpdateCorruptedHealth();
    }

    public void TakeDamage(float PhysDamage, float MagicDamage, bool IgnoreResistances, string SourceId, GameObject SourceObject)
    {
        float l_TotalPhysDamage=PhysDamage;
        float l_TotalMagicDamage=MagicDamage;
        if(!IgnoreResistances) 
        {
            l_TotalPhysDamage /= (1.0f + m_MinionStats.GetArmor() / 100.0f);
            l_TotalMagicDamage /= (1.0f + m_MinionStats.GetMagicRes() / 100.0f);
        }
        UpdateCurrentHealthRpc(l_TotalPhysDamage + l_TotalMagicDamage, true);
        m_IngameUI.AddDamageInstance(l_TotalPhysDamage, l_TotalMagicDamage, SourceId);
    } 
    public void OnDeath()
    {
        Destroy(gameObject);
    }

    [Rpc(SendTo.Everyone)]
    void UpdateCurrentHealthRpc(float DamageAmount, bool TookDamage)
    {
        float l_NewHealth=m_MinionStats.GetCurrentHealth()-DamageAmount;
        float l_LeftoverDamage=0.0f; 
        if(l_NewHealth<0.0f) 
        {
            l_LeftoverDamage=-l_NewHealth;
            l_NewHealth=0.0f;
        }
        m_MinionStats.SetCurrentHealthRpc(l_NewHealth);
        if(l_LeftoverDamage>0.0f) 
        {
            m_MinionStats.SetCorruptedHealth(m_MinionStats.GetCorruptedHealth()-l_LeftoverDamage);
            if(m_MinionStats.GetCorruptedHealth()<0.0f)
                m_MinionStats.SetCorruptedHealth(0.0f);
        }
    }
    void UpdateHealthBar() 
    {
         m_IngameUI.m_IngameHealthBar.value=m_MinionStats.GetCurrentHealth()/m_MinionStats.GetMaxHealth();
    }
    void UpdateCorruptedHealth()
    {
        m_IngameUI.m_IngameCorruptedHealthBar.value=(m_MinionStats.GetCurrentHealth()+m_MinionStats.GetCorruptedHealth())/m_MinionStats.GetMaxHealth();
    }

    //GETTERS & SETTERS
    public TowerController GetNearTower() 
    {
        return m_EnemyTower;
    }
    public void SetNearTower(TowerController Tower) 
    {
        m_EnemyTower=Tower;
    }
    public CharacterStats GetCharacterStats()
    {
        return m_MinionStats;
    }
    public EnemyMovement GetEnemyMovement() 
    {
        return m_MinionMovement;
    }
    public void SetIngameUICamera(CameraController CanvasCamera)
    {
        m_IngameUI.SetCameraController(CanvasCamera);
    }
    public IngameCharacterUI GetIngameCharacterUI()
    {
        return m_IngameUI;
    }
}
