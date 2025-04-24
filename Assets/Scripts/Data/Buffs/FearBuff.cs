
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/FearBuff")]
public class FearBuff : Buff
{
    public TimedBuff InitializeBuff(float Duration, GameObject obj)
    {
        m_Duration = Duration;
        return new TimedFearBuff(Duration, this, obj);
    }
}