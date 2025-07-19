using UnityEngine;

public class ZappadasQProjectile : Projectile
{
    bool m_Move;

    float m_ExplosionHitboxQ = 80f;
    float m_RangeQ = 1300f;
    float m_ProjectileSpeedQ = 900f;

    float m_DistanceTraveled = 0;
    Vector3 m_Direction;

    ZappadasCharacterController m_CharacterController;


    private void Awake()
    {
        m_Move = false;
    }

    public void SetProjectile(float damage, float ProjectileHitbox, float Range, float ProjectileSpeed, Vector3 Direction, ZappadasCharacterController _CharacterController)
    {
        SetDamage(damage);
        SetHitboxRadius((ProjectileHitbox/100)/2);
        transform.localScale = new Vector3(ProjectileHitbox / 100, ProjectileHitbox / 100, ProjectileHitbox / 100);
        m_RangeQ = Range;
        m_ProjectileSpeedQ = ProjectileSpeed;

        m_DistanceTraveled = 0;
        m_Direction = Direction;

        m_Move = true;

        m_CharacterController = _CharacterController;
        SetCanBeDeleted(true);
    }

    private void Update()
    {
        if (m_Move)
        {
            float distanceThisFrame = (m_ProjectileSpeedQ / 100) * Time.deltaTime;

            transform.position += m_Direction * distanceThisFrame;
            m_DistanceTraveled += distanceThisFrame;

            if (m_DistanceTraveled >= m_RangeQ / 100)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_CharacterController != null)
            m_CharacterController.OnTriggerEnterQ(other, gameObject);
    }

}
