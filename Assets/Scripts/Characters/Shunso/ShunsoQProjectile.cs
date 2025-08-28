using System;
using System.Collections;
using TreeEditor;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class ShunsoQProjectile : NetworkBehaviour
{
    ShunsoCharacterController m_Player;

    public GameObject m_InitialProjectile;
    public GameObject m_SlashProjectile;

    float m_Damage;
    float m_ProjectileSpeed;
    float m_ProjectileWidth;
    float m_ProjectileMaxRange;
    float m_SlashSpeed;
    float m_SlashWidth;
    float m_SlashMaxRange;
    float m_CurrentSpeed;
    int m_NormalCharges;
    int m_CorruptedCharges;

    Vector3 m_InitPos;
    Vector3 m_Direction;
    bool m_Exploded;
    bool m_GoRight;

    public event Action<int> m_EStacksOnHit;
    public event Action<GameObject> m_OnDamageEnemy;

    void Start()
    {
    }

    void Update()
    {
        transform.forward=m_Direction;
        transform.position+=m_Direction*m_CurrentSpeed*Time.deltaTime;
        if(!m_Exploded && Vector3.Distance(m_InitPos, transform.position)>=m_ProjectileMaxRange)
        { 
            StartCoroutine(StartSlash());
        }
        else if(m_Exploded && Vector3.Distance(m_InitPos, transform.position)>=m_SlashMaxRange) 
        {
            Destroy(gameObject);
        }
    }

    public void SetStats(ShunsoCharacterController Player, float Damage, float ProjectileSpeed, float ProjectileWidth, float ProjectileRange, float SlashSpeed, float SlashWidth, float SlashRange, 
        Vector3 Direction, bool GoRight, float NormalCharges, float CorruptedCharges) 
    {
        m_InitPos=transform.position;
        m_Player=Player;
        m_Damage=Damage;
        m_ProjectileSpeed=ProjectileSpeed/100.0f;
        m_ProjectileWidth=ProjectileWidth/100.0f;
        m_ProjectileMaxRange=ProjectileRange/100.0f;
        m_SlashSpeed=SlashSpeed/100.0f;
        m_SlashWidth=SlashWidth/100.0f;
        m_SlashMaxRange=SlashRange/100.0f;
        m_CurrentSpeed=m_ProjectileSpeed;
        m_Direction=Direction;
        m_Direction.y=0.0f;
        m_Direction.Normalize();
        m_GoRight=GoRight;
        transform.localScale=new Vector3(m_ProjectileWidth, transform.localScale.y, m_ProjectileWidth);
        m_NormalCharges=(int)NormalCharges;
        m_CorruptedCharges=(int)CorruptedCharges;
    }

    IEnumerator StartSlash() 
    {
        m_Exploded=true;
        m_InitPos=transform.position;
        yield return new WaitForSeconds(0.05f);
        m_InitialProjectile.SetActive(false);
        m_SlashProjectile.SetActive(true);
        transform.localScale=new Vector3(m_SlashWidth, transform.localScale.y, m_SlashWidth);
        m_Direction=-m_Direction;
        Vector3 l_Axis=transform.up;
        if(!m_GoRight) 
            l_Axis=-l_Axis;
        m_Direction=Quaternion.AngleAxis(35, l_Axis)*m_Direction;
        transform.forward=m_Direction;
        m_CurrentSpeed=m_SlashSpeed;
    }

    private void OnTriggerEnter(Collider other)
	{
        if(!IsSpawned||!HasAuthority)
        {
            return;
        }

        if(other.CompareTag("Enemy")) 
        {
            if(!m_Exploded)
                StartCoroutine(StartSlash());
            if(other.TryGetComponent(out ITakeDamage Enemy))
	        {
		        Enemy.TakeDamage(m_Damage, 0.0f, false, m_Player.m_CharacterStats.GetPlayerName(), m_Player.gameObject);
                if(Enemy.GetCharacterStats().GetCorruptedHealth()>0.0f) 
                    m_EStacksOnHit?.Invoke(m_CorruptedCharges);
                else 
                    m_EStacksOnHit?.Invoke(m_NormalCharges);
                m_OnDamageEnemy?.Invoke(other.gameObject);
            }
        } 
	}
}
