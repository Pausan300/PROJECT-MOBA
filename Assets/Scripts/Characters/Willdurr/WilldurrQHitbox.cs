using UnityEngine;

public class WilldurrQHitbox : MonoBehaviour
{
    public WilldurrCharacterController m_WilldurrCharacterController;
    private void OnTriggerEnter(Collider other)
    {
        m_WilldurrCharacterController.OnTriggerEnterQ(other);
    }
}
