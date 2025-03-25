using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillAttribute 
{
    public string m_AttributeId;
    public bool m_IsPct;
    public List<float> m_LevelScaling;
}

[CreateAssetMenu(menuName="Powers/Skill")]
public class Skill : Power
{
    public float[] m_SkillCooldownPerLevel;
    public float[] m_SkillManaPerLevel;
    public float m_SkillDisabledTime;
    public bool m_CancelableWithMouseClick;
    bool m_UsingSkill;
    public SkillAttribute[] m_AttributeList;

    public override void SetInitStats()
    {
        base.SetInitStats();
        //m_SkillMana=m_SkillManaPerLevel[0];
        SetCooldown(0);
        SetUsingSkill(false);
    }

    //public void LevelUp()
    //{
    //    m_SkillLevel++;
    //    SetCd(m_SkillLevel);
    //    SetMana(m_SkillLevel);
    //}
    //public int GetLevel()
    //{
    //    return m_SkillLevel;
    //}
    //public void SetLevel(int Level)
    //{
    //    m_SkillLevel=Level;
    //}
    public float GetMana(int Level)
    {
        return GetAttribute("Coste de Mana", Level);
    }
    //public void SetMana(int Level)
    //{
    //    m_SkillMana=m_SkillManaPerLevel[Level-1];
    //}
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
