using UnityEngine;

public class WilldurrRZone : MonoBehaviour
{
    [HideInInspector]
    public WilldurrCharacterController m_CharacterController;
    private void OnTriggerEnter(Collider other)
    {
        m_CharacterController.TriggerEnterRZone(other);
    }
    private void OnTriggerExit(Collider other)
    {
        m_CharacterController.TriggerExitRZone(other);

    }
}
