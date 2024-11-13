using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffableEntity : MonoBehaviour
{
    private readonly Dictionary<Buff, TimedBuff> m_Buffs=new Dictionary<Buff, TimedBuff>();

    void Update()
    {
        foreach(var buff in m_Buffs.Values.ToList())
        {
            buff.Tick(Time.deltaTime);
            if(buff.m_IsFinished)
            {
                m_Buffs.Remove(buff.m_Buff);
                if(TryGetComponent(out CharacterMaster Player))
                {
                    Player.GetCharacterUI().DeleteBuffObject(buff);
                }
            }
        }
    }
    public void AddBuff(TimedBuff buff)
    {
        if(m_Buffs.ContainsKey(buff.m_Buff))
        {
            m_Buffs[buff.m_Buff].Activate();
        }
        else
        {
            m_Buffs.Add(buff.m_Buff, buff);
            if(TryGetComponent(out CharacterMaster Player))
            {
                Player.GetCharacterUI().CreateBuffObject(buff);
            }
            buff.Activate();
        }
    }
    public bool IsBuffActive(Buff _Buff)
    {
        if(m_Buffs.ContainsKey(_Buff))
            return true;
        else
            return false;
    }
    public bool IsMarkBuffActive(Buff _Buff)
    {
        if(m_Buffs.ContainsKey(_Buff))
        {
            TimedMarkBuff l_MarkBuff=(TimedMarkBuff) m_Buffs[_Buff];
            return l_MarkBuff.GetIsEffectActive();
        }
        return false;
    }
}