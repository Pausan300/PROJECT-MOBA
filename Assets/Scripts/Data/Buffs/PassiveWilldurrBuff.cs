
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/PassiveWilldurrBuff")]
public class PassiveWilldurrBuff : Buff
{
    public TimedBuff InitializeBuff(int Souls, GameObject obj)
    {
        m_ValueAmount = Souls;
        return new TimedPassiveWilldurrBuff(float.MaxValue, this, obj);
    }
}