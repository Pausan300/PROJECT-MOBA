using UnityEditor.SceneManagement;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void Awake()
    {
        if (GetComponent<Collider>() == null) {
            SphereCollider l_Collider = gameObject.AddComponent<SphereCollider>();
            
        }
        SetCanBeDeleted(m_CanBeDeletedStart);
    }

    float m_HitboxRadius;
    float m_Damage;
    bool m_CanBeDeleted = false; // Si puede ser borrado por ejemlo en skill W de Zappadas cuando pasa por el portal. En general siempre true.

    [Header("Siempre true a no ser que nites")]
    public bool m_CanBeDeletedStart = true;
    public void SetHitboxRadius(float _HitboxRadius)
    {
        m_HitboxRadius = _HitboxRadius;
    }
    public float GetHitboxRadius()
    {
        return m_HitboxRadius;
    }
    public void SetDamage(float _Damage)
    {
        m_Damage = _Damage;
    }
    public float GetDamage()
    {
        return m_Damage;
    }
    public void SetCanBeDeleted(bool _CanBeDeleted)
    {
        m_CanBeDeleted = _CanBeDeleted;
    }
    public bool GetCanBeDeleted()
    {
        return m_CanBeDeleted;
    }
}
