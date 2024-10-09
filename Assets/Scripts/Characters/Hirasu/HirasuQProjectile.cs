using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HirasuQProjectile : MonoBehaviour
{
    Vector3 m_Direction;
    float m_Duration;
    float m_Speed;
    float m_TravelTimer;
    float m_LifeTimer;
    bool m_Traveling;
    bool m_WaitForExplosion;
    float m_WExplosionRadius;
    float m_WTimeToExpand;
    float m_Damage;
    float m_Splinters;
    LayerMask m_DamageLayerMask;
    GameObject m_WExplosion;

    void Update()
    {
        if(m_Traveling)
        {
            transform.position+=m_Direction*m_Speed*Time.deltaTime;
            m_TravelTimer+=Time.deltaTime;
            if(m_TravelTimer>=m_Duration)
            {
                m_Traveling=false;
                m_WaitForExplosion=true;
            }
        }
        else if(m_WaitForExplosion)
        {

        }
    }
    public void SetStats(float Range, float Duration, Vector3 Direction, float Damage)
    {
        m_Direction=Direction;
        m_Duration=Duration;
        m_Speed=Range/Duration;
        m_Damage=Damage;
        transform.forward=Direction;
        m_Traveling=true;
    }
    IEnumerator WSpawnExplosion(Vector3 EnemyPos, Vector3 DesiredPos, float Radius, float Scale, float Damage)
	{
		GameObject l_Explosion=Instantiate(m_WExplosion, EnemyPos, m_WExplosion.transform.rotation);
		Vector3 l_DesiredPos=DesiredPos*Radius;
		float l_Timer=0.0f;
		List<Collider> l_CollidersHit=new List<Collider>();
		Debug.Log(l_DesiredPos);
		while(l_Explosion.transform.localScale.x<Scale)
		{
			l_Explosion.transform.localScale=Vector3.Lerp(Vector3.zero, Vector3.one*Scale, l_Timer/m_WTimeToExpand);
			l_Explosion.transform.position=Vector3.Lerp(EnemyPos, EnemyPos+l_DesiredPos, l_Timer/m_WTimeToExpand);
			l_Timer+=Time.deltaTime;
			Collider[] l_HitColliders=Physics.OverlapSphere(l_Explosion.transform.position, l_Explosion.transform.localScale.x/2.0f, m_DamageLayerMask);
			foreach(Collider Entity in l_HitColliders)
			{
				if(!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
				{
					Enemy.TakeDamage(Damage, 0.0f);
					l_CollidersHit.Add(Entity);
				}
			}
			yield return null;
		}
		Destroy(l_Explosion);
        Destroy(gameObject);
	}
	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Enemy"))
        {
            if(other.TryGetComponent(out ITakeDamage Enemy))
			{
				Enemy.TakeDamage(m_Damage, 0.0f);
            }
        }
	}
}
