using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    float m_HitboxRadius;
    bool m_CanBeDeleted = false; // Si puede ser borrado por ejemlo en skill W de Zappadas cuando pasa por el portal. En general siempre true.

    [Header("Siempre true a no ser que nites")]
    public bool m_CanBeDeletedStart = true;

    DamageInstance m_DamageInstance;

    private void Awake()
    {
        if (GetComponent<Collider>() == null) {
            SphereCollider l_Collider = gameObject.AddComponent<SphereCollider>();
        }
        SetCanBeDeleted(m_CanBeDeletedStart);
    }

    public void SetHitboxRadius(float _HitboxRadius)
    {
        m_HitboxRadius = _HitboxRadius;
    }
    public float GetHitboxRadius()
    {
        return m_HitboxRadius;
    }
    public void SetCanBeDeleted(bool _CanBeDeleted)
    {
        m_CanBeDeleted = _CanBeDeleted;
    }
    public bool GetCanBeDeleted()
    {
        return m_CanBeDeleted;
    }
    public void SetDamageInstance(DamageInstance Instance) 
    {
        m_DamageInstance=Instance;
    }
    public DamageInstance GetDamageInstance() 
    {
        return m_DamageInstance;
    }
}
