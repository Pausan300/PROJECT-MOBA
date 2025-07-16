using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ShunsoWProjectile : NetworkBehaviour
{
    public GameObject m_Projectile;
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
    Vector3 m_InitialScale;
    Vector3 m_TargetScale;
    
    public event Action<int> m_EStacksOnHit;
    public event Action<GameObject> m_OnDamageEnemy;
    
    void Start()
    {
    }

    void Update()
    {
        m_Timer+=Time.deltaTime;
        m_Projectile.transform.localScale=Vector3.Lerp(m_InitialScale, m_TargetScale, m_Timer/m_TimeNeeded);
        m_Projectile.transform.localPosition=Vector3.Lerp(m_InitialPos, m_TargetPos, m_Timer/m_TimeNeeded);

        if(m_Timer>=m_TimeNeeded) 
        {
            Destroy(gameObject);
        }
    }

    public void SetStats(ShunsoCharacterController Player, float CorruptedHealth, float CorruptedHealthDamage, float Speed, float Width, float Range, float Offset, Vector3 Direction, 
        float NormalCharges, float CorruptedCharges) 
    {
        m_Player=Player;
        m_CorruptedHealth=CorruptedHealth;
        m_CorruptedHealthDamage=CorruptedHealthDamage;
        m_Speed=Speed/100.0f;
        m_MaxRange=Range/100.0f;
        m_TimeNeeded=m_MaxRange/m_Speed;
        m_NormalCharges=(int)NormalCharges;
        m_CorruptedCharges=(int)CorruptedCharges;

        transform.forward=Direction;
        m_Projectile.transform.localScale=new Vector3(Width/100.0f, 0.5f, 0.0f);
        m_InitialScale=m_Projectile.transform.localScale;
        m_Projectile.transform.localPosition=new Vector3(0.0f, 0.5f, 0.0f);
        m_InitialPos=m_Projectile.transform.localPosition;

        Vector3 l_FinalPos=transform.position+Direction*m_MaxRange+transform.right*Offset;
        m_Projectile.transform.forward=(l_FinalPos-transform.position).normalized;

        m_TargetPos=transform.InverseTransformPoint((l_FinalPos+transform.position)/2.0f);
        m_TargetPos.y=m_InitialPos.y;
        m_TargetScale=new Vector3(m_Projectile.transform.localScale.x, m_Projectile.transform.localScale.y, (l_FinalPos-transform.position).magnitude);
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
                Enemy.TakeDamage(m_CorruptedHealth, 0.0f, true, m_Player.m_CharacterStats.GetPlayerName());
                //Enemy.GetCharacterStats().SetCurrentHealthRpc(Enemy.GetCharacterStats().GetCurrentHealth()-m_CorruptedHealth);
                if(!m_Player.m_GainStacksCooldown) 
                {
                    if(Enemy.GetCharacterStats().GetCorruptedHealth()>0.0f)
                        m_EStacksOnHit.Invoke(m_CorruptedCharges);
                    else
                        m_EStacksOnHit.Invoke(m_NormalCharges);
                    m_Player.StartCoroutine(m_Player.WAlreadyGainedEStacks());
                }
                    
                m_OnDamageEnemy?.Invoke(other.gameObject);
                Enemy.GetCharacterStats().SetCorruptedHealth(Enemy.GetCharacterStats().GetCorruptedHealth()+m_CorruptedHealth);
                Enemy.GetCharacterStats().SetCorruptedHealthDamage(m_CorruptedHealthDamage);
            }
        }
	}
}
