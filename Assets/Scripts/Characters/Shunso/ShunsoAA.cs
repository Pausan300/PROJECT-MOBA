using System;
using UnityEngine;

public class ShunsoAA : RangedAutoAttack
{
    public event Action<int> m_EStacksOnHit;

    protected override void DamageEnemy(ITakeDamage Enemy)
    {
        base.DamageEnemy(Enemy);
        if(Enemy.GetCharacterStats().GetCorruptedHealth()>0.0f)
            m_EStacksOnHit?.Invoke(2);
        else
            m_EStacksOnHit?.Invoke(1);
    }
}
