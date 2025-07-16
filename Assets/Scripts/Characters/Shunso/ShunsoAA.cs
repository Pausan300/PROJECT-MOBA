using System;
using UnityEngine;

public class ShunsoAA : RangedAutoAttack
{
    int m_NormalCharges;
    int m_CorruptedCharges;

    public event Action<int> m_EStacksOnHit;

    public void SetChargesOnHit(float Normal, float Corrupted)
    {
        m_NormalCharges=(int)Normal;
        m_CorruptedCharges=(int)Corrupted;
    }

    protected override void DamageEnemy(ITakeDamage Enemy)
    {
        base.DamageEnemy(Enemy);
        if(Enemy.GetCharacterStats().GetCorruptedHealth()>0.0f)
            m_EStacksOnHit?.Invoke(m_CorruptedCharges);
        else
            m_EStacksOnHit?.Invoke(m_NormalCharges);
    }
}
