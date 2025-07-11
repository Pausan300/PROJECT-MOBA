using UnityEngine;

public class TimedImmobilizeBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;
    float m_Timer;

    public TimedImmobilizeBuff(float Duration, Buff buff, GameObject obj) : base(buff)
    {
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }
    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
            m_StatsComponent.SetImmobilized(true);
        }
    }
    public override void End()
    {
        if (m_StatsComponent != null)
        {
            m_StatsComponent.SetImmobilized(false);
        }
    }
    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent != null)
        {
            
        }

    }
}
