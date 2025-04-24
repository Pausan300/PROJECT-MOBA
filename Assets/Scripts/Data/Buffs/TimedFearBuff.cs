
using UnityEngine;

public class TimedFearBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;

    public TimedFearBuff(float Duration, Buff buff, GameObject obj) : base(buff, obj)
    {
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }

    //Start TimedFearBuff
    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
            m_StatsComponent.SetScared(true);
        }
    }

    //End TimedFearBuff
    public override void End()
    {
        if (m_StatsComponent != null)
        {
            m_StatsComponent.SetScared(false);
        }
    }

    //Update TimedFearBuff
    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent != null)
        {
        }
    }
}