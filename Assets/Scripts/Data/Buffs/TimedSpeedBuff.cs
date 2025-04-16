using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpeedBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;
    float m_Timer;

    public TimedSpeedBuff(float Duration, Buff buff, GameObject obj) : base(buff, obj)
    {
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }
    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
            SpeedBuff l_SpeedBuff = (SpeedBuff)m_Buff;
            m_Timer = l_SpeedBuff.m_Duration;
            switch (l_SpeedBuff.m_SpeedType)
            {
                case SpeedBuff.SpeedType.FLAT:
                    m_StatsComponent.AddMovSpeedBonusFlat(l_SpeedBuff.m_SpeedIncrease);
                    break;
                case SpeedBuff.SpeedType.ADDITIVE:
                    m_StatsComponent.AddMovSpeedBonusAddi(l_SpeedBuff.m_SpeedIncrease);
                    break;
                case SpeedBuff.SpeedType.MULTIPLICATIVE:
                    m_StatsComponent.AddMovSpeedBonusMulti(l_SpeedBuff.m_BuffName, l_SpeedBuff.m_SpeedIncrease);
                    break;
                default:
                    break;
            }
        }
    }
    public override void End()
    {
        if (m_StatsComponent != null)
        {
            SpeedBuff l_SpeedBuff = (SpeedBuff)m_Buff;
            switch (l_SpeedBuff.m_SpeedType)
            {
                case SpeedBuff.SpeedType.FLAT:
                    m_StatsComponent.AddMovSpeedBonusFlat(-l_SpeedBuff.m_SpeedIncrease);
                    break;
                case SpeedBuff.SpeedType.ADDITIVE:
                    m_StatsComponent.AddMovSpeedBonusAddi(-l_SpeedBuff.m_SpeedIncrease);
                    break;
                case SpeedBuff.SpeedType.MULTIPLICATIVE:
                    m_StatsComponent.RemoveMovSpeedBonusMulti(l_SpeedBuff.m_BuffName);
                    break;
                default:
                    break;
            }
        }
    }
    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent != null)
        {
            SpeedBuff l_SpeedBuff = (SpeedBuff)m_Buff;
            if (l_SpeedBuff.m_DecreaseInTime)
            {

                Debug.LogError("m_Timer: " + m_Timer);
                m_Timer -= delta;
                float l_BuffTimePercentage = m_Timer / l_SpeedBuff.m_Duration;
                Debug.LogError("l_BuffTimePercentage: " + l_BuffTimePercentage);

                switch (l_SpeedBuff.m_SpeedType)
                {
                    case SpeedBuff.SpeedType.FLAT:
                        m_StatsComponent.AddMovSpeedBonusFlat(-l_SpeedBuff.m_SpeedIncrease);
                        m_StatsComponent.AddMovSpeedBonusFlat((int)(l_SpeedBuff.m_SpeedIncrease * l_BuffTimePercentage));
                        break;
                    case SpeedBuff.SpeedType.ADDITIVE:
                        m_StatsComponent.AddMovSpeedBonusAddi(-l_SpeedBuff.m_SpeedIncrease);
                        m_StatsComponent.AddMovSpeedBonusAddi((int)(l_SpeedBuff.m_SpeedIncrease * l_BuffTimePercentage));
                        break;
                    case SpeedBuff.SpeedType.MULTIPLICATIVE:
                        m_StatsComponent.RemoveMovSpeedBonusMulti(l_SpeedBuff.m_BuffName);
                        m_StatsComponent.AddMovSpeedBonusMulti(l_SpeedBuff.m_BuffName, (int)(l_SpeedBuff.m_SpeedIncrease * l_BuffTimePercentage));
                        break;
                    default:
                        break;
                }
            }
        }

    }
}