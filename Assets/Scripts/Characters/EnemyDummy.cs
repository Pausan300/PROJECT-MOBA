using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDummy : NetworkBehaviour, ITakeDamage
{
    [Header("CHARACTER STATS")]
    public CharacterStats m_CharacterStats;

    [Header("ATTACKS")]
    public float m_TimeToAttack;
    public float m_AttackRadius;
    public LayerMask m_LayerMask;
    bool m_ShowGizmos;

    [Header("UI")]
    public GameObject m_WorldCanvasPrefab;
    public GameObject m_IngameUIPrefab;
    public IngameCharacterUI m_IngameUI;
    public float m_TimeToStartRegen;
    float m_TimerLeftToRegen;

    public GameObject m_DamageNumbers;
    public Vector3 m_DamageNumbersPosOffset;
    public TMP_FontAsset m_PhysDamageFont;
    public TMP_FontAsset m_MagicDamageFont;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_CharacterStats.SetInitStats();
        if(!IsSpawned || !HasAuthority)
        {
            return;
        }
        GameObject l_WorldCanvas=Instantiate(m_WorldCanvasPrefab, null);
        l_WorldCanvas.GetComponent<NetworkObject>().Spawn();
        GameObject l_IngameUIObject=Instantiate(m_IngameUIPrefab, null);
        l_IngameUIObject.GetComponent<NetworkObject>().Spawn();
        SpawnCanvasRpc(l_WorldCanvas.GetComponent<NetworkObject>(), l_IngameUIObject.GetComponent<NetworkObject>());
    }
    [Rpc(SendTo.Everyone)]
    void SpawnCanvasRpc(NetworkObjectReference WorldCanvas, NetworkObjectReference IngameUIObject) 
    {
        NetworkObject l_WorldCanvas=WorldCanvas;
        if(HasAuthority)
            l_WorldCanvas.TrySetParent(transform, false);
        NetworkObject l_IngameUIObject=IngameUIObject;
        if(HasAuthority)
            l_IngameUIObject.TrySetParent(l_WorldCanvas.transform, false);
        m_IngameUI=l_IngameUIObject.GetComponent<IngameCharacterUI>();
        m_IngameUI.m_WorldCanvas=l_WorldCanvas.gameObject;
    }
    void Update()
    {
        if(m_CharacterStats.GetCurrentHealth()<m_CharacterStats.GetMaxHealth())
        {
            m_TimerLeftToRegen-=Time.deltaTime;
            if(m_TimerLeftToRegen<=0.0f)
            {
                UpdateCurrentHealthRpc(m_CharacterStats.GetMaxHealth(), false);
                m_CharacterStats.SetCurrentManaRpc(m_CharacterStats.GetMaxMana());
            }
        }
    }
    public IEnumerator PerformAttack()
    {
        Collider[] l_HitColliders=Physics.OverlapSphere(transform.position, m_AttackRadius/100.0f, m_LayerMask);
        foreach(Collider Entity in l_HitColliders)
        {
            if(Entity.TryGetComponent(out ITakeDamage Enemy))
                Enemy.TakeDamage(m_CharacterStats.GetAttackDamage(), m_CharacterStats.GetAbilityPower());
        }
        m_CharacterStats.SetCurrentManaRpc(m_CharacterStats.GetCurrentMana()-10.0f);
        m_ShowGizmos=true;
        yield return new WaitForSeconds(0.25f);
        m_ShowGizmos=false;
    }
    void OnDrawGizmos()
    {
        if(m_ShowGizmos)
        {
            Gizmos.color=Color.red;
            Gizmos.DrawSphere(transform.position, m_AttackRadius/100.0f);
        }
    }
    public void TakeDamage(float PhysDamage, float MagicDamage)
    {
        float l_TotalPhysDamage=PhysDamage/(1.0f+m_CharacterStats.GetArmor()/100.0f);
        float l_TotalMagicDamage=MagicDamage/(1.0f+m_CharacterStats.GetMagicRes()/100.0f);
        UpdateCurrentHealthRpc(m_CharacterStats.GetCurrentHealth()-l_TotalPhysDamage-l_TotalMagicDamage, true);

        //UI Display
        Vector3 l_PosOffset=m_DamageNumbersPosOffset;
        if(l_TotalPhysDamage>0.0f)
        {
            GameObject l_PhysDamageText=Instantiate(m_DamageNumbers, m_IngameUI.transform);
            l_PhysDamageText.GetComponent<RectTransform>().localPosition=Vector3.zero+l_PosOffset;
            l_PosOffset.y-=l_PhysDamageText.GetComponent<RectTransform>().sizeDelta.y;
            TextMeshProUGUI l_TextMesh=l_PhysDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font=m_PhysDamageFont;
            l_TextMesh.text=l_TotalPhysDamage.ToString("f0");
        }
        if(l_TotalMagicDamage>0.0f)
        {
            GameObject l_MagicDamageText=Instantiate(m_DamageNumbers, m_IngameUI.transform);
            l_MagicDamageText.GetComponent<RectTransform>().localPosition=Vector3.zero+l_PosOffset;
            TextMeshProUGUI l_TextMesh=l_MagicDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font=m_MagicDamageFont;
            l_TextMesh.text=l_TotalMagicDamage.ToString("f0");
        }
    }

    [Rpc(SendTo.Everyone)]
    void UpdateCurrentHealthRpc(float Amount, bool TookDamage) 
    {
        m_CharacterStats.SetCurrentHealthRpc(Amount);
        m_IngameUI.m_IngameHealthBar.value=m_CharacterStats.GetCurrentHealth()/m_CharacterStats.GetMaxHealth();
        if(TookDamage)
            m_TimerLeftToRegen=m_TimeToStartRegen;
    }
    [Rpc(SendTo.Everyone)]
    public void AddHealthRpc(float Health)
    {
        m_CharacterStats.SetMaxHealth(m_CharacterStats.GetMaxHealth()+Health);
        m_CharacterStats.SetCurrentHealthRpc(m_CharacterStats.GetCurrentHealth()+Health);
        if(m_CharacterStats.GetCurrentHealth()>m_CharacterStats.GetMaxHealth())
            m_CharacterStats.SetCurrentHealthRpc(m_CharacterStats.GetMaxHealth());
    }
    [Rpc(SendTo.Everyone)]
    public void AddResistsRpc() 
    {
        m_CharacterStats.SetArmor(m_CharacterStats.GetArmor()+10.0f);
        m_CharacterStats.SetMagicRes(m_CharacterStats.GetMagicRes()+10.0f);
    }

    public CharacterStats GetCharacterStats()
    {
        return m_CharacterStats;
    }
    public void SetCanvasCamera(Camera CanvasCamera)
    {
        m_IngameUI.SetCamera(CanvasCamera);
    }
    public IngameCharacterUI GetIngameCharacterUI() 
    {
        return m_IngameUI;
    }
}
