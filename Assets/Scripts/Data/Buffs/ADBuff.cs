
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/ADBuff")]
public class ADBuff : Buff
{
    [Header("AD stats")]
    public ADType m_ADType;
    public enum ADType
    {
        FLAT,
        ADDITIVE,
        MULTIPLICATIVE
    }

    public TimedBuff InitializeBuff(float Duration, float Damage, GameObject obj)
    {
        m_Duration=Duration;
        return new TimedADBuff(Duration, Damage, this, obj);
    }
}