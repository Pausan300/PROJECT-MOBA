using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Buffs/SpeedBuff")]
public class SpeedBuff : Buff
{
    [Header("Speed stats")]
    public SpeedType m_SpeedType;
    public enum SpeedType
    {
        FLAT,
        ADDITIVE,
        MULTIPLICATIVE
    }
    public WayOfChanging m_WayOfChanging;
    public enum WayOfChanging
    {
        NOCHANGEINTIME,
        DECREASE,
        INCREASE,
        VALUETOVALUE,
        VALUETOVALUEWITHTIME,
    }
    [Header("VALUETOVALUE or VALUETOVALUEWITHTIME")]
    public float m_Value1;
    public float m_Value2;
    [Header("VALUETOVALUEWITHTIME")]
    public float m_ValueToValueTime;

    [HideInInspector]
    public float m_SpeedIncrease;

    public TimedBuff InitializeBuff(float Duration, float SpeedIncrease, GameObject obj)
    {
        m_Duration = Duration;
        m_SpeedIncrease = SpeedIncrease;
        return new TimedSpeedBuff(Duration, this, obj);
    }
}

[CustomEditor(typeof(SpeedBuff))]
public class SpeedBuffEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Buff l_Buff = (Buff)target;
        if(l_Buff.m_IsEffectStacked)
        {   
            l_Buff.m_MaxStacks=EditorGUILayout.IntField("Max Stacks", l_Buff.m_MaxStacks);
        }
    }
}
