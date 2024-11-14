using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDummy : MonoBehaviour, ITakeDamage
{
    [Header("CHARACTER STATS OBJECT")]
    public CharacterStatsBlock m_CharacterStats;

    [Header("HEALTH & RESISTANCES")]
    public WorldSpaceCanvasBillboard m_Canvas;
    public Slider m_HealthBar;
    public float m_TimeToStartRegen;
    float m_TimerSinceLastDamageTaken;

    [Header("ATTACKS")]
    public float m_TimeToAttack;
    public float m_AttackRadius;
    public LayerMask m_LayerMask;
    bool m_ShowGizmos;

    [Header("UI")]
    public GameObject m_DamageNumbers;
    public Vector3 m_DamageNumbersPosOffset;
    public TMP_FontAsset m_PhysDamageFont;
    public TMP_FontAsset m_MagicDamageFont;

    void Start()
    {
        m_CharacterStats.SetInitStats();
    }
    void Update()
    {
        m_HealthBar.value=m_CharacterStats.GetCurrentHealth()/m_CharacterStats.GetMaxHealth();
        if(m_CharacterStats.GetCurrentHealth()<m_CharacterStats.GetMaxHealth())
        {
            m_TimerSinceLastDamageTaken+=Time.deltaTime;
            if(m_TimerSinceLastDamageTaken>=m_TimeToStartRegen)
            {
                m_CharacterStats.SetCurrentHealth(m_CharacterStats.GetMaxHealth());
                m_CharacterStats.SetCurrentMana(m_CharacterStats.GetMaxMana());
            }
        }
        m_CharacterStats.UpdateMovement();
    }
    public IEnumerator PerformAttack()
    {
        Collider[] l_HitColliders=Physics.OverlapSphere(transform.position, m_AttackRadius/100.0f, m_LayerMask);
        foreach(Collider Entity in l_HitColliders)
        {
            if(Entity.TryGetComponent(out ITakeDamage Enemy))
                Enemy.TakeDamage(m_CharacterStats.GetAttackDamage(), m_CharacterStats.GetAbilityPower());
        }
        m_CharacterStats.SetCurrentMana(m_CharacterStats.GetCurrentMana()-10.0f);
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
        if(PhysDamage>0.0f)
            m_CharacterStats.SetCurrentHealth(m_CharacterStats.GetCurrentHealth()-l_TotalPhysDamage);
        if(MagicDamage>0.0f)
            m_CharacterStats.SetCurrentHealth(m_CharacterStats.GetCurrentHealth()-l_TotalMagicDamage);
        m_TimerSinceLastDamageTaken=0.0f;

        //UI Display
        Vector3 l_PosOffset=m_DamageNumbersPosOffset;
        if(l_TotalPhysDamage>0.0f)
        {
            GameObject l_PhysDamageText=Instantiate(m_DamageNumbers, m_Canvas.transform);
            l_PhysDamageText.GetComponent<RectTransform>().localPosition=Vector3.zero+l_PosOffset;
            l_PosOffset.y-=l_PhysDamageText.GetComponent<RectTransform>().sizeDelta.y;
            TextMeshProUGUI l_TextMesh=l_PhysDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font=m_PhysDamageFont;
            l_TextMesh.text=l_TotalPhysDamage.ToString("f0");
        }
        if(l_TotalMagicDamage>0.0f)
        {
            GameObject l_MagicDamageText=Instantiate(m_DamageNumbers, m_Canvas.transform);
            l_MagicDamageText.GetComponent<RectTransform>().localPosition=Vector3.zero+l_PosOffset;
            TextMeshProUGUI l_TextMesh=l_MagicDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font=m_MagicDamageFont;
            l_TextMesh.text=l_TotalMagicDamage.ToString("f0");
        }
    }
    public CharacterStatsBlock GetCharacterStats()
    {
        return m_CharacterStats;
    }
    public void SetCanvasCamera(Camera CanvasCamera)
    {
        m_Canvas.m_Camera=CanvasCamera;
    }
    public void AddHealth(float Health)
    {
        m_CharacterStats.SetMaxHealth(m_CharacterStats.GetMaxHealth()+Health);
        m_CharacterStats.SetCurrentHealth(m_CharacterStats.GetCurrentHealth()+Health);
        if(m_CharacterStats.GetCurrentHealth()>m_CharacterStats.GetMaxHealth())
            m_CharacterStats.SetCurrentHealth(m_CharacterStats.GetMaxHealth());
    }
}
