using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillAttribute 
{
    public string m_AttributeId;
    public bool m_IsPct;
    public bool m_ShowScalingInPopup;
    public List<float> m_LevelScaling;
}

[Serializable]
public class SkillDescriptionDamage 
{
    public string m_DescriptionId;
    public bool m_IsMagicDamage;
    public float m_BaseDamageMultiplier;
    public float m_BonusDamagePct;
}

[CreateAssetMenu(menuName="Powers/Skill")]
public class Skill : Power
{
    public SkillAttribute[] m_AttributeList;
    public SkillDescriptionDamage[] m_DescriptionDamageList;
    public string[] m_ExtraSpecifications;
    public GameObject m_IndicatorUIObject;
    public float m_SkillDisabledTime;
    public bool m_CancelableWithMouseClick;
    bool m_UsingSkill;

    public override void SetInitStats()
    {
        base.SetInitStats();
        SetCooldown(0);
        SetUsingSkill(false);
    }

    public float GetMana(int Level)
    {
        return GetAttribute("Coste de Mana", Level);
    }

    public void SetCooldown(int Level)
    {
        SetCd(GetAttribute("Enfriamiento", Level));
    }

    public bool GetUsingSkill()
    {
        return m_UsingSkill;
    }
    public void SetUsingSkill(bool True)
    {
        m_UsingSkill=True;
    }

    public float GetAttribute(string Id, int Level) 
    {
        int l_Index=Level-1;
        if(l_Index<0)
            l_Index=0;
        foreach(SkillAttribute Attribute in m_AttributeList) 
        {
            if(Attribute.m_AttributeId==Id)
                return Attribute.m_LevelScaling[l_Index];
        }
        return 0.0f;
    }
}
