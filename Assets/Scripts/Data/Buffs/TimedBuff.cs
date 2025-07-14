using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class TimedBuff
{
    public Buff m_Buff { get; }
    protected float m_TimeLeft;
    protected int m_CurrentStacks;
    bool m_LosingStacks;
    float m_LooseStacksTimer;
    protected float m_TimeSinceLastTick;
    int m_TicksLeft;
    public bool m_IsFinished;
    public GameObject m_AffectedCharacter;
    BuffDebuffObjectUI m_UIObject;

    public event Action<bool> m_OnFullStacks;
    public event Action<GameObject> m_OnTick;
    public event Action<GameObject> m_OnEnd;

    public TimedBuff(Buff _Buff)
    {
        m_Buff = _Buff;
    }
    public void Tick(float delta)
    {
        m_TimeLeft -= delta;
        m_TimeSinceLastTick += delta;
        if (m_TimeSinceLastTick >= m_Buff.m_EffectInterval)
        {
            ApplyTick(delta);
            m_TimeSinceLastTick = 0.0f;
            m_TicksLeft--;
        }
        if (m_TimeLeft <= 0.0f)
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
                LooseStack();
        }
    }
    public void Activate()
    {
        if (m_Buff.m_IsEffectStacked/* || m_TimeLeft <= 0.0f*/)
        {
            m_CurrentStacks++;
            if (m_CurrentStacks<=m_Buff.m_MaxStacks)
            {
                if(m_CurrentStacks==m_Buff.m_MaxStacks)
                    m_OnFullStacks?.Invoke(true);
                ApplyEffect();
            }
            else if(m_CurrentStacks>m_Buff.m_MaxStacks)
                m_CurrentStacks=m_Buff.m_MaxStacks;
            
            m_LosingStacks=false;
        }
        else
            ApplyEffect();

        if (m_Buff.m_IsDurationRefreshed || m_TimeLeft <= 0.0f)
        {
            m_TimeLeft = m_Buff.m_Duration;
            m_TicksLeft=Mathf.RoundToInt(m_Buff.m_Duration/m_Buff.m_EffectInterval);
        }
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
        m_TimeLeft = 0.0f;
    }
    public void InvokeTickEvent() 
    {
        m_OnTick?.Invoke(m_AffectedCharacter);
    }

    protected abstract void ApplyEffect();
    protected abstract void ApplyTick(float delta);

    public virtual void LooseStack() 
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
    public virtual void End() 
    {
        if(m_CurrentStacks>=m_Buff.m_MaxStacks)
            m_OnFullStacks?.Invoke(false);

        if(m_AffectedCharacter) 
        {
            for(int i=0; i<m_TicksLeft; ++i)
                m_OnTick?.Invoke(m_AffectedCharacter);

            m_OnEnd?.Invoke(m_AffectedCharacter);
        }

        m_IsFinished = true;
    }
}