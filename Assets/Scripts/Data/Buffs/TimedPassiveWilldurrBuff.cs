
using UnityEngine;

public class TimedPassiveWilldurrBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;
    public TimedPassiveWilldurrBuff(float Duration, Buff buff, GameObject obj) : base(buff, obj)
    {
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }

    //Start TimedPassiveWilldurrBuff
    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
        }
    }

    //End TimedPassiveWilldurrBuff
    public override void End()
    {
        if (m_StatsComponent != null)
        {
        }
    }

    //Update TimedPassiveWilldurrBuff
    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent != null)
        {
        }
    }
}