using System.Collections;
using UnityEngine;

public class ZappadasWWormhole : MonoBehaviour
{
    public GameObject m_WWormholeEndPrefab;
    public GameObject m_ProjectileUpgradePrefab;
    public CapsuleCollider m_CapsuleCollider;
    GameObject m_WWormholeEnd;
    Vector3 m_EndDirection;
    bool m_SkillUpgraded;
    float m_ProjectileExitSpeed;
    float m_ProjectileExitRange;
    float m_ProjectileExitExtraHitbox;
    float m_DarkPowerDamage;
    float m_DarkPowerRange;
    ZappadasCharacterController m_CharacterController;
    bool m_WSkillUpgrade;

    public void SetWWormhole(Vector3 EndPosition, Vector3 EndDirection, float WormholeDuration, bool SkillUpgraded, float _ProjectileExitSpeed, float _ProjectileExitRange, float _ProjectileExitExtraHitbox, float _DarkPowerDamage, float _DarkPowerRange, float _WWormholeStartHitboxRadius, ZappadasCharacterController _CharacterController, bool _WSkillUpgrade)
    {
        StartCoroutine(DestroyWormhole(WormholeDuration));
        EndDirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(EndDirection.normalized);
        m_WWormholeEnd = Instantiate(m_WWormholeEndPrefab, EndPosition, rotation);
        StartCoroutine(DestroyWormhole(WormholeDuration));
        m_CapsuleCollider.radius = _WWormholeStartHitboxRadius/100;
        m_EndDirection = EndDirection;
        m_SkillUpgraded = SkillUpgraded;
        m_ProjectileExitSpeed = _ProjectileExitSpeed;
        m_ProjectileExitRange = _ProjectileExitRange;
        m_ProjectileExitExtraHitbox = _ProjectileExitExtraHitbox;
        m_DarkPowerDamage = _DarkPowerDamage;
        m_DarkPowerRange = _DarkPowerRange;
        m_CharacterController = _CharacterController;
        m_WSkillUpgrade = _WSkillUpgrade;
    }

    IEnumerator DestroyWormhole(float WormholeDuration)
    {
        yield return new WaitForSeconds(WormholeDuration);

        Destroy(m_WWormholeEnd);
        Destroy(gameObject);

    }


    private void OnTriggerStay(Collider other)
    {
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            if (projectile.GetCanBeDeleted())
            {
                float l_Damage = projectile.GetDamage();
                GameObject l_ProjectileCopy = projectile.gameObject;
                projectile.enabled = false;
                l_ProjectileCopy.transform.position = m_WWormholeEnd.GetComponent<ZappadasWWormholeExit>().GetExitPos();
                ZappadasWormholeProjectile l_ZappadasWormholeProjectile = l_ProjectileCopy.AddComponent<ZappadasWormholeProjectile>();
                l_ZappadasWormholeProjectile.Set(m_ProjectileExitSpeed, m_ProjectileExitRange, m_ProjectileExitExtraHitbox, m_EndDirection, m_CharacterController, l_Damage);

                if (m_WSkillUpgrade)
                {
                    GameObject l_ProjectileUpgrade = Instantiate(m_ProjectileUpgradePrefab, m_WWormholeEnd.GetComponent<ZappadasWWormholeExit>().GetExitPos(), Quaternion.identity);
                    l_ProjectileUpgrade.GetComponent<ZappadasQProjectileUpgrade>().SetProjectilUpGrade(m_DarkPowerDamage, m_DarkPowerRange / m_ProjectileExitSpeed, m_DarkPowerRange, 0, m_CharacterController.m_DamageLayerMask, 1, other, m_CharacterController.GetCharacterStats(), m_CharacterController);

                }


            }
        }
    }

}
