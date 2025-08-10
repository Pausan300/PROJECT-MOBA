using UnityEngine;

public class ZappadasRProjectil : MonoBehaviour
{
    ZappadasCharacterController m_CharacterController;
    public void SetProjectil(ZappadasCharacterController characterController)
    {
        m_CharacterController = characterController;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_CharacterController.RTriggerEnter(other);
    }
}
