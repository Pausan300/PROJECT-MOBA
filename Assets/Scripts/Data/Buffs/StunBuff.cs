using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/StunBuff")]
public class StunBuff : Buff
{

    public TimedBuff InitializeBuff(float Duration, GameObject obj)
    {
        m_Duration = Duration;
        return new TimedStunBuff(Duration, this, obj);
    }
}
