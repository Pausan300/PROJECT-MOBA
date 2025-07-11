using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffObjectUI : MonoBehaviour
{
    public TimedBuff m_TimedBuff;
    public Image m_BuffImage;
    public Image m_BuffDurationImage;
    public TextMeshProUGUI m_CumulativeAmountValueText;
    
    private void Update()
    {
        m_BuffDurationImage.fillAmount = Mathf.Lerp(0.0f, 1.0f, m_TimedBuff.GetCurrentDuration() / m_TimedBuff.m_Buff.m_Duration);
        if (/*m_TimedBuff.GetCurrentDuration() <= 0.0f || */m_TimedBuff.m_IsFinished)
            Destroy(gameObject);
    }

    public void UpdateBuffObject()
    {
        if (m_TimedBuff.m_Buff.m_ValueAmount != -1)
        {
            m_CumulativeAmountValueText.gameObject.SetActive(true);
            m_CumulativeAmountValueText.text = m_TimedBuff.m_Buff.m_ValueAmount.ToString();
        }
        else if(m_TimedBuff.m_Buff.m_IsEffectStacked)
        { 
            m_CumulativeAmountValueText.gameObject.SetActive(true);
            m_CumulativeAmountValueText.text = m_TimedBuff.GetCurrentStacks().ToString();
        }
        else
        {
            m_CumulativeAmountValueText.gameObject.SetActive(false);
        }
    }
}

