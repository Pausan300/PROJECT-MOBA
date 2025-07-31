using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffableEntity : MonoBehaviour
{
    private readonly Dictionary<Buff, TimedBuff> m_Buffs = new Dictionary<Buff, TimedBuff>();

    void Update()
    {
        foreach (var buff in m_Buffs.Values.ToList())
        {
            buff.Tick(Time.deltaTime);
            if (buff.m_IsFinished)
            {
                if (TryGetComponent(out CharacterMaster Player))
                {
                    Player.GetCharacterUI().DeleteBuffObject(m_Buffs[buff.m_Buff].GetUIOBject());
                }
                m_Buffs.Remove(buff.m_Buff);
            }
        }
    }

    public void AddBuff(TimedBuff Buff)
    {
        if (m_Buffs.ContainsKey(Buff.m_Buff))
        {
            m_Buffs[Buff.m_Buff].Activate(false);
            if(m_Buffs[Buff.m_Buff].GetUIOBject())
                m_Buffs[Buff.m_Buff].GetUIOBject().UpdateBuffObject();
        }
        else
        {
            m_Buffs.Add(Buff.m_Buff, Buff);
            Buff.Activate(true);
            if (TryGetComponent(out CharacterMaster Player))
            {
                Buff.SetUIObject(Player.GetCharacterUI().CreateBuffObject(Buff));
            }
        }
    }

    public bool IsMarkBuffActive(Buff _Buff)
    {
        if (m_Buffs.ContainsKey(_Buff))
        {
            TimedMarkBuff l_MarkBuff = (TimedMarkBuff)m_Buffs[_Buff];
            return l_MarkBuff.GetIsEffectActive();
        }
        return false;
    }

    public List<TimedBuff> GetBuffs()
    {
        return m_Buffs.Values.ToList();
    }
    public TimedBuff GetBuffWithKey(Buff _Key)
    {
        if (m_Buffs.ContainsKey(_Key))
            return m_Buffs[_Key];
        
        /*
        foreach (TimedBuff _Buff in m_Buffs.Values.ToList())
        {
            if (_Buff.m_Buff.m_BuffName == _Key)
            {
                return _Buff;
            }
        }
        */
        return null;
    }

    public void RemoveBuff(TimedBuff Buff) 
    {
        if(TryGetComponent(out CharacterMaster Player))
        {
            Player.GetCharacterUI().DeleteBuffObject(m_Buffs[Buff.m_Buff].GetUIOBject());
        }
        m_Buffs.Remove(Buff.m_Buff);
    }
}