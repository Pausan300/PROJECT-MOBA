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
    Collider m_LastTarget;
    LayerMask m_DamageLayerMask;
    int m_MaxBounces;
    int m_Bounces;
    float m_Speed;
    List<Collider> m_EnemysToIgnore;
    ZappadasCharacterController m_CharacterController;
    CharacterStats m_CharacterStats;
    bool m_isQ;

    public void SetProjectilUpGrade(float Damage, float TimeToArribeTarget, float Range, float AditionalDamageMinions, LayerMask DamageLayerMask, int MaxBounces, Collider EnemyToIgnore, CharacterStats _CharacterStats, ZappadasCharacterController _ZappadasCharacterController, bool isQ)
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
        m_isQ = isQ;
    }

    private void FindEnemy()
    {
        Debug.Log("m_Bounces: " + m_Bounces + "/" + m_MaxBounces);
        if (m_Bounces == m_MaxBounces)
            Destroy(gameObject);
        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(m_ProjectilTransform.position, m_Range / 100, m_DamageLayerMask);
        m_Target = null;
        float l_MinDistance = float.MaxValue;
        bool l_TargetIsMinion = false;
        Collider l_targetCollider = null;
        Debug.Log("m_Range: " + m_Range / 100);
        Debug.Log("m_ProjectilTransform.position: " + m_ProjectilTransform.position);
        Debug.Log("l_HitColliders: " + l_HitColliders.Length);

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
                            l_TargetPosW = Entity.transform.position;
                            m_Target = Entity;
                            l_targetCollider = Entity;
                        }
                    }
                }
                else
                {
                    if (Vector3.Distance(Entity.transform.position, m_ProjectilTransform.position) < l_MinDistance)
                    {
                        l_TargetPosW = Entity.transform.position;
                        m_Target = Entity;
                        l_targetCollider = Entity;
                        if (Enemy.GetCharacterStats().GetEnemyType() == CharacterStats.EnemyType.MINION)
                            l_TargetIsMinion = true;
                    }
                }
                l_CollidersHit.Add(Entity);
            }
        }

        Debug.Log(m_Target);
        if (m_Target == null)
        {
            Destroy(gameObject);
            return;
        }

        m_EnemysToIgnore.Add(l_targetCollider);
        m_Bounces++;
        Vector3 l_TargetPos = m_Target.transform.position;
        l_TargetPos.y = m_ProjectilTransform.position.y;
        m_Speed = Vector3.Distance(l_TargetPos, m_ProjectilTransform.position) / m_TimeToArribeTarget;
        m_LastTarget = m_Target;


    }
    Vector3 l_TargetPosW;
    private void Update()
    {
        if (m_isQ)
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
        else
        {
            if (m_Target != null)
            {
                Vector3 l_TargetPos = l_TargetPosW;
                l_TargetPos.y = m_ProjectilTransform.position.y;
                m_ProjectilTransform.position = Vector3.MoveTowards(m_ProjectilTransform.position, l_TargetPos, m_Speed * Time.deltaTime);

                if (Vector3.Distance(l_TargetPos, m_ProjectilTransform.position) <= 0.05f)
                {
                    Vector3 l_TargetPosEnemy = m_Target.transform.position;
                    l_TargetPosEnemy.y = m_ProjectilTransform.position.y;
                    if (Vector3.Distance(l_TargetPosEnemy, m_ProjectilTransform.position) <= 0.05f)
                    {
                        DoDamage();
                    }
                    FindEnemy();

                }
            }
        }
    }

    private void DoDamage()
    {
        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(m_ProjectilTransform.position, m_Range / 100, m_DamageLayerMask);
        Collider l_Target = null;
        float l_MinDistance = float.MaxValue;
        bool l_TargetIsMinion = false;
        Collider l_targetCollider = null;

        foreach (Collider Entity in l_HitColliders)
        {
            if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage _Enemy) && !m_EnemysToIgnore.Contains(Entity))
            {
                if (l_TargetIsMinion)
                {
                    if (_Enemy.GetCharacterStats().GetEnemyType() == CharacterStats.EnemyType.MINION)
                    {
                        if (Vector3.Distance(Entity.transform.position, m_ProjectilTransform.position) < l_MinDistance)
                        {
                            l_Target = Entity;
                            l_targetCollider = Entity;
                        }
                    }
                }
                else
                {
                    if (Vector3.Distance(Entity.transform.position, m_ProjectilTransform.position) < l_MinDistance)
                    {
                        l_Target = Entity;
                        l_targetCollider = Entity;
                        if (_Enemy.GetCharacterStats().GetEnemyType() == CharacterStats.EnemyType.MINION)
                            l_TargetIsMinion = true;
                    }
                }
                l_CollidersHit.Add(Entity);
            }
        }


        if (m_Target.TryGetComponent(out ITakeDamage Enemy))
        {
            float l_Damage = m_Damage;

            if (Enemy.GetCharacterStats().GetEnemyType() == CharacterStats.EnemyType.MINION)
                l_Damage += m_AditionalDamageMinions;

            Debug.Log("Sum" + (m_MaxBounces - m_Bounces));
            Debug.Log("MAX" + m_MaxBounces);
            Debug.Log("BOU" + m_Bounces);

            if (l_Target == null)
                l_Damage = m_Damage * (m_MaxBounces - m_Bounces);

            Debug.Log("TAKEN " + l_Damage + " DAMAGE");
            DamageInstance l_DamageInstance=new DamageInstance(0.0f, l_Damage, m_CharacterStats.GetPlayerName(), m_CharacterController.gameObject);
            Enemy.TakeDamage(l_DamageInstance);
            m_CharacterController.AddDarkPower(2);
        }
    }
}
