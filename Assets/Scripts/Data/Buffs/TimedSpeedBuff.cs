using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpeedBuff : TimedBuff
{
    private readonly CharacterStatsBlock m_StatsComponent;

    public TimedSpeedBuff(float Duration, Buff buff, GameObject obj) : base(buff, obj)
    {
        buff.m_Duration=Duration;
        if(obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent=Entity.GetCharacterStats();
    }
    protected override void ApplyEffect()
    {
        if(m_StatsComponent!=null)
        {
            SpeedBuff l_SpeedBuff=(SpeedBuff) m_Buff;
		    switch(l_SpeedBuff.m_SpeedType)
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
        if(m_StatsComponent!=null)
        {
            SpeedBuff l_SpeedBuff=(SpeedBuff) m_Buff;
            switch(l_SpeedBuff.m_SpeedType)
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
    protected override void ApplyTick()
    {
    }
}