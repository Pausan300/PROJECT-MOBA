using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class RangedAutoAttack : NetworkBehaviour
{
    CharacterMaster m_CharacterMaster;
    Transform m_Target;
    float m_PhysicDamage;
    float m_MagicDamage;
    float m_Speed;
    public float m_TimeToReachTarget;
    public ParticleSystem m_ParticleSystem;
    float m_TimeToDestroyWithParticle = 5;
    bool m_DamageDone = false;

    public event Action m_OnHitEffects;

    void Update()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, m_Target.position, m_Speed * Time.deltaTime);
        if (!m_DamageDone)
        {

            if (Vector3.Distance(transform.position, m_Target.position) <= 0.005f)
            {
                if (m_Target.TryGetComponent<ITakeDamage>(out ITakeDamage Enemy))
                    Enemy.TakeDamage(m_PhysicDamage, m_MagicDamage, "AutoAttack");

                m_CharacterMaster.RangedAutoAttackHitDamage(m_Target);

                if (m_ParticleSystem != null)
                {
                    m_DamageDone = true;
                    StartCoroutine(DestroyWithParticles());
                }
                else
                    Destroy(gameObject);
            }
           
            m_OnHitEffects?.Invoke();
        }
    }
    IEnumerator DestroyWithParticles()
    {
        yield return null;
        m_ParticleSystem.Stop();
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(m_TimeToDestroyWithParticle);
        Destroy(gameObject);
    }
    public void SetStats(Transform Target, float PhysDamage, float MagicDamage, CharacterMaster _CharacterMaster)
    {
        m_CharacterMaster = _CharacterMaster;
        m_DamageDone = false;
        m_Target = Target;
        m_PhysicDamage = PhysDamage;
        m_MagicDamage = MagicDamage;
        float l_DistanceToTarget = (m_Target.position - transform.position).magnitude;
        m_Speed = l_DistanceToTarget / m_TimeToReachTarget;
    }
}
