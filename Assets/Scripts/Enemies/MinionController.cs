using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class MinionController : NetworkBehaviour, ITakeDamage
{
    CharacterStats m_MinionStats;
    TowerController m_EnemyTower;
    EnemyMovement m_MinionMovement;

    [Header("UI")]
    public GameObject m_IngameUIPrefab;
    IngameEnemyUI m_IngameUI;
    public Color m_HealthBarColor;
    public Color m_CorruptedHealthBarColor;

    [Header("TOWER MODIFIER")]
    [Range(0.0f, 1.0f)]
    public float m_TowerDamagePct;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_MinionStats=GetComponent<CharacterStats>();
        m_MinionMovement=GetComponent<EnemyMovement>();
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }
        GameObject l_IngameUIObject = Instantiate(m_IngameUIPrefab, null);
        l_IngameUIObject.GetComponent<NetworkObject>().Spawn();
        l_IngameUIObject.GetComponent<NetworkObject>().TrySetParent(transform, false);
        m_IngameUI=l_IngameUIObject.GetComponent<IngameEnemyUI>();
        SetIngameUICamera(GameManager.m_GameManagerInstance.GetPlayersList()[0].GetCameraController());
        m_IngameUI.ChangeHealthColor(m_HealthBarColor);
        m_IngameUI.ChangeCorruptedHealthColor(m_CorruptedHealthBarColor);
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
        m_IngameUI.GetBuffMarksCanvas().GetComponent<NetworkObject>().Despawn(true);
        m_IngameUI.GetComponent<NetworkObject>().Despawn(true);
        GetComponent<NetworkObject>().Despawn(true);
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
    public IngameEnemyUI GetIngameUI()
    {
        return m_IngameUI;
    }
}
