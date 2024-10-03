using System.Collections;
using System.Collections.Generic;
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
        if(PhysDamage>0.0f)
        {
            m_CurrentHealth-=PhysDamage/(1.0f+m_Armor/100.0f);
            Debug.Log("Taking "+PhysDamage+" physical damage, reduced to "+(PhysDamage/(1.0f+m_Armor/100.0f))+" damage");
        }
        if(MagicDamage>0.0f)
        {
            m_CurrentHealth-=MagicDamage/(1.0f+m_MagicResistance/100.0f);
            Debug.Log("Taking "+MagicDamage+" magical damage, reduced to "+(MagicDamage/(1.0f+m_Armor/100.0f))+" damage");
        }
        m_TimerSinceLastDamageTaken=0.0f;
    }
    public void SetCanvasCamera(Camera CanvasCamera)
    {
        m_Canvas.m_Camera=CanvasCamera;
    }
}
