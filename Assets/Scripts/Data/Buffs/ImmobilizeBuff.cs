using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/ImmobilizeBuff")]
public class ImmobilizeBuff : Buff
{

    public TimedBuff InitializeBuff(float Duration, GameObject obj)
    {
        m_Duration = Duration;
        return new TimedImmobilizeBuff(Duration, this, obj);
    }
}
