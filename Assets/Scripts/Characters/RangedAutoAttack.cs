using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.EventSystems.EventTrigger;

public class RangedAutoAttack : NetworkBehaviour
{
    CharacterMaster m_CharacterMaster;
    Transform m_Target;
    float m_PhysicDamage;
    float m_MagicDamage;
    public float m_Speed;
    public ParticleSystem m_ParticleSystem;
    float m_TimeToDestroyWithParticle = 5;
    bool m_DamageDone = false;

    public event Action<GameObject> m_OnHitEffects;

    void Update()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        Vector3 l_NewPos=Vector3.MoveTowards(transform.position, m_Target.position, (m_Speed/100.0f) * Time.deltaTime);
        transform.forward=(l_NewPos-transform.position).normalized;
        transform.position=l_NewPos;

        if (!m_DamageDone)
        {
            if (Vector3.Distance(transform.position, m_Target.position) <= 0.005f)
            {
                if (m_Target.TryGetComponent(out ITakeDamage Enemy))
                    DamageEnemy(Enemy);
                else if(m_Target.TryGetComponent(out ITakeDamageTower Tower))
                    DamageTower(Tower);

                m_CharacterMaster.RangedAutoAttackHitDamage(m_Target);

                if (m_ParticleSystem != null)
                {
                    m_DamageDone = true;
                    StartCoroutine(DestroyWithParticles());
                }
                else
                    Destroy(gameObject);
            }  
        }
    }
    protected virtual void DamageEnemy(ITakeDamage Enemy) 
    {
        Enemy.TakeDamage(m_PhysicDamage, m_MagicDamage, false, "AutoAttack", m_CharacterMaster.gameObject);
        m_OnHitEffects?.Invoke(m_Target.gameObject);
    }
    void DamageTower(ITakeDamageTower Tower) 
    {
        Tower.TakeDamage(m_PhysicDamage, m_MagicDamage, false, "AutoAttack");
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
    }
}
