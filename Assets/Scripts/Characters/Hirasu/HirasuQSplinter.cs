using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HirasuQSplinter : MonoBehaviour
{
    HirasuCharacterController m_Player;
    float m_LifeTimer;
    bool m_AlreadyExploded;

    void Update()
    {
        if(m_LifeTimer>0.0f && !m_AlreadyExploded)
        {
            m_LifeTimer-=Time.deltaTime;
            if(m_LifeTimer<=0.0f)
            {
                m_Player.DeleteSplintersFromList(this);
                Destroy(gameObject);
            }
        }
    }
    public void SetStats(HirasuCharacterController Player, float LifeTime)
    {
        m_Player=Player;
        m_LifeTimer=LifeTime;
        m_Player.AddSplinterToList(this);
    }
    public IEnumerator WSpawnExplosion(GameObject Explosion, Vector3 EnemyPos, float Radius, float Scale, float TimeToExpand, float Damage, LayerMask DamageLayerMask)
	{
        m_AlreadyExploded=true;
		GameObject l_Explosion=Instantiate(Explosion, EnemyPos, Explosion.transform.rotation);
		float l_Timer=0.0f;
		List<Collider> l_CollidersHit=new List<Collider>();
		while(l_Explosion.transform.localScale.x<Scale)
		{
			l_Explosion.transform.localScale=Vector3.Lerp(Vector3.zero, Vector3.one*Scale, l_Timer/TimeToExpand);
			l_Timer+=Time.deltaTime;
			Collider[] l_HitColliders=Physics.OverlapSphere(l_Explosion.transform.position, l_Explosion.transform.localScale.x/2.0f, DamageLayerMask);
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
        m_Player.DeleteSplintersFromList(this);
		Destroy(l_Explosion);
        Destroy(gameObject);
	}
    public bool GetAlreadyExploded()
    {
        return m_AlreadyExploded;
    }
}
