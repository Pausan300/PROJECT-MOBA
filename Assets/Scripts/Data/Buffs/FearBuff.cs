
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/FearBuff")]
public class FearBuff : Buff
{
    public TimedBuff InitializeBuff(Vector3 FearPosition, float Duration, GameObject obj)
    {
        m_Duration = Duration;
        return new TimedFearBuff(FearPosition, Duration, this, obj);
    }
}