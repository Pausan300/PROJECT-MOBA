using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    CharacterStats m_CharacterStats;
    public float m_HeightAboveGround;

    bool m_MoveToPoint;
    List<Transform> m_TargetPoints;
    int m_TargetIndex; 
 
    void Start()
    {
        m_CharacterStats=GetComponent<CharacterStats>();

        Vector3 l_Pos=transform.position;
        l_Pos.y=m_HeightAboveGround;
        transform.position=l_Pos;
    }

    void Update()
    {
        if (m_MoveToPoint && !m_CharacterStats.GetImmobilized() && !m_CharacterStats.GetStuned())
        {
            Vector3 l_Pos=transform.position;
            l_Pos.y=0.0f;
            if (Vector3.Distance(l_Pos, m_TargetPoints[m_TargetIndex].position) > 0.5f)
            {
                Vector3 l_TargetPos=m_TargetPoints[m_TargetIndex].position;
                l_TargetPos.y=m_HeightAboveGround;
                Vector3 l_Dir = l_TargetPos - transform.position;
                l_Dir.Normalize();
                transform.position += l_Dir * (m_CharacterStats.GetMovSpeed() / 100.0f) * Time.deltaTime;
                transform.forward = l_Dir;
            }
            else 
            {
                if(m_TargetIndex+1>=m_TargetPoints.Count)
                    m_MoveToPoint=false;
                else
                    m_TargetIndex++;
            }
        }
    }

    //GETTERS & SETTERS
    public void SetMovement(bool Move)
    {
        m_MoveToPoint=Move;
    }
    public void SetMovementPoints(List<Transform> Points)
    {
        m_TargetPoints=Points;
    }
}
