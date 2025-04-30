
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/SoulTheftBuff")]
public class SoulTheftBuff : Buff
{
    public TimedBuff InitializeBuff(WilldurrCharacterController _WilldurrCharacterController, float Duration, GameObject obj)
    {
        m_Duration = Duration;
        return new TimedSoulTheftBuff(_WilldurrCharacterController, Duration, this, obj);
    }
}