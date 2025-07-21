using System.Collections;
using UnityEngine;

public class ZappadasEBall : MonoBehaviour
{
    bool m_Updated;
    ZappadasCharacterController m_CharacterController;
    int m_BallNum;

    public void SetBallE(float _TimeToDestroy, ZappadasCharacterController _CharacterController, bool Updated, int BallNum)
    {
        m_BallNum = BallNum;
        m_Updated = Updated;
        m_CharacterController = _CharacterController;
        StartCoroutine(DestroyObjectInTime(_TimeToDestroy));
    }
    
    IEnumerator DestroyObjectInTime(float time)
    {
        yield return new WaitForSeconds(time);
        m_CharacterController.ExploteEBall(m_Updated, transform.position, m_BallNum);
        Destroy(gameObject);
    }
}
