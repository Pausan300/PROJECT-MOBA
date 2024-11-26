using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public TextMeshProUGUI m_PowerName;
    public Image m_PowerImage;
    public TextMeshProUGUI m_PowerKey;
    public TextMeshProUGUI m_PowerCd;
    public TextMeshProUGUI m_SkillMana;
    public TextMeshProUGUI m_MainDescription;


    public void UpdatePopupInfo(string Description, string Name, string Key, string Cd, string Mana, Sprite Icon)
    {
        m_MainDescription.text=Description;
        m_PowerName.text=Name;
        m_PowerKey.text="["+Key+"]";
        m_PowerCd.text=Cd+" s";
        if(Mana!=null)
            m_SkillMana.text=Mana+" Mana";
        else
            m_SkillMana.text="";
        m_PowerImage.sprite=Icon;
        Debug.Log(m_MainDescription.transform.GetComponent<RectTransform>().sizeDelta.x);
        Debug.Log(m_MainDescription.GetPreferredValues(m_MainDescription.text));
    }
}
