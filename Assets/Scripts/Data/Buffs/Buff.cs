using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : ScriptableObject
{
    public string m_BuffName; 
    public Sprite m_BuffSprite;
    public bool m_IsDurationRefreshed;
    public bool m_IsEffectStacked;
    [HideInInspector]
    public float m_Duration;
}