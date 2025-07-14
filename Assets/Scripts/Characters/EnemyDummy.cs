using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CharacterStats;
using static UnityEngine.GraphicsBuffer;

public class EnemyDummy : NetworkBehaviour, ITakeDamage
{
    [Header("CHARACTER STATS")]
    public CharacterStats m_CharacterStats;
    public EnemyType m_EnemyType;

    [Header("ATTACKS")]
    public float m_TimeToAttack;
    public float m_AttackRadius;
    public LayerMask m_LayerMask;
    bool m_ShowGizmos;
    bool m_CanDie = false;

    [Header("UI")]
    public GameObject m_WorldCanvasPrefab;
    public GameObject m_IngameUIPrefab;
    public GameObject m_BuffMarksUIPrefab;
    public GameObject m_BuffMarksUI;
    public IngameCharacterUI m_IngameUI;
    public float m_TimeToStartRegen;
    float m_TimerLeftToRegen;

    [Header("MOVEMENT TEST")]
    bool m_MoveToPoint;
    Transform m_TargetPoint;

    private void Awake()
    {
        m_CharacterStats.SetEnemyType(m_EnemyType);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_CharacterStats.SetInitStats();
        GameManager.m_GameManagerInstance.AddToEnemyList(this);
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        GameObject l_WorldCanvas = Instantiate(m_WorldCanvasPrefab, null);
        l_WorldCanvas.GetComponent<NetworkObject>().Spawn();
        GameObject l_IngameUIObject = Instantiate(m_IngameUIPrefab, null);
        l_IngameUIObject.GetComponent<NetworkObject>().Spawn();
        GameObject l_BuffMarksUIObject=Instantiate(m_BuffMarksUIPrefab, null);
        l_BuffMarksUIObject.GetComponent<NetworkObject>().Spawn();
        SpawnCanvasRpc(l_WorldCanvas.GetComponent<NetworkObject>(), l_IngameUIObject.GetComponent<NetworkObject>(), l_BuffMarksUIObject.GetComponent<NetworkObject>());
    }
    private void Start()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }
        foreach (CharacterMaster Player in GameManager.m_GameManagerInstance.GetPlayersList())
        {
            Player.GetOptionsUI().m_VideoMenu.ChangeColorblindMode();
            //Debug.Log(Player.GetOptionsUI().m_VideoMenu)
        }
    }
    [Rpc(SendTo.Everyone)]
    void SpawnCanvasRpc(NetworkObjectReference WorldCanvas, NetworkObjectReference IngameUIObject, NetworkObjectReference BuffUIObject)
    {
        NetworkObject l_WorldCanvas = WorldCanvas;
        if (HasAuthority)
            l_WorldCanvas.TrySetParent(transform, false);
        NetworkObject l_IngameUIObject = IngameUIObject;
        if (HasAuthority)
            l_IngameUIObject.TrySetParent(l_WorldCanvas.transform, false);
        m_IngameUI = l_IngameUIObject.GetComponent<IngameCharacterUI>();
        m_IngameUI.m_WorldCanvas = l_WorldCanvas.gameObject;
        SetIngameUICamera(GameManager.m_GameManagerInstance.GetPlayersList()[0].GetCameraController());
        NetworkObject l_BuffUIObject=BuffUIObject;
        if(HasAuthority)
            l_BuffUIObject.TrySetParent(transform, false);
        m_BuffMarksUI=l_BuffUIObject.gameObject;
    }
    void Update()
    {
        if (m_CharacterStats.GetCurrentHealth() < m_CharacterStats.GetMaxHealth() && !m_CanDie)
        {
            m_TimerLeftToRegen -= Time.deltaTime;
            if (m_TimerLeftToRegen <= 0.0f)
            {
                m_CharacterStats.SetCurrentHealthRpc(m_CharacterStats.GetMaxHealth());
                m_CharacterStats.SetCorruptedHealth(0.0f);
                m_CharacterStats.SetCurrentManaRpc(m_CharacterStats.GetMaxMana());
            }
        }

        if (m_MoveToPoint && !m_CharacterStats.GetImmobilized() && !m_CharacterStats.GetStuned())
        {
            if (Vector3.Distance(transform.position, m_TargetPoint.position) > 1.0f)
            {
                Vector3 l_Dir = m_TargetPoint.position - transform.position;
                l_Dir.Normalize();
                l_Dir.y = 0.0f;
                transform.position += l_Dir * (m_CharacterStats.GetMovSpeed() / 100.0f) * Time.deltaTime;
                transform.forward = l_Dir;
            }
        }

        ScaredBuff();

        UpdateHealthBarRpc();
        UpdateCorruptedHealthRpc();
        UpdateManaBar();

        if (m_CanDie)
        {
            if (m_CharacterStats.GetCurrentHealth() <= 0.0f && m_CharacterStats.GetCorruptedHealth() <= 0.0f) 
                OnDeath();
        }
    }

    void ScaredBuff()
    {
        if (m_CharacterStats.GetScared())
        {
            Vector3 l_DirectionToTarget = (new Vector3(m_CharacterStats.GetFearPos().x, transform.position.y, m_CharacterStats.GetFearPos().z) - transform.position).normalized;
            Vector3 l_OppositeDirection = -l_DirectionToTarget;

            transform.position += l_OppositeDirection * (m_CharacterStats.GetMovSpeed() / 100.0f) * Time.deltaTime;
        }
    }

    public IEnumerator PerformAttack()
    {
        Collider[] l_HitColliders = Physics.OverlapSphere(transform.position, m_AttackRadius / 100.0f, m_LayerMask);
        foreach (Collider Entity in l_HitColliders)
        {
            if (Entity.TryGetComponent(out ITakeDamage Enemy))
                Enemy.TakeDamage(m_CharacterStats.GetAttackDamage(), m_CharacterStats.GetAbilityPower(), false, "PracticeDummy");
        }
        m_CharacterStats.SetCurrentManaRpc(m_CharacterStats.GetCurrentMana() - 10.0f);
        m_ShowGizmos = true;
        yield return new WaitForSeconds(0.25f);
        m_ShowGizmos = false;
    }
    void OnDrawGizmos()
    {
        if (m_ShowGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, m_AttackRadius / 100.0f);
        }
    }
    public void TakeDamage(float PhysDamage, float MagicDamage, bool IgnoreResistances, string SourceId)
    {
        float l_TotalPhysDamage=PhysDamage;
        float l_TotalMagicDamage=MagicDamage;
        if(!IgnoreResistances) 
        {
            l_TotalPhysDamage /= (1.0f + m_CharacterStats.GetArmor() / 100.0f);
            l_TotalMagicDamage /= (1.0f + m_CharacterStats.GetMagicRes() / 100.0f);
        }
        UpdateCurrentHealthRpc(l_TotalPhysDamage + l_TotalMagicDamage, true);
        m_IngameUI.AddDamageInstance(l_TotalPhysDamage, l_TotalMagicDamage, SourceId);
    }
    public void OnDeath()
    {
        if (m_CharacterStats.GetCanSoulTheft())
            m_CharacterStats.GetWilldurrCharacterController().AddSoul(m_CharacterStats.GetEnemyType());

        m_IngameUI.m_WorldCanvas.GetComponent<NetworkObject>().Despawn();
        m_IngameUI.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
    public void SetMovement(bool Move)
    {
        m_MoveToPoint = Move;
    }
    public void SetMovementTarget(Transform Target)
    {
        m_TargetPoint = Target;
    }

    [Rpc(SendTo.Everyone)]
    void UpdateCurrentHealthRpc(float DamageAmount, bool TookDamage)
    {
        float l_NewHealth=m_CharacterStats.GetCurrentHealth()-DamageAmount;
        float l_LeftoverDamage=0.0f; 
        if(l_NewHealth<0.0f) 
        {
            l_LeftoverDamage=-l_NewHealth;
            l_NewHealth=0.0f;
        }
        m_CharacterStats.SetCurrentHealthRpc(l_NewHealth);
        if(l_LeftoverDamage>0.0f) 
        {
            m_CharacterStats.SetCorruptedHealth(m_CharacterStats.GetCorruptedHealth()-l_LeftoverDamage);
            if(m_CharacterStats.GetCorruptedHealth()<0.0f)
                m_CharacterStats.SetCorruptedHealth(0.0f);
        }

        if (TookDamage)
            m_TimerLeftToRegen = m_TimeToStartRegen;
    }
    [Rpc(SendTo.Everyone)]
    void UpdateHealthBarRpc() 
    {
         m_IngameUI.m_IngameHealthBar.value = m_CharacterStats.GetCurrentHealth() / m_CharacterStats.GetMaxHealth();
    }
    void UpdateCorruptedHealthRpc()
    {
        //Debug.Log("Health: "+m_CharacterStats.GetCurrentHealth()+" Corrupted: "+m_CharacterStats.GetCorruptedHealth()+" Value: "+(m_CharacterStats.GetCurrentHealth()+m_CharacterStats.GetCorruptedHealth())/m_CharacterStats.GetMaxHealth());
        m_IngameUI.m_IngameCorruptedHealthBar.value = (m_CharacterStats.GetCurrentHealth() + m_CharacterStats.GetCorruptedHealth()) / m_CharacterStats.GetMaxHealth();
    }
    void UpdateManaBar() 
    {
        m_IngameUI.m_IngameManaBar.value=m_CharacterStats.GetCurrentMana()/m_CharacterStats.GetMaxMana();
    }
    [Rpc(SendTo.Everyone)]
    public void AddHealthRpc(float Health)
    {
        m_CharacterStats.SetMaxHealth(m_CharacterStats.GetMaxHealth() + Health);
        m_CharacterStats.SetCurrentHealthRpc(m_CharacterStats.GetCurrentHealth() + Health);
        if (m_CharacterStats.GetCurrentHealth() > m_CharacterStats.GetMaxHealth())
            m_CharacterStats.SetCurrentHealthRpc(m_CharacterStats.GetMaxHealth());
    }
    [Rpc(SendTo.Everyone)]
    public void AddResistsRpc()
    {
        m_CharacterStats.SetArmor(m_CharacterStats.GetArmor() + 10.0f);
        m_CharacterStats.SetMagicRes(m_CharacterStats.GetMagicRes() + 10.0f);
    }


    public CharacterStats GetCharacterStats()
    {
        return m_CharacterStats;
    }
    public void SetIngameUICamera(CameraController CanvasCamera)
    {
        m_IngameUI.SetCameraController(CanvasCamera);
    }
    public IngameCharacterUI GetIngameCharacterUI()
    {
        return m_IngameUI;
    }
    public void SetCanDie(bool _CanDie)
    {
        m_CanDie = _CanDie;
    }
}
