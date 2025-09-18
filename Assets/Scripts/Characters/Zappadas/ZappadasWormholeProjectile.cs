using System.Collections.Generic;
using UnityEngine;

public class ZappadasWormholeProjectile : MonoBehaviour
{
    float m_ProjectileExitSpeed;
    float m_ProjectileExitRange;
    float m_ProjectileExitExtraHitbox;
    Vector3 m_EndDirection;
    Vector3 m_StartPosition;
    ZappadasCharacterController m_CharacterController;
    DamageInstance m_DamageInstance;

    public void Set(float _ProjectileExitSpeed, float _ProjectileExitRange, float _ProjectileExitExtraHitbox, Vector3 EndDirection, ZappadasCharacterController _CharacterController, DamageInstance Instance)
    {
        m_ProjectileExitSpeed = _ProjectileExitSpeed;
        m_ProjectileExitRange = _ProjectileExitRange;
        m_ProjectileExitExtraHitbox = _ProjectileExitExtraHitbox / 100f;
        m_EndDirection = EndDirection.normalized; 
        m_StartPosition = transform.position;
        m_CharacterController = _CharacterController;
        transform.localScale *= (m_ProjectileExitExtraHitbox + 1);
        m_DamageInstance=Instance;
        m_DamageInstance.m_Id=m_CharacterController.GetCharacterStats().GetPlayerName();
        m_DamageInstance.m_SourceObject=m_CharacterController.gameObject;
    }

    void Update()
    {
        transform.position += m_EndDirection * (m_ProjectileExitSpeed/100) * Time.deltaTime;

        float distanceTravelled = Vector3.Distance(m_StartPosition, transform.position);
        if (distanceTravelled >= m_ProjectileExitRange/100)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider OtherEntity)
    {
        if (OtherEntity.GetComponent<CharacterMaster>())
            return;

        if (OtherEntity.TryGetComponent(out ITakeDamage takeDamage))
        {
            if (OtherEntity.TryGetComponent(out ITakeDamage Enemy))
            {
                Enemy.TakeDamage(m_DamageInstance);
                m_CharacterController.AddmDarkPowerDamageLightlessWithSkill(OtherEntity.gameObject);
            }
            Destroy(gameObject);
        }
    }

}

