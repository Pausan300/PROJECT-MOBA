using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class TimedBuff
{
    protected float m_TickRate = 0f;
    protected float m_TimeLeft;
    protected int m_CurrentStacks;
    public Buff m_Buff { get; }
    public bool m_IsFinished;
    bool m_LosingStacks;
    float m_LooseStacksTimer;
    protected float m_TimeSinceLastTick;

    BuffDebuffObjectUI m_UIObject;

    public event Action<bool> m_OnFullStacks;

    public TimedBuff(Buff buff)
    {
        m_Buff = buff;
    }
    public void Tick(float delta)
    {
        m_TimeLeft -= delta;
        m_TimeSinceLastTick += delta;
        if (m_TimeSinceLastTick >= m_TickRate)
        {
            ApplyTick(delta);
            m_TimeSinceLastTick = 0;
        }
        if (m_TimeLeft <= 0)
        {
            if(m_Buff.m_IsEffectStacked) 
            {
                if(m_Buff.m_AreAllStacksLost) 
                    End();
                else 
                    m_LosingStacks=true;
            }
            else 
                End();
        }

        if(m_LosingStacks) 
        {
            m_LooseStacksTimer+=Time.deltaTime;
            if(m_LooseStacksTimer>=m_Buff.m_StackLooseInterval) 
            {
                if(m_CurrentStacks>=m_Buff.m_MaxStacks)
                   m_OnFullStacks?.Invoke(false);
                m_CurrentStacks--;
                if(m_UIObject!=null)
                    m_UIObject.UpdateBuffObject();
                m_LooseStacksTimer=0.0f;
                if(m_CurrentStacks<=0)
                    End();
            }
        }
    }
    public void Activate()
    {
        if (m_Buff.m_IsEffectStacked || m_TimeLeft <= 0)
        {
            m_CurrentStacks++;
            if (m_CurrentStacks>=m_Buff.m_MaxStacks)
            {
                m_OnFullStacks?.Invoke(true);
                m_CurrentStacks=m_Buff.m_MaxStacks;
            }
            m_LosingStacks=false;
        }

        if (m_Buff.m_IsDurationRefreshed || m_TimeLeft <= 0)
        {
            m_TimeLeft = m_Buff.m_Duration;
        }

        ApplyEffect();
    }
    public float GetCurrentDuration()
    {
        return m_TimeLeft;
    }
    public float GetCurrentStacks() 
    {
        return m_CurrentStacks;
    }
    public BuffDebuffObjectUI GetUIOBject() 
    {
        return m_UIObject;
    }
    public void SetUIObject(BuffDebuffObjectUI Object) 
    {
        m_UIObject=Object;
    }
    public void EndBuffNow()
    {
        m_TimeLeft = 0;
    }

    protected abstract void ApplyEffect();
    protected abstract void ApplyTick(float delta);
    public virtual void End() 
    {
        if(m_CurrentStacks>=m_Buff.m_MaxStacks)
            m_OnFullStacks?.Invoke(false);
        m_IsFinished = true;
    }
}