
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/DarkPowerBuff")]
public class DarkPowerBuff : Buff
{
    public TimedBuff InitializeBuff(int DarkPowerValue, GameObject obj)
    {
        m_ValueAmount = DarkPowerValue;
        return new TimedDarkPowerBuff(float.MaxValue, this, obj);
    }
}