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
    float m_Damage;

    public void Set(float _ProjectileExitSpeed, float _ProjectileExitRange, float _ProjectileExitExtraHitbox, Vector3 EndDirection, ZappadasCharacterController _CharacterController, float _Damage)
    {
        m_ProjectileExitSpeed = _ProjectileExitSpeed;
        m_ProjectileExitRange = _ProjectileExitRange;
        m_ProjectileExitExtraHitbox = _ProjectileExitExtraHitbox / 100f;
        m_EndDirection = EndDirection.normalized; 
        m_StartPosition = transform.position;
        m_CharacterController = _CharacterController;
        transform.localScale *= (m_ProjectileExitExtraHitbox + 1);
        m_Damage = _Damage;
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
                float l_Damage = m_Damage;
                Debug.Log("TAKEN " + l_Damage + " DAMAGE");
                Enemy.TakeDamage(0, l_Damage, false, m_CharacterController.m_CharacterStats.GetPlayerName(), m_CharacterController.gameObject);
                m_CharacterController.AddmDarkPowerDamageLightlessWithSkill(OtherEntity.gameObject);
            }
            Destroy(gameObject);
        }
    }

}

