
using UnityEngine;

public class TimedADBuff : TimedBuff
{
    private readonly CharacterStats m_StatsComponent;

    float m_DamageIncrease;

    public TimedADBuff(float Duration, float Damage, Buff buff, GameObject obj) : base(buff, obj)
    {
        buff.m_Duration=Duration;
        m_DamageIncrease=Damage;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent=Entity.GetCharacterStats();
    }

    protected override void ApplyEffect()
    {
        if (m_StatsComponent!=null)
        {
            ADBuff l_SpeedBuff=(ADBuff)m_Buff;
            switch (l_SpeedBuff.m_ADType)
            {
                case ADBuff.ADType.FLAT:
                    m_StatsComponent.SetBonusAttackDamage(m_StatsComponent.GetBonusAttackDamage()+m_DamageIncrease);
                    break;
                case ADBuff.ADType.ADDITIVE:
                    break;
                case ADBuff.ADType.MULTIPLICATIVE:
                    break;
            }
        }
    }

    public override void End()
    {
        if (m_StatsComponent!=null)
        {
            ADBuff l_SpeedBuff=(ADBuff)m_Buff;
            switch (l_SpeedBuff.m_ADType)
            {
                case ADBuff.ADType.FLAT:
                    m_StatsComponent.SetBonusAttackDamage(m_StatsComponent.GetBonusAttackDamage()-m_DamageIncrease);
                    break;
                case ADBuff.ADType.ADDITIVE:
                    break;
                case ADBuff.ADType.MULTIPLICATIVE:
                    break;
            }
        }
    }

    protected override void ApplyTick(float delta)
    {
        if (m_StatsComponent!=null)
        {
        }
    }
}