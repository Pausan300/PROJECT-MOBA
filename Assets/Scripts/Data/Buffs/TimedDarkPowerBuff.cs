
using UnityEngine;

public class TimedDarkPowerBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;

    public TimedDarkPowerBuff(float Duration, Buff buff, GameObject obj) : base(buff, obj)
    {
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }

    //Start TimedDarkPowerBuff
    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
        }
    }

    //End TimedDarkPowerBuff
    public override void End()
    {
        if (m_StatsComponent != null)
        {
        }
    }

    //Update TimedDarkPowerBuff
    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent != null)
        {
        }
    }
}