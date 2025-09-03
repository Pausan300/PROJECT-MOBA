using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ShunsoWProjectile : NetworkBehaviour
{
    ShunsoCharacterController m_Player;

    float m_CorruptedHealth;
    float m_CorruptedHealthDamage;
    float m_Speed;
    float m_TimeNeeded;
    float m_Timer;
    float m_MaxRange;
    int m_NormalCharges;
    int m_CorruptedCharges;
    Vector3 m_InitialPos;
    Vector3 m_TargetPos;
    Vector3 m_Direction;
    
    public event Action<int> m_EStacksOnHit;
    public event Action<GameObject> m_OnDamageEnemy;
    
    void Start()
    {
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, m_InitialPos)<=m_MaxRange)
            transform.position+=m_Direction*m_Speed*Time.deltaTime;
        else
            Destroy(gameObject);
    }

    public void SetStats(ShunsoCharacterController Player, float CorruptedHealth, float CorruptedHealthDamage, float Time, float Width, float Range, float Offset, Vector3 Direction, 
        float NormalCharges, float CorruptedCharges) 
    {
        m_Player=Player;
        m_CorruptedHealth=CorruptedHealth;
        m_CorruptedHealthDamage=CorruptedHealthDamage;
        m_NormalCharges=(int)NormalCharges;
        m_CorruptedCharges=(int)CorruptedCharges;

        transform.forward=Direction;
        transform.localScale=new Vector3(Width/100.0f, 0.5f, 1.0f);
        m_InitialPos=transform.position;

        m_TargetPos=transform.position+Direction*(Range/100.0f)+transform.right*Offset;
        m_TargetPos.y=m_InitialPos.y;
        m_Direction=(m_TargetPos-transform.position).normalized;
        transform.forward=m_Direction;
        
        m_MaxRange=Vector3.Distance(transform.position, m_TargetPos);
        m_Speed=m_MaxRange/Time;
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
                Enemy.TakeDamage(m_CorruptedHealth, 0.0f, true, m_Player.m_CharacterStats.GetPlayerName(), m_Player.gameObject);
                //Enemy.GetCharacterStats().SetCurrentHealthRpc(Enemy.GetCharacterStats().GetCurrentHealth()-m_CorruptedHealth);

                if(Enemy.GetCharacterStats().GetCorruptedHealth()>0.0f && !m_Player.m_WEnemiesHit.Contains(other.gameObject))
                    m_EStacksOnHit.Invoke(m_CorruptedCharges);
                else 
                {
                    m_EStacksOnHit.Invoke(m_NormalCharges);
                    if(!m_Player.m_WEnemiesHit.Contains(other.gameObject))
                        m_Player.m_WEnemiesHit.Add(other.gameObject);
                }

                if(!m_Player.m_WResetingEnemiesHit)
                    m_Player.StartCoroutine(m_Player.WResetEnemyHitList());
                    
                m_OnDamageEnemy?.Invoke(other.gameObject);
                Enemy.GetCharacterStats().SetCorruptedHealth(Enemy.GetCharacterStats().GetCorruptedHealth()+m_CorruptedHealth);
                Enemy.GetCharacterStats().SetCorruptedHealthDamage(m_CorruptedHealthDamage);
            }
        }
	}
}
