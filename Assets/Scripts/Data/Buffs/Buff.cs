using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Buff : ScriptableObject
{
    public string m_BuffName;
    public Sprite m_BuffSprite;
    public bool m_IsDurationRefreshed;
    public bool m_IsEffectStacked;
    public bool m_InfiniteDuration;
    [HideInInspector] 
    public int m_MaxStacks=1;    
    [HideInInspector] 
    public bool m_AreAllStacksLost;
    [HideInInspector] 
    public float m_StackLooseInterval;
    [HideInInspector]
    public float m_Duration;
    [HideInInspector]
    public float m_ValueAmount = -1f;
}

[CustomEditor(typeof(Buff))]
public class BuffEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Buff l_Buff=(Buff)target;
        if(l_Buff.m_IsEffectStacked)
        {   
            l_Buff.m_MaxStacks=EditorGUILayout.IntField("Max Stacks", l_Buff.m_MaxStacks);
            l_Buff.m_AreAllStacksLost=EditorGUILayout.Toggle("Are All Stacks Lost", l_Buff.m_AreAllStacksLost);
            if(!l_Buff.m_AreAllStacksLost)
                l_Buff.m_StackLooseInterval=EditorGUILayout.FloatField("Stack Loose Interval", l_Buff.m_StackLooseInterval);
        }
    }
}