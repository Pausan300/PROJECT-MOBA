using UnityEngine;

public class WilldurrWScythe : MonoBehaviour
{
    [HideInInspector]
    public WilldurrCharacterController m_CharacterController;
    public GameObject m_ReactivedGO;
    public SphereCollider m_Collider;
    private void Awake()
    {
        m_ReactivedGO.SetActive(false);
        m_Collider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        m_CharacterController.OnTriggerEnterWReactivation(other);
    }

    public void SetReactive(float HitboxRatio)
    {
        m_Collider.enabled = true;
        m_Collider.radius = HitboxRatio / 100;

        HitboxRatio *= 2;
        m_ReactivedGO.SetActive(true);
        m_ReactivedGO.transform.localScale = new Vector3(HitboxRatio / 100, HitboxRatio / 100, HitboxRatio / 100);
    }
}
