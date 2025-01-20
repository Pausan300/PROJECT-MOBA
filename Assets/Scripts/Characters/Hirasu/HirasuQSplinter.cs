using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HirasuQSplinter : NetworkBehaviour
{
    HirasuCharacterController m_Player;
    ITakeDamage m_AttachedEnemy;
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
    public void SetStats(HirasuCharacterController Player, ITakeDamage AttachedEnemy, float LifeTime)
    {
        m_Player=Player;
        if(AttachedEnemy!=null)
            m_AttachedEnemy=AttachedEnemy;
        m_LifeTimer=LifeTime;
        m_Player.AddSplinterToList(this);
    }
    public IEnumerator WSpawnExplosion(GameObject Explosion, Vector3 EnemyPos, float Radius, float Scale, float TimeToExpand, float AttachedDamage, float Damage, LayerMask DamageLayerMask)
	{
        m_AlreadyExploded=true;
		GameObject l_Explosion=Instantiate(Explosion, EnemyPos, Explosion.transform.rotation);
        l_Explosion.GetComponent<NetworkObject>().SpawnWithOwnership(m_Player.GetComponent<NetworkObject>().OwnerClientId);
		float l_Timer=0.0f;
        float l_TotalDamage=Damage;
        float l_SlowAmount=m_Player.m_WSpeedDebuffAmountExplosion;
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
                    if(Enemy==m_AttachedEnemy)
                    {
                        l_TotalDamage=AttachedDamage;
                        l_SlowAmount=m_Player.m_WSpeedDebuffAmountSplinter;
                    }
                    if(Entity.TryGetComponent(out BuffableEntity Buffs))
                    {
                        AddDebuffRpc(Entity.GetComponent<NetworkObject>(), l_SlowAmount);
						if(Buffs.IsMarkBuffActive(m_Player.m_WMarksDebuff))
						    l_TotalDamage*=(1.0f+m_Player.m_WMarksExtraDamage/100.0f);
					    Debug.Log("TAKEN "+l_TotalDamage+" DAMAGE");
                    }
					Enemy.TakeDamage(l_TotalDamage, 0.0f);
					l_CollidersHit.Add(Entity);
				}
			}
			yield return null;
		}
        m_Player.DeleteSplintersFromList(this);
		Destroy(l_Explosion);
        Destroy(gameObject);
	}
	[Rpc(SendTo.Everyone)]
	void AddDebuffRpc(NetworkObjectReference Enemy, float SlowAmount) 
	{
		NetworkObject l_Enemy=Enemy;
		l_Enemy.GetComponent<BuffableEntity>().AddBuff(m_Player.m_WSpeedDebuff.InitializeBuff(m_Player.m_WSpeedDebuffDuration, SlowAmount, l_Enemy.gameObject));
	}

    public bool GetAlreadyExploded()
    {
        return m_AlreadyExploded;
    }
}
