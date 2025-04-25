using UnityEngine;

public class WilldurrHeadR : MonoBehaviour
{
    public SphereCollider m_Collider;

    public void SetRadiusHitbox(float Radius)
    {
        m_Collider.radius = Radius / 100;
    }
}
