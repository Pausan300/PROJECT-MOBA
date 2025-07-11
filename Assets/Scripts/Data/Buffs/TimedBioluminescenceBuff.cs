using UnityEngine;

public class TimedBioluminescenceBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;
    float m_Timer;

    public TimedBioluminescenceBuff(float Duration, Buff buff, GameObject obj) : base(buff)
    {
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }
    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
            Debug.LogError("Start Bioluminescence Buff");
        }
    }
    public override void End()
    {
        if (m_StatsComponent != null)
        {
            Debug.LogError("End Bioluminescence Buff");
        }
    }
    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent != null)
        {

        }

    }
}
