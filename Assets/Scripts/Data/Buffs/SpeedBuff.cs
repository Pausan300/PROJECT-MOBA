using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Buffs/SpeedBuff")]
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
    [HideInInspector]
    public float m_SpeedIncrease;

    public TimedBuff InitializeBuff(float Duration, float SpeedIncrease, GameObject obj)
    {
        m_Duration=Duration;
        m_SpeedIncrease=SpeedIncrease;
        return new TimedSpeedBuff(Duration, this, obj);
    }
}