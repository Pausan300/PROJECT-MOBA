using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDummy : MonoBehaviour, ITakeDamage
{
    [Header("HEALTH & RESISTANCES")]
    public float m_MaxHealth;
    public WorldSpaceCanvasBillboard m_Canvas;
    public Slider m_HealthBar;
    public float m_Armor;
    public float m_MagicResistance;
    public float m_TimeToStartRegen;
    float m_CurrentHealth;
    float m_TimerSinceLastDamageTaken;

    [Header("ATTACKS")]
    public float m_AttackDamage;
    public float m_AbilityPower;
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
        m_CurrentHealth=m_MaxHealth;
    }
    void Update()
    {
        m_HealthBar.value=m_CurrentHealth/m_MaxHealth;
        if(m_CurrentHealth<m_MaxHealth)
        {
            m_TimerSinceLastDamageTaken+=Time.deltaTime;
            if(m_TimerSinceLastDamageTaken>=m_TimeToStartRegen)
                m_CurrentHealth=m_MaxHealth;
        }
    }
    public IEnumerator PerformAttack()
    {
        Collider[] l_HitColliders=Physics.OverlapSphere(transform.position, m_AttackRadius/100.0f, m_LayerMask);
        foreach(Collider Entity in l_HitColliders)
        {
            if(Entity.TryGetComponent(out ITakeDamage Enemy))
                Enemy.TakeDamage(m_AttackDamage, m_AbilityPower);
        }
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
        float l_TotalPhysDamage=PhysDamage/(1.0f+m_Armor/100.0f);
        float l_TotalMagicDamage=MagicDamage/(1.0f+m_MagicResistance/100.0f);
        if(PhysDamage>0.0f)
            m_CurrentHealth-=l_TotalPhysDamage;
        if(MagicDamage>0.0f)
            m_CurrentHealth-=l_TotalMagicDamage;
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
    public void SetCanvasCamera(Camera CanvasCamera)
    {
        m_Canvas.m_Camera=CanvasCamera;
    }
    public void AddHealth(float Health)
    {
        m_MaxHealth+=Health;
        m_CurrentHealth+=Health;
        if(m_CurrentHealth>m_MaxHealth)
            m_CurrentHealth=m_MaxHealth;
    }
}
