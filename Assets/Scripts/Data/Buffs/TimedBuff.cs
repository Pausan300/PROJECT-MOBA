using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedBuff
{
    protected float m_TickRate = 0.5f;
    protected float m_TimeLeft;
    protected int m_EffectStacks;
    public Buff m_Buff { get; }
    protected readonly GameObject m_Obj;
    public bool m_IsFinished;
    protected float m_TimeSinceLastTick;

    public TimedBuff(Buff buff, GameObject obj)
    {
        m_Buff=buff;
        m_Obj=obj;
    }
    public void Tick(float delta)
    {
        m_TimeLeft-=delta;
        if(m_TimeSinceLastTick>=m_TickRate)
        {
            ApplyTick();
            m_TimeSinceLastTick=0;
        }
        if(m_TimeLeft<=0)
        {
            End();
            m_IsFinished=true;
        }
    }
    public void Activate()
    {
        if(m_Buff.m_IsEffectStacked || m_TimeLeft<=0)
        {
            m_EffectStacks++;
            ApplyEffect();
        }
        
        if(m_Buff.m_IsDurationRefreshed || m_TimeLeft<=0)
        {
            m_TimeLeft=m_Buff.m_Duration;
        }
    }
    public float GetCurrentDuration()
    {
        return m_TimeLeft;
    }
    protected abstract void ApplyEffect();
    protected abstract void ApplyTick();
    public abstract void End();
}