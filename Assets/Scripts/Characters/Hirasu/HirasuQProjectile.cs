using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HirasuQProjectile : MonoBehaviour
{
    HirasuCharacterController m_Player;
    GameObject m_Splinter;
    Vector3 m_Direction;
    float m_Duration;
    float m_Range;
    float m_Speed;
    float m_Damage;
    float m_ExtraPhysDamage;
    float m_ExtraMagicDamage;
    float m_SplintersLeft;
    float m_TravelTimer;
    float m_SplintersDuration;
    float m_DistancePerSplinter;
    float m_DistancePerSplinterTraveled;
    bool m_Traveling;
    bool m_DropSplinters;
    bool m_EnemyHit;

    void Update()
    {
        if(m_Traveling)
        {
            transform.position+=m_Direction*m_Speed*Time.deltaTime;
            if(m_DropSplinters && m_SplintersLeft>0)
            {
                m_DistancePerSplinterTraveled+=m_Speed*Time.deltaTime;
                if(m_DistancePerSplinterTraveled>=m_DistancePerSplinter)
                {
                    SpawnSplinter(transform.position, transform.up, null, null);
                    m_DistancePerSplinterTraveled=0.0f;
                }
            }
            m_TravelTimer+=Time.deltaTime;
            if(m_TravelTimer>=m_Duration)
            {
                if(m_SplintersLeft>0 && m_EnemyHit)
                    SpawnSplinter(transform.position, transform.up, null, null);
                Destroy(gameObject);
            }
        }
    }
    public void SetStats(HirasuCharacterController Player, float Range, float Duration, Vector3 Direction, float Damage, float ExtraPhysDamage, float ExtraMagicDamage,
        int TotalSplinters, GameObject SplinterObject, float SplintersDuration)
    {
        m_Player=Player;
        m_Direction=Direction;
        m_Duration=Duration;
        m_Range=Range;
        m_Speed=m_Range/m_Duration;
        m_Damage=Damage;
        m_ExtraPhysDamage=ExtraPhysDamage;
        m_ExtraMagicDamage=ExtraMagicDamage;
        m_SplintersLeft=TotalSplinters;
        m_Splinter=SplinterObject;
        m_SplintersDuration=SplintersDuration;
        transform.forward=Direction;
        m_EnemyHit=false;
        m_DropSplinters=false;
        m_Traveling=true;

    }
	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Enemy"))
        {
            if(other.TryGetComponent(out ITakeDamage Enemy))
			{
				Enemy.TakeDamage(m_Damage+m_ExtraPhysDamage, m_ExtraMagicDamage);
                for(int i=0; i<2; i++)
                {
                    if(m_SplintersLeft<=0)
                        break;
                    SpawnSplinter(other.transform.position, transform.forward, other.transform, Enemy);
                }
                if(m_SplintersLeft>0)
                    CalcDistancePerSplinter();
                m_EnemyHit=true;
            }
        }
	}
    void SpawnSplinter(Vector3 Position, Vector3 Forward, Transform Parent, ITakeDamage AttachedEnemy)
    {   
        GameObject l_Splinter=Instantiate(m_Splinter, Position, m_Splinter.transform.rotation, Parent);
        l_Splinter.transform.forward=Forward;
        l_Splinter.GetComponent<HirasuQSplinter>().SetStats(m_Player, AttachedEnemy, m_SplintersDuration);
        m_SplintersLeft--;
    }
    void CalcDistancePerSplinter()
    {
        float l_DistanceLeft=(m_Duration-m_TravelTimer)*m_Speed;
        m_DistancePerSplinter=l_DistanceLeft/m_SplintersLeft;
        m_DistancePerSplinterTraveled=0.0f;
        m_DropSplinters=true;
        Debug.Log("Distance Left: "+l_DistanceLeft+"  DistancePerSplinter: "+m_DistancePerSplinter);
    }
}
