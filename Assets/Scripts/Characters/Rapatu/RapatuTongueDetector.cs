using UnityEngine;

public class RapatuTongueDetector : MonoBehaviour
{
    public RapatuCharacterController m_RapatuCharacterController;

    private void OnTriggerEnter(Collider _Collider)
    {
        m_RapatuCharacterController.TongueDetectCollider(_Collider);
    }
}
