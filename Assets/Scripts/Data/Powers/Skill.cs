using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillAttribute
{
    public string m_AttributeId;
    public bool m_ShowScalingInPopup;
    [Tooltip("Ej: Si el valor son segundos poner -> s || no poner espacios || si no tiene formato dejar vacío")]
    public string m_ValueFormat = "";
    public List<float> m_LevelScaling;
}

[Serializable]
public class SkillDescriptionDamage
{
    public string m_DescriptionId;
    public TextColors.TextColorTypes m_Color;
    public string m_AttributeId;
    public float m_MultiplyValue = 1;
    public SkillDescriptionTooltip[] m_SkillDescriptionTooltips;
}

[Serializable]
public class SkillDescriptionTooltip
{
    public enum SkillDescriptionTooltipTypes
    {
        TOTALATTACK,
        ADDITIONALATTACK,
        ADDITIONALLIFE,
        SKILLPOWER
    }
    public float m_Value;
    [Tooltip("Ej: Si el valor es porcentaje poner -> % || no poner espacios || si no tiene formato dejar vacío")]
    public string m_Format = "%";
    public string m_ToolTip;
    public SkillDescriptionTooltipTypes m_AtributeType;
    public TextColors.TextColorTypes m_Color;

}

[CreateAssetMenu(menuName = "Powers/Skill")]
public class Skill : Power
{
    [Header("Skill")]
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

    public void SetLoadCooldown(int Level)
    {
        SetCd(GetAttribute("Tiempo de recarga", Level));
    }


    public bool GetUsingSkill()
    {
        return m_UsingSkill;
    }
    public void SetUsingSkill(bool True)
    {
        m_UsingSkill = True;
    }

    public float GetAttribute(string Id, int Level)
    {
        int l_Index = Level - 1;
        if (l_Index < 0)
            l_Index = 0;
        foreach (SkillAttribute Attribute in m_AttributeList)
        {
            if (Attribute.m_AttributeId == Id)
                return Attribute.m_LevelScaling[l_Index];
        }
        Debug.LogError("COULDN'T FIND THE ATTRIBUTE, MAYBE IT DOESN'T EXIST OR THE NAME DOESN'T MATCH");
        return 0.0f;
    }
    public float GetAttribute(string Id)
    {
        foreach (SkillAttribute Attribute in m_AttributeList)
        {
            if (Attribute.m_AttributeId == Id)
                return Attribute.m_LevelScaling[0];
        }
        return 0.0f;
    }
}
