
using UnityEngine;

public class TimedSoulTheftBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;

    WilldurrCharacterController m_WilldurrCharacterController;
    public TimedSoulTheftBuff(WilldurrCharacterController _WilldurrCharacterController, float Duration, Buff buff, GameObject obj) : base(buff)
    {

        m_WilldurrCharacterController = _WilldurrCharacterController;
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }

    //Start TimedSoulTheftBuff
    protected override void ApplyEffect()
    {
        if (m_StatsComponent != null)
        {
            m_StatsComponent.SetCanSoulTheft(true, m_WilldurrCharacterController);
        }
    }

    //End TimedSoulTheftBuff
    public override void End()
    {
        if (m_StatsComponent != null)
        {
            m_StatsComponent.SetCanSoulTheft(false, null);
        }
    }

    //Update TimedSoulTheftBuff
    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent != null)
        {
        }
    }
}