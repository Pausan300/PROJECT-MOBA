using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TimedMarkBuff : TimedBuff
{
    private readonly GameObject m_Canvas;
    RectTransform m_MarkTransform;
    bool m_EffectActive;

    public TimedMarkBuff(float Duration, Buff buff, GameObject obj) : base(buff, obj)
    {
        buff.m_Duration=Duration;
        m_Canvas=obj.GetComponent<EnemyDummy>().GetIngameCharacterUI().gameObject;
    }
    protected override void ApplyEffect()
    {
        MarkBuff l_MarkBuff=(MarkBuff) m_Buff;

        if(!m_EffectActive)
        {
		    if(m_EffectStacks>=l_MarkBuff.m_MaxMarks)
            { 
                m_EffectStacks=l_MarkBuff.m_MaxMarks;
                for(int i=1; i<=l_MarkBuff.m_MaxMarks; ++i)
                    m_MarkTransform.GetChild(i).gameObject.SetActive(false);
                m_MarkTransform.GetChild(0).gameObject.SetActive(true);
                m_EffectActive=true;
            }
            else
            {
                if(m_EffectStacks==1)
                {
                    GameObject l_MarkObject=Object.Instantiate(l_MarkBuff.m_MarkObject, Vector3.zero, l_MarkBuff.m_MarkObject.transform.rotation, null);
                    l_MarkObject.GetComponent<NetworkObject>().Spawn();
                    l_MarkObject.GetComponent<NetworkObject>().TrySetParent(m_Canvas.transform, false);
                    m_MarkTransform=l_MarkObject.GetComponent<RectTransform>();
                    m_MarkTransform.localPosition=l_MarkBuff.m_MarkObject.transform.position;
                    m_MarkTransform.localRotation=l_MarkBuff.m_MarkObject.transform.rotation;
                }
                m_MarkTransform.GetChild(m_EffectStacks).gameObject.SetActive(true);
            }
        }
    }
    public bool GetIsEffectActive()
    {
        return m_EffectActive;
    }
    public override void End()
    {
        Object.Destroy(m_MarkTransform.gameObject);
    }
    protected override void ApplyTick()
    {
    }
}
