using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZappadasQProjectileUpgrade : MonoBehaviour
{
    public Transform m_ProjectilTransform;
    float m_Damage;
    float m_TimeToArribeTarget;
    float m_Range;
    float m_AditionalDamageMinions;
    Collider m_Target;
    LayerMask m_DamageLayerMask;
    int m_MaxBounces;
    int m_Bounces;
    float m_Speed;
    List<Collider> m_EnemysToIgnore;
    ZappadasCharacterController m_CharacterController;
    CharacterStats m_CharacterStats;

    public void SetProjectilUpGrade(float Damage, float TimeToArribeTarget, float Range, float AditionalDamageMinions, LayerMask DamageLayerMask, int MaxBounces, Collider EnemyToIgnore, CharacterStats _CharacterStats, ZappadasCharacterController _ZappadasCharacterController)
    {
        m_Damage = Damage;
        m_TimeToArribeTarget = TimeToArribeTarget;
        m_Range = Range;
        m_AditionalDamageMinions = AditionalDamageMinions;
        m_DamageLayerMask = DamageLayerMask;
        m_MaxBounces = MaxBounces;
        m_Bounces = 0;
        m_EnemysToIgnore = new List<Collider>();
        m_EnemysToIgnore.Add(EnemyToIgnore);
        m_CharacterStats = _CharacterStats;
        m_CharacterController = _ZappadasCharacterController;
        FindEnemy();
    }

    private void FindEnemy()
    {
        if (m_Bounces == m_MaxBounces)
            Destroy(gameObject);
        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(m_ProjectilTransform.position, m_Range / 100, m_DamageLayerMask);
        m_Target = null;
        float l_MinDistance = float.MaxValue;
        bool l_TargetIsMinion = false;
        Collider l_targetCollider = null;

        foreach (Collider Entity in l_HitColliders)
        {
            if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy) && !m_EnemysToIgnore.Contains(Entity))
            {
                if (l_TargetIsMinion)
                {
                    if (Enemy.GetCharacterStats().GetEnemyType() == CharacterStats.EnemyType.MINION)
                    {
                        if (Vector3.Distance(Entity.transform.position, m_ProjectilTransform.position) < l_MinDistance)
                        {
                            m_Target = Entity;
                            l_targetCollider = Entity;
                        }
                    }
                }
                else
                {
                    if (Vector3.Distance(Entity.transform.position, m_ProjectilTransform.position) < l_MinDistance)
                    {
                        m_Target = Entity;
                        l_targetCollider = Entity;
                        if (Enemy.GetCharacterStats().GetEnemyType() == CharacterStats.EnemyType.MINION)
                            l_TargetIsMinion = true;
                    }
                }
                l_CollidersHit.Add(Entity);
            }
        }
        if (m_Target == null)
            Destroy(gameObject);

        m_EnemysToIgnore.Add(l_targetCollider);

        m_Bounces++;
        Vector3 l_TargetPos = m_Target.transform.position;
        l_TargetPos.y = m_ProjectilTransform.position.y;
        m_Speed = Vector3.Distance(l_TargetPos, m_ProjectilTransform.position) / m_TimeToArribeTarget;
    }

    private void Update()
    {
        if (m_Target != null)
        {
            Vector3 l_TargetPos = m_Target.transform.position;
            l_TargetPos.y = m_ProjectilTransform.position.y;
            m_ProjectilTransform.position = Vector3.MoveTowards(m_ProjectilTransform.position, l_TargetPos, m_Speed * Time.deltaTime);

            if (Vector3.Distance(l_TargetPos, m_ProjectilTransform.position) <= 0.05f)
            {
                DoDamage();
                FindEnemy();
            }
        }
    }

    private void DoDamage()
    {

        if (m_Target.TryGetComponent(out ITakeDamage Enemy))
        {
            float l_Damage = m_Damage;
            if (Enemy.GetCharacterStats().GetEnemyType() == CharacterStats.EnemyType.MINION)
                l_Damage += m_AditionalDamageMinions;

            Debug.Log("TAKEN " + l_Damage + " DAMAGE");
            Enemy.TakeDamage(0, l_Damage, m_CharacterStats.GetPlayerName());
            m_CharacterController.AddmDarkPowerDamageLightlessWithSkill();
        }
    }
}
