using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HirasuQProjectile : NetworkBehaviour
{
    public BoxCollider m_Collider;
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
        int TotalSplinters, GameObject SplinterObject, float SplintersDuration, float Width)
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
        m_Collider.size=new Vector3(Width/100.0f, m_Collider.size.y, m_Collider.size.z);
    }
	private void OnTriggerEnter(Collider other)
	{
        if(!IsSpawned||!HasAuthority)
        {
            return;
        }

		if(other.CompareTag("Enemy"))
        {
            if(other.TryGetComponent(out ITakeDamage Enemy))
			{
				Enemy.TakeDamage(m_Damage+m_ExtraPhysDamage, m_ExtraMagicDamage);
                for(int i=0; i<2; i++)
                {
                    if(m_SplintersLeft<=0)
                        break;
                    SpawnSplinter(other.transform.position, transform.forward, other.transform, other.gameObject);
                }
                if(m_SplintersLeft>0)
                    CalcDistancePerSplinter();
			    if(m_Player.GetWSkillLevel()>0 && m_Player.GetRSkillLevel()>0)
                    AddBuffMarkRpc(other.GetComponent<NetworkObject>());
                m_EnemyHit=true;
            }
        }
	}
    void SpawnSplinter(Vector3 Position, Vector3 Forward, Transform Parent, GameObject AttachedEnemy)
    {   
        GameObject l_Splinter=Instantiate(m_Splinter, Position, m_Splinter.transform.rotation, Parent);
        l_Splinter.transform.forward=Forward;
        NetworkObject l_SplinterNetwork=l_Splinter.GetComponent<NetworkObject>();
        l_SplinterNetwork.Spawn();
        if(AttachedEnemy)
            SetSplinterStatsRpc(l_SplinterNetwork, AttachedEnemy.GetComponent<NetworkObject>());
        else
            SetSplinterStatsRpc(l_SplinterNetwork);
        m_SplintersLeft--;
    }
    [Rpc(SendTo.Everyone)]
    void SetSplinterStatsRpc(NetworkObjectReference Splinter) 
    {
        NetworkObject l_Splinter=Splinter;
        l_Splinter.GetComponent<HirasuQSplinter>().SetStats(m_Player, null, m_SplintersDuration);
    }
    [Rpc(SendTo.Everyone)]
    void SetSplinterStatsRpc(NetworkObjectReference Splinter, NetworkObjectReference AttachedEnemy) 
    {
        NetworkObject l_Splinter=Splinter;
        NetworkObject l_AttachedEnemy=AttachedEnemy;
        l_Splinter.GetComponent<HirasuQSplinter>().SetStats(m_Player, l_AttachedEnemy.GetComponent<ITakeDamage>(), m_SplintersDuration);
    }
    void CalcDistancePerSplinter()
    {
        float l_DistanceLeft=(m_Duration-m_TravelTimer)*m_Speed;
        m_DistancePerSplinter=l_DistanceLeft/m_SplintersLeft;
        m_DistancePerSplinterTraveled=0.0f;
        m_DropSplinters=true;
    }
	[Rpc(SendTo.Everyone)]
	void AddBuffMarkRpc(NetworkObjectReference Enemy) 
	{
		NetworkObject l_Enemy=Enemy;
		if(l_Enemy.TryGetComponent(out BuffableEntity Buffs))
			Buffs.AddBuff(m_Player.m_WMarksDebuff.InitializeBuff(m_Player.m_WMarksDuration, l_Enemy.gameObject));
	}
}
