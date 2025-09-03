using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShunsoPManaParticle : MonoBehaviour
{
    Transform m_Target;

    float m_Speed;
    public float m_TimeToReachTarget;
    public ParticleSystem m_ParticleSystem;
    float m_TimeToDestroyWithParticle = 1.0f;
    bool m_TargetReached = false;

    public event Action m_OnReachTarget;

    void Update()
    {
        Vector3 l_TargetPos=m_Target.position+Vector3.up;
        transform.position = Vector3.MoveTowards(transform.position, l_TargetPos, m_Speed * Time.deltaTime);
        if (!m_TargetReached)
        {
            if (Vector3.Distance(transform.position, l_TargetPos) <= 0.01f)
            {
                m_TargetReached=true;
                m_OnReachTarget?.Invoke();

                if (m_ParticleSystem != null)
                    StartCoroutine(DestroyWithParticles());
                else
                    Destroy(gameObject);
            }  
        }
    }

    public void SetStats(Transform Target)
    {
        m_TargetReached = false;
        m_Target = Target;
        float l_DistanceToTarget = (m_Target.position - transform.position).magnitude;
        m_Speed = l_DistanceToTarget / m_TimeToReachTarget;
    }

    IEnumerator DestroyWithParticles()
    {
        m_ParticleSystem.Stop();
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(m_TimeToDestroyWithParticle);
        Destroy(gameObject);
    }
}
