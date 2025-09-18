using System;
using System.Collections;
using UnityEngine;

public class ShunsoRProjectile : Projectile
{
    public SpriteRenderer m_ByteAreaSprite;
    ShunsoCharacterController m_Player;

    float m_Damage;
    float m_Speed;
    float m_ByteRadius;
    int m_NormalCharges;
    int m_CorruptedCharges;
    Vector3 m_InitialPos;
    Vector3 m_TargetPosition;
    Vector3 m_Direction;
    bool m_Exploding;
    bool m_StopTraveling;

    public event Action<int> m_EStacksOnHit;
    public event Action<GameObject> m_OnDamageEnemy;

    
    void Update()
    {
        if(!m_StopTraveling && Vector3.Distance(transform.position, m_TargetPosition)>0.25)
            transform.position+=m_Direction*m_Speed*Time.deltaTime;
        else if(!m_Exploding)
            StartCoroutine(DoByte(false));
    }

    public void SetStats(ShunsoCharacterController Player, float Damage, float Speed, float ByteRadius, Vector3 TargetPos, float NormalCharges, float CorruptedCharges) 
    {
        m_InitialPos=transform.position;
        m_Player=Player;
        m_Damage=Damage;
        m_Speed=Speed/100.0f;
        m_ByteRadius=ByteRadius;
        m_TargetPosition=TargetPos;
        m_TargetPosition.y=transform.position.y;
        m_Direction=m_TargetPosition-transform.position;
        m_Direction.y=0.0f;
        m_Direction.Normalize();
        m_NormalCharges=(int)NormalCharges;
        m_CorruptedCharges=(int)CorruptedCharges;

        float l_SpriteWidth=m_ByteAreaSprite.sprite.rect.width*transform.localScale.x;
        float l_NewWidth=ByteRadius*2.0f/l_SpriteWidth;
        m_ByteAreaSprite.transform.localScale=new Vector2(l_NewWidth, l_NewWidth);
        m_ByteAreaSprite.gameObject.SetActive(false);

        SetDamageInstance(new DamageInstance(m_Damage, 0.0f, m_Player.GetCharacterStats().GetPlayerName(), m_Player.gameObject));
    }

    IEnumerator DoByte(bool EnemyHit) 
    {
        m_Exploding=true;
        if(EnemyHit)
            yield return new WaitForSeconds(0.1f);
        m_StopTraveling=true;
        m_ByteAreaSprite.gameObject.SetActive(true);
        Collider l_Collider=GetComponent<Collider>();
        Collider[] l_HitColliders=Physics.OverlapSphere(l_Collider.bounds.center, m_ByteRadius/100.0f, m_Player.m_DamageLayerMask);
        float l_ExtraDamage=Mathf.Round(Vector3.Distance(m_InitialPos, transform.position))*5.0f;
        GetDamageInstance().AddToBiggestDamage(l_ExtraDamage);
		foreach(Collider Entity in l_HitColliders)
		{
            if(Entity.TryGetComponent(out ITakeDamage Enemy) && Entity.transform.CompareTag("Enemy"))
	        {
		        Enemy.TakeDamage(GetDamageInstance());
                if(Enemy.GetCharacterStats().GetCorruptedHealth()>0.0f) 
                    m_EStacksOnHit?.Invoke(m_CorruptedCharges);
                else
                    m_EStacksOnHit?.Invoke(m_NormalCharges);
                m_OnDamageEnemy?.Invoke(Entity.gameObject);
            }
        }
        l_Collider.enabled=false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
	{
        if(!IsSpawned||!HasAuthority)
        {
            return;
        }

		if(other.CompareTag("Enemy") && !m_Exploding)
            StartCoroutine(DoByte(true));
	}
}
