using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/BioluminescenceBuff")]
public class BioluminescenceBuff : Buff
{

    public TimedBuff InitializeBuff(float Duration, GameObject obj)
    {
        m_Duration = Duration;
        return new TimedBioluminescenceBuff(Duration, this, obj);
    }
}
