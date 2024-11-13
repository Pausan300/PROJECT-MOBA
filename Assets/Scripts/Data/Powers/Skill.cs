using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Powers/Skill")]
public class Skill : Power
{
    int m_SkillLevel;
    public float[] m_SkillCooldownPerLevel;
    public float[] m_SkillManaPerLevel;
    float m_SkillMana;

    public override void SetInitStats()
    {
        base.SetInitStats();
        m_SkillLevel=0;
        m_SkillMana=m_SkillManaPerLevel[0];
        SetCd(m_SkillCooldownPerLevel[0]);
    }

    public void LevelUp()
    {
        m_SkillLevel++;
        SetCd(m_SkillLevel);
        SetMana(m_SkillLevel);
    }
    public int GetLevel()
    {
        return m_SkillLevel;
    }
    public void SetLevel(int Level)
    {
        m_SkillLevel=Level;
    }
    public float GetMana()
    {
        return m_SkillMana;
    }
    public void SetMana(int Level)
    {
        m_SkillMana=m_SkillManaPerLevel[Level-1];
    }
    public void SetCd(int Level)
    {
        SetCd(m_SkillCooldownPerLevel[Level-1]);
    }
}