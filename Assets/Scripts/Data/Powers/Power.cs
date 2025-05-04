using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : ScriptableObject
{
    public enum PowerType
    {
        PASSIVESKILL,
        QSKILL,
        WSKILL,
        ESKILL,
        RSKILL,
        SUMMONER1,
        SUMMONER2
    }
    public PowerType m_PowerType;
    public string m_PowerName;
    public Sprite m_Sprite;
    [TextArea(3, 10)]
    public string m_Description;
    float m_Cooldown;
    float m_Timer;
    bool m_OnCd;
    bool m_ZeroCd;

    [Header("Loads")]
    public bool m_HaveLoads = false;
    public int m_MAXLoads = 0;
    int m_ActualLoads = 0;

    public void Tick(float Delta)
    {
        m_Timer -= Delta;
        if (m_Timer <= 0.0f)
            m_OnCd = false;
    }
    public virtual void SetInitStats()
    {
        m_OnCd = false;
        m_Timer = 0.0f;
        m_ActualLoads = 0;
    }

    public float GetTimer()
    {
        return m_Timer;
    }
    public void SetTimer(float Time)
    {
        if (!m_ZeroCd)
            m_Timer = Time;
        else
            m_Timer = 0.5f;
    }

    public bool GetIsOnCd()
    {
        return m_OnCd;
    }
    public void SetIsOnCd(bool IsOnCd)
    {
        m_OnCd = IsOnCd;
    }

    public float GetCd()
    {
        return m_Cooldown;
    }
    public void SetCd(float Cd)
    {
        m_Cooldown = Cd;
    }

    public void SetZeroCooldown(bool IsZeroCd)
    {
        m_ZeroCd = IsZeroCd;
        if (m_ZeroCd && m_Timer > 0.0f)
            m_Timer = 0.0f;
    }
    public bool GetZeroCooldown()
    {
        return m_ZeroCd;
    }

    public bool GetHaveLoads()
    {
        return m_HaveLoads;
    }
    public int GetActualLoads()
    {
        return m_ActualLoads;
    }
    public void AddActualLoads(int Loads)
    {
        m_ActualLoads += Loads;
    }
    public int GetMAXLoads()
    {
        return m_MAXLoads;
    }
}