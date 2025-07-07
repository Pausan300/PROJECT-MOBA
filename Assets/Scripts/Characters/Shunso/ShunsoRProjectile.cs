using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ShunsoRProjectile : NetworkBehaviour
{
    public SpriteRenderer m_ByteAreaSprite;
    ShunsoCharacterController m_Player;

    float m_Damage;
    float m_Speed;
    float m_ByteRadius;
    Vector3 m_InitialPos;
    Vector3 m_TargetPosition;
    Vector3 m_Direction;
    bool m_Exploding;

    public event Action<int> m_EStacksOnHit;

    
    void Start()
    {
    }

    void Update()
    {
        if(!m_Exploding) 
        {
            if(Vector3.Distance(transform.position, m_TargetPosition)>0.25)
                transform.position+=m_Direction*m_Speed*Time.deltaTime;
            else
                StartCoroutine(DoByte());
        }
    }

    public void SetStats(ShunsoCharacterController Player, float Damage, float Speed, float ByteRadius, Vector3 TargetPos) 
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

        float l_SpriteWidth=m_ByteAreaSprite.sprite.rect.width*transform.localScale.x;
        float l_NewWidth=ByteRadius*2.0f/l_SpriteWidth;
        m_ByteAreaSprite.transform.localScale=new Vector2(l_NewWidth, l_NewWidth);
        m_ByteAreaSprite.gameObject.SetActive(false);
    }

    IEnumerator DoByte() 
    {
        m_Exploding=true;
        m_ByteAreaSprite.gameObject.SetActive(true);
        Collider l_Collider=GetComponent<Collider>();
        Collider[] l_HitColliders=Physics.OverlapSphere(l_Collider.bounds.center, m_ByteRadius/100.0f, m_Player.m_DamageLayerMask);
        float l_ExtraDamage=Mathf.Round(Vector3.Distance(m_InitialPos, transform.position))*5.0f;
		foreach(Collider Entity in l_HitColliders)
		{
            if(Entity.TryGetComponent(out ITakeDamage Enemy))
	        {
		        Enemy.TakeDamage(m_Damage+l_ExtraDamage, 0.0f, false, m_Player.m_CharacterStats.GetPlayerName());
                  if(Enemy.GetCharacterStats().GetCorruptedHealth()>0.0f) 
                    m_EStacksOnHit?.Invoke(4);
                else
                    m_EStacksOnHit?.Invoke(1);
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
            StartCoroutine(DoByte());
	}
}
