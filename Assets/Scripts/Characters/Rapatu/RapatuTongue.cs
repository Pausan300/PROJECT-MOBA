using UnityEngine;

public class RapatuTongue : MonoBehaviour
{
    RapatuCharacterController m_RapatuCharacterController;
    public LineRenderer m_TongueLineRenderer;
    public GameObject m_DamageZone;

    public void SetTongue(RapatuCharacterController _RapatuCharacterController)
    {
        m_RapatuCharacterController = _RapatuCharacterController;
    }
    public Transform GetTongueEndPos()
    {
        return transform;
    }
    public GameObject GetDamageZone()
    {
        return m_DamageZone;
    }
    public LineRenderer GetTongueLineRenderer()
    {
        return m_TongueLineRenderer;
    }
    private void OnTriggerEnter(Collider _Collider)
    {
        m_RapatuCharacterController.TongueDetectCollider(_Collider);
    }
}
