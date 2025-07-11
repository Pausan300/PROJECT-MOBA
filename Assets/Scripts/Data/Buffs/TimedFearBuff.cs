
using UnityEngine;

public class TimedFearBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;
    Vector3 m_FearPosition;

    public TimedFearBuff(Vector3 FearPosition, float Duration, Buff buff, GameObject obj) : base(buff)
    {
        m_FearPosition = FearPosition;
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }

    //Start TimedFearBuff
    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
            m_StatsComponent.SetScared(true, m_FearPosition);
            m_StatsComponent.SetImmobilized(true);
        }
    }

    //End TimedFearBuff
    public override void End()
    {
        if (m_StatsComponent != null)
        {
            m_StatsComponent.SetScared(false, Vector3.zero);
            m_StatsComponent.SetImmobilized(false);
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