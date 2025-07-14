using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpeedBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;
    private float m_Timer;
    private float m_LastAppliedValue = 0f;
    private float m_AccumulatedValue;

    public TimedSpeedBuff(float Duration, Buff _Buff, GameObject obj) : base(_Buff)
    {
        _Buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }

    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
            SpeedBuff l_SpeedBuff = (SpeedBuff)m_Buff;

            m_Timer = (l_SpeedBuff.m_WayOfChanging == SpeedBuff.WayOfChanging.VALUETOVALUEWITHTIME)
                ? l_SpeedBuff.m_ValueToValueTime
                : l_SpeedBuff.m_Duration;

            m_LastAppliedValue = l_SpeedBuff.m_SpeedIncrease;

            switch (l_SpeedBuff.m_SpeedType)
            {
                case SpeedBuff.SpeedType.FLAT:
                    m_StatsComponent.SetMovSpeedBonusFlat(m_StatsComponent.GetMovSpeedBonusFlat()+m_LastAppliedValue);
                    m_AccumulatedValue+=l_SpeedBuff.m_SpeedIncrease;
                    break;
                case SpeedBuff.SpeedType.ADDITIVE:
                    m_StatsComponent.SetMovSpeedBonusAddi(m_StatsComponent.GetMovSpeedBonusAddi()+m_LastAppliedValue);
                    m_AccumulatedValue+=l_SpeedBuff.m_SpeedIncrease;
                    break;
                case SpeedBuff.SpeedType.MULTIPLICATIVE:
                    m_StatsComponent.AddMovSpeedBonusMulti(l_SpeedBuff.m_BuffName, m_LastAppliedValue);
                    break;
            }
        }
    }

    public override void LooseStack()
    {
        base.LooseStack();
        if (m_StatsComponent != null && !m_IsFinished)
        {
            SpeedBuff l_SpeedBuff = (SpeedBuff)m_Buff;

            switch (l_SpeedBuff.m_SpeedType)
            {
                case SpeedBuff.SpeedType.FLAT:
                    m_StatsComponent.SetMovSpeedBonusFlat(m_StatsComponent.GetMovSpeedBonusFlat()-m_LastAppliedValue);
                    m_AccumulatedValue+=l_SpeedBuff.m_SpeedIncrease;
                    break;
                case SpeedBuff.SpeedType.ADDITIVE:
                    m_StatsComponent.SetMovSpeedBonusAddi(m_StatsComponent.GetMovSpeedBonusAddi()-m_LastAppliedValue);
                    m_AccumulatedValue+=l_SpeedBuff.m_SpeedIncrease;
                    break;
                case SpeedBuff.SpeedType.MULTIPLICATIVE:
                    m_StatsComponent.AddMovSpeedBonusMulti(l_SpeedBuff.m_BuffName, -m_LastAppliedValue);
                    break;
            }
        }
    }

    public override void End()
    { 
        base.End();

        if (m_StatsComponent != null)
        {
            SpeedBuff l_SpeedBuff = (SpeedBuff)m_Buff;

            switch (l_SpeedBuff.m_SpeedType)
            {
                case SpeedBuff.SpeedType.FLAT:
                    m_StatsComponent.SetMovSpeedBonusFlat(m_StatsComponent.GetMovSpeedBonusFlat()-m_AccumulatedValue);
                    break;
                case SpeedBuff.SpeedType.ADDITIVE:
                    m_StatsComponent.SetMovSpeedBonusAddi(m_StatsComponent.GetMovSpeedBonusAddi()-m_AccumulatedValue);
                    break;
                case SpeedBuff.SpeedType.MULTIPLICATIVE:
                    m_StatsComponent.RemoveMovSpeedBonusMulti(l_SpeedBuff.m_BuffName);
                    break;
            }
        }
    }

    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent == null) return;

        SpeedBuff l_SpeedBuff = (SpeedBuff)m_Buff;
        float l_NewValue = m_LastAppliedValue;

        switch (l_SpeedBuff.m_WayOfChanging)
        {
            case SpeedBuff.WayOfChanging.NOCHANGEINTIME:
                return;

            case SpeedBuff.WayOfChanging.DECREASE:
                m_Timer -= delta;
                float l_DecreasePercent = m_Timer / l_SpeedBuff.m_Duration;
                l_NewValue = l_SpeedBuff.m_SpeedIncrease * Mathf.Clamp01(l_DecreasePercent);
                break;

            case SpeedBuff.WayOfChanging.INCREASE:
                m_Timer -= delta;
                float l_IncreasePercent = 1f - (m_Timer / l_SpeedBuff.m_Duration);
                l_NewValue = l_SpeedBuff.m_SpeedIncrease * Mathf.Clamp01(l_IncreasePercent);
                break;

            case SpeedBuff.WayOfChanging.VALUETOVALUE:
                float l_Progress = 1f - (m_Timer / l_SpeedBuff.m_Duration);
                l_NewValue = Mathf.Lerp(l_SpeedBuff.m_Value1, l_SpeedBuff.m_Value2, Mathf.Clamp01(l_Progress));
                break;

            case SpeedBuff.WayOfChanging.VALUETOVALUEWITHTIME:
                m_Timer -= delta;
                float l_TimeProgress = 1f - (m_Timer / l_SpeedBuff.m_ValueToValueTime);
                l_NewValue = Mathf.Lerp(l_SpeedBuff.m_Value1, l_SpeedBuff.m_Value2, Mathf.Clamp01(l_TimeProgress));
                break;
        }

        switch (l_SpeedBuff.m_SpeedType)
        {
            case SpeedBuff.SpeedType.FLAT:
                m_StatsComponent.SetMovSpeedBonusFlat(-m_LastAppliedValue);
                m_StatsComponent.SetMovSpeedBonusFlat((int)l_NewValue);
                break;

            case SpeedBuff.SpeedType.ADDITIVE:
                m_StatsComponent.SetMovSpeedBonusAddi(-m_LastAppliedValue);
                m_StatsComponent.SetMovSpeedBonusAddi((int)l_NewValue);
                break;

            case SpeedBuff.SpeedType.MULTIPLICATIVE:
                m_StatsComponent.RemoveMovSpeedBonusMulti(l_SpeedBuff.m_BuffName);
                m_StatsComponent.AddMovSpeedBonusMulti(l_SpeedBuff.m_BuffName, (int)l_NewValue);
                break;
        }

        m_LastAppliedValue = (int)l_NewValue;
    }
}

