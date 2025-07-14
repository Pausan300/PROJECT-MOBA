using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName="Buffs/MarkBuff")]
public class MarkBuff : Buff
{
    [Header("Mark stats")]
    public int m_MaxMarks;
    public GameObject m_MarkObject;

    public TimedBuff InitializeBuff(float Duration, float Interval, GameObject Enemy)
    {
        m_Duration=Duration;
        m_EffectInterval=Interval;
        return new TimedMarkBuff(m_Duration, this, Enemy);
    }
}


[CustomEditor(typeof(MarkBuff))]
public class MarkBuffEditor : BuffEditor
{
}