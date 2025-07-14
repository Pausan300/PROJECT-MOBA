using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TimedMarkBuff : TimedBuff
{
    private readonly GameObject m_Canvas;
    RectTransform m_MarkTransform;
    bool m_EffectActive;

    public TimedMarkBuff(float Duration, Buff _Buff, GameObject Character) : base(_Buff)
    {
        _Buff.m_Duration=Duration;
        m_AffectedCharacter=Character;
        m_Canvas=m_AffectedCharacter.GetComponent<EnemyDummy>().m_BuffMarksUI;
    }
    protected override void ApplyEffect()
    {
        MarkBuff l_MarkBuff=(MarkBuff) m_Buff;

        if(!m_EffectActive)
        {
            if(m_MarkTransform==null)
            {
                GameObject l_MarkObject=Object.Instantiate(l_MarkBuff.m_MarkObject, Vector3.zero, l_MarkBuff.m_MarkObject.transform.rotation, null);
                l_MarkObject.GetComponent<NetworkObject>().Spawn();
                l_MarkObject.GetComponent<NetworkObject>().TrySetParent(m_Canvas.transform, false);
                m_MarkTransform=l_MarkObject.GetComponent<RectTransform>();
                m_MarkTransform.localPosition=l_MarkBuff.m_MarkObject.transform.position;
                m_MarkTransform.localRotation=l_MarkBuff.m_MarkObject.transform.rotation;
            }

            if(l_MarkBuff.m_MaxMarks>1) 
            {
		        if(m_CurrentStacks>=l_MarkBuff.m_MaxMarks)
                { 
                    m_CurrentStacks=l_MarkBuff.m_MaxMarks;
                    for(int i=1; i<=l_MarkBuff.m_MaxMarks; ++i)
                        m_MarkTransform.GetChild(i).gameObject.SetActive(false);
                    m_MarkTransform.GetChild(0).gameObject.SetActive(true);
                    m_EffectActive=true;
                }
                else
                    m_MarkTransform.GetChild(m_CurrentStacks).gameObject.SetActive(true);
            }
            else 
            {
                m_MarkTransform.GetChild(0).gameObject.SetActive(true);
                m_EffectActive=true;
            }
        }
    }
    public bool GetIsEffectActive()
    {
        return m_EffectActive;
    }
    public override void End()
    {
        base.End();
        Object.Destroy(m_MarkTransform.gameObject);
    }
    protected override void ApplyTick(float delta)
    {
        if(m_EffectActive)
            InvokeTickEvent();
    }
}
