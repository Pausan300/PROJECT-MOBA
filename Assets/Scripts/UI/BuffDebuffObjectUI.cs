using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffObjectUI : MonoBehaviour
{
    public TimedBuff m_TimedBuff;
    public Image m_BuffImage;
    public Image m_BuffDurationImage;

	private void Update()
	{
		m_BuffDurationImage.fillAmount=Mathf.Lerp(0.0f, 1.0f, m_TimedBuff.GetCurrentDuration()/m_TimedBuff.m_Buff.m_Duration);
	}
}