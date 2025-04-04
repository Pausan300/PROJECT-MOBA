using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class RangedAutoAttack : NetworkBehaviour
{
    Transform m_Target;
    float m_PhysicDamage;
    float m_MagicDamage;
    float m_Speed;
    public float m_TimeToReachTarget;
    
    void Update()
    {
        if(!IsSpawned || !HasAuthority)
        {
            return;
        }
        Vector3 l_Direction=m_Target.position-transform.position;
        l_Direction.Normalize();
        transform.position+=l_Direction*m_Speed*Time.deltaTime;
        if(Vector3.Distance(transform.position, m_Target.position)<=0.1f) 
        {
            if(m_Target.TryGetComponent<ITakeDamage>(out ITakeDamage Enemy))
                Enemy.TakeDamage(m_PhysicDamage, m_MagicDamage, "AutoAttack");
            Destroy(gameObject);
        }
    }
    public void SetStats(Transform Target, float PhysDamage, float MagicDamage) 
    {
        m_Target=Target;
        m_PhysicDamage=PhysDamage;
        m_MagicDamage=MagicDamage;
        float l_DistanceToTarget=(m_Target.position-transform.position).magnitude;
        m_Speed=l_DistanceToTarget/m_TimeToReachTarget;
    }
}
